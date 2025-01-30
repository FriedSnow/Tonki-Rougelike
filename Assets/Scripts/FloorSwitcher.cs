using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorSwitcher : MonoBehaviour
{
    public List<GameObject> roomPrefabs; // Массив префабов обычных комнат
    public GameObject shopRoomPrefab; // Префаб комнаты-магазина
    public GameObject itemRoomPrefab; // Префаб комнаты с предметом
    public GameObject bossRoomPrefab; // Префаб комнаты с боссом
    private TankController player; // Ссылка на игрока
    private Rigidbody playerRb; // Ссылка на Rigidbody игрока
    private float moveDuration = .5f; // Длительность перемещения
    private RoomSpawner roomSpawner;
    private CameraController cameraController;
    private void Start()
    {
        roomSpawner = FindObjectOfType<RoomSpawner>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = FindObjectOfType<TankController>();
            cameraController = FindObjectOfType<CameraController>();
            playerRb = player.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero; // Останавливаем движение
                StartCoroutine(MoveDownAndRespawn());
            }
        }
    }

    private IEnumerator MoveDownAndRespawn()
    {
        Transform parentTransform = transform.parent;
        Vector3 initialPosition = parentTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            parentTransform.position = Vector3.Lerp(initialPosition, new Vector3(transform.position.x, -50, transform.position.z), elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parentTransform.position = new Vector3(transform.position.x, -50, transform.position.z);

        yield return new WaitForSeconds(1f); // Задержка после перемещения

        if (player != null)
        {
            player.transform.position = new Vector3(0, 2, 0);
            player.transform.rotation = Quaternion.identity;
        }
        cameraController.MoveCameraToRoom(new Vector3(0, 85, -18));
        //сюдасюдасюда логику респавна комнат
        roomSpawner.RegenerateRooms(roomPrefabs, shopRoomPrefab, itemRoomPrefab, bossRoomPrefab);
    }
}