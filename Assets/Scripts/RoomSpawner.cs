using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public GameObject[] roomPrefabs; // Массив префабов обычных комнат
    public GameObject shopRoomPrefab; // Префаб комнаты-магазина
    public GameObject itemRoomPrefab; // Префаб комнаты с предметом
    public GameObject bossRoomPrefab; // Префаб комнаты с боссом
    public GameObject closedRoomPrefab; // Префаб закрытой комнаты
    public int roomsToSpawn = 10; // Количество комнат для спауна
    public float roomWidth = 16.0f; // Ширина комнаты (в единицах мира)
    public float roomHeight = 9.0f; // Высота комнаты (в единицах мира)
    private GameObject startRoom; // Стартовая комната
    private List<GameObject> spawnedRooms = new List<GameObject>(); // Список всех сгенерированных комнат
    private List<Vector2Int> spawnedRoomPositions = new List<Vector2Int>();
    private HashSet<Vector2Int> availablePositions = new HashSet<Vector2Int>();
    private int roomIncrement;
    private Vector2Int shopRoomPosition; // Позиция комнаты-магазина
    private Vector2Int itemRoomPosition; // Позиция комнаты с предметом
    private Vector2Int bossRoomPosition; // Позиция комнаты с боссом

    void Start()
    {
        // Найти стартовую комнату
        startRoom = GameObject.FindWithTag("StartRoom");
        if (startRoom == null)
        {
            Debug.LogError("Start room not found! Make sure the start room has the 'StartRoom' tag.");
            return;
        }
        // Спавним комнаты вокруг стартовой
        roomIncrement = Random.Range(-2, 3);
        roomsToSpawn += roomIncrement;
        Debug.Log(roomsToSpawn + " Rooms spawned");
        // Спавним обычные комнаты
        SpawnRoomsAroundStart(startRoom.transform.position, roomsToSpawn);
        // Спавним комнату-магазин и комнату с боссом в свободные позиции
        SpawnShopRoom();
        SpawnItemRoom();
        SpawnBossRoom();
        // Спавним закрытые комнаты по периметру
        SpawnClosedRooms();
    }

    // Новый метод для удаления всех комнат кроме стартовой и генерации новых
    public void RegenerateRooms(List<GameObject> newRoomPrefabs, GameObject newShopRoomPrefab,
                            GameObject newItemRoomPrefab, GameObject newBossRoomPrefab, bool isEnd = false)
    {
        // Удаляем все объекты ShopItem в сцене
        ShopItem[] shopItems = FindObjectsOfType<ShopItem>();
        foreach (var item in shopItems)
        {
            Destroy(item.gameObject);
        }

        // Удаляем все объекты с тегом "Pickable"
        GameObject[] pickableObjects = GameObject.FindGameObjectsWithTag("Pickable");
        foreach (var pickable in pickableObjects)
        {
            Destroy(pickable);
        }

        // Удаляем все сгенерированные комнаты, кроме стартовой
        foreach (var room in spawnedRooms)
        {
            if (room != startRoom)
            {
                Destroy(room);
            }
        }

        // Очищаем списки
        spawnedRooms.Clear();
        spawnedRoomPositions.Clear();
        availablePositions.Clear();

        // Обновляем префабы
        roomPrefabs = newRoomPrefabs.ToArray();
        shopRoomPrefab = newShopRoomPrefab;
        itemRoomPrefab = newItemRoomPrefab;
        bossRoomPrefab = newBossRoomPrefab;

        // Если это конец — спавним только комнату с боссом
        if (isEnd)
        {
            Vector2Int startGridPosition = WorldToGrid(startRoom.transform.position);
            spawnedRoomPositions.Add(startGridPosition); // добавляем стартовую комнату!
            availablePositions.Add(startGridPosition);

            SpawnSingleBossRoom();
            SpawnClosedRooms();
        }

        else
        {
            // Иначе — обычная генерация
            SpawnRoomsAroundStart(startRoom.transform.position, roomsToSpawn);
            SpawnShopRoom();
            SpawnItemRoom();
            SpawnBossRoom();
            SpawnClosedRooms();
        }
    }
    void SpawnRoomsAroundStart(Vector3 startPosition, int count)
    {
        Vector2Int startGridPosition = WorldToGrid(startPosition);
        spawnedRoomPositions.Add(startGridPosition);
        availablePositions.Add(startGridPosition); // Добавляем стартовую комнату как занятую

        Queue<Vector2Int> positionsToCheck = new Queue<Vector2Int>();
        positionsToCheck.Enqueue(startGridPosition);

        Vector2Int[] directions = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (positionsToCheck.Count > 0 && spawnedRoomPositions.Count < count)
        {
            Vector2Int currentPos = positionsToCheck.Dequeue();
            ShuffleArray(directions);

            int maxRoomsToSpawnAround = Random.Range(1, 10);
            if (maxRoomsToSpawnAround > 3) maxRoomsToSpawnAround = 1;
            int roomsSpawnedAround = 0;

            foreach (Vector2Int direction in directions)
            {
                if (roomsSpawnedAround >= maxRoomsToSpawnAround) break;

                Vector2Int neighborPos = currentPos + direction;

                if (!spawnedRoomPositions.Contains(neighborPos))
                {
                    spawnedRoomPositions.Add(neighborPos);
                    availablePositions.Add(neighborPos); // Добавляем позицию как занятую
                    positionsToCheck.Enqueue(neighborPos);
                    SpawnRoom(neighborPos);
                    roomsSpawnedAround++;

                    if (spawnedRoomPositions.Count >= count)
                        return;
                }
            }
        }
    }
    void SpawnRoom(Vector2Int gridPosition)
    {
        GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
        Vector3 worldPosition = GridToWorld(gridPosition);
        GameObject newRoom = Instantiate(roomPrefab, worldPosition, Quaternion.identity);
        spawnedRooms.Add(newRoom); // Добавляем в список сгенерированных комнат
    }
    void SpawnShopRoom()
    {
        shopRoomPosition = GetEmptyPosition();
        if (shopRoomPosition != Vector2Int.zero)
        {
            Vector3 worldPosition = GridToWorld(shopRoomPosition);
            GameObject shopRoom = Instantiate(shopRoomPrefab, worldPosition, Quaternion.identity);
            spawnedRooms.Add(shopRoom); // Добавляем магазин в список комнат
            spawnedRoomPositions.Add(shopRoomPosition);
        }
    }
    void SpawnItemRoom()
    {
        itemRoomPosition = GetEmptyPosition();
        if (itemRoomPosition != Vector2Int.zero)
        {
            Vector3 worldPosition = GridToWorld(itemRoomPosition);
            GameObject itemRoom = Instantiate(itemRoomPrefab, worldPosition, Quaternion.identity);
            spawnedRooms.Add(itemRoom); // Добавляем комнату с предметом в список комнат
            spawnedRoomPositions.Add(itemRoomPosition);
        }
    }
    void SpawnBossRoom()
    {
        bossRoomPosition = GetEmptyPositionFarthestFromStart();
        if (bossRoomPosition != Vector2Int.zero)
        {
            Vector3 worldPosition = GridToWorld(bossRoomPosition);
            GameObject bossRoom = Instantiate(bossRoomPrefab, worldPosition, Quaternion.identity);
            spawnedRooms.Add(bossRoom); // Добавляем комнату с боссом в список комнат
            spawnedRoomPositions.Add(bossRoomPosition);
        }
    }
    void SpawnClosedRooms()
    {
        List<Vector2Int> surroundingPositions = GetSurroundingPositions();

        foreach (Vector2Int position in surroundingPositions)
        {
            if (!spawnedRoomPositions.Contains(position) && position != shopRoomPosition && position != bossRoomPosition && position != itemRoomPosition)
            {
                Vector3 worldPosition = GridToWorld(position);
                GameObject closedRoom = Instantiate(closedRoomPrefab, worldPosition, Quaternion.identity);
                spawnedRooms.Add(closedRoom); // Добавляем закрытую комнату в список комнат
                spawnedRoomPositions.Add(position);
            }
        }
    }
    Vector2Int GetEmptyPosition()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        foreach (Vector2Int pos in GetSurroundingPositions())
        {
            if (!spawnedRoomPositions.Contains(pos))
            {
                emptyPositions.Add(pos);
            }
        }

        if (emptyPositions.Count > 0)
        {
            return emptyPositions[Random.Range(0, emptyPositions.Count)];
        }

        return Vector2Int.zero; // Возвращаем пустое значение, если нет свободных позиций
    }
    Vector2Int GetEmptyPositionFarthestFromStart()
    {
        Vector2Int startGridPosition = WorldToGrid(startRoom.transform.position);
        Vector2Int farthestRoom = Vector2Int.zero;
        float maxDistance = 0;

        foreach (Vector2Int roomPos in GetSurroundingPositions())
        {
            if (!spawnedRoomPositions.Contains(roomPos))
            {
                float distance = Vector2.Distance(roomPos, startGridPosition);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestRoom = roomPos;
                }
            }
        }

        return farthestRoom;
    }
    List<Vector2Int> GetSurroundingPositions()
    {
        List<Vector2Int> surroundingPositions = new List<Vector2Int>();

        foreach (Vector2Int pos in spawnedRoomPositions)
        {
            surroundingPositions.Add(pos + Vector2Int.up);
            surroundingPositions.Add(pos + Vector2Int.down);
            surroundingPositions.Add(pos + Vector2Int.left);
            surroundingPositions.Add(pos + Vector2Int.right);
        }

        return surroundingPositions;
    }
    Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * roomWidth, 0, gridPosition.y * roomHeight);
    }
    Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x / roomWidth), Mathf.RoundToInt(worldPosition.z / roomHeight));
    }
    void ShuffleArray(Vector2Int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            Vector2Int temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
    void SpawnSingleBossRoom()
    {
        Vector2Int startGridPosition = WorldToGrid(startRoom.transform.position);

        // Спавним босса рядом со стартовой комнатой
        Vector2Int bossDirection = new Vector2Int(1, 0); // например, справа от старта
        bossRoomPosition = startGridPosition + bossDirection;

        Vector3 worldPosition = GridToWorld(bossRoomPosition);
        GameObject bossRoom = Instantiate(bossRoomPrefab, worldPosition, Quaternion.identity);
        spawnedRooms.Add(bossRoom);
        spawnedRoomPositions.Add(bossRoomPosition);
    }
}