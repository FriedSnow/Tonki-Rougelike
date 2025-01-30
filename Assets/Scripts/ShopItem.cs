using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShopItem : MonoBehaviour
{
    public bool isRare;
    public bool isPickup;
    public int cost = 1;
    public Transform itemSpawn;
    public List<GameObject> dropItemPool = new List<GameObject>(); // Используем List вместо массива для динамической загрузки
    public List<GameObject> rareDropItemPool = new List<GameObject>();
    public GameObject[] pickupsPool;

    private TankController player;
    private GameObject item;
    private Collider itemCollider;
    private bool isSpawned = false;

    private bool commonItemsLoaded = false;
    private bool rareItemsLoaded = false;

    private void Awake()
    {
        // Загружаем предметы при инициализации объекта
        LoadDropItems();
    }

    void Start()
    {
        // Если это Pickup, делаем его не редким, и наоборот
        if (isPickup) isRare = false;
        if (isRare) isPickup = false;

        player = FindObjectOfType<TankController>();

        // Проверяем, что ссылка на itemSpawn установлена
        if (itemSpawn == null)
        {
            Debug.LogError("itemSpawn не установлен в инспекторе!");
            return; // Выходим из метода, чтобы не вызвать NullReferenceException
        }

        // Запускаем проверку загрузки предметов и их спавн
        StartCoroutine(CheckItemsLoadedAndSpawn());
    }

    // Coroutine для проверки загрузки предметов и их спавна
    IEnumerator CheckItemsLoadedAndSpawn()
    {
        // Ждём завершения загрузки предметов
        yield return new WaitUntil(() => commonItemsLoaded && rareItemsLoaded);

        // Проверяем и выполняем спавн предмета после загрузки
        if (!isSpawned)
        {
            if (!isRare && !isPickup && dropItemPool.Count > 0)
            {
                item = Instantiate(dropItemPool[Random.Range(0, dropItemPool.Count)], itemSpawn.position, transform.rotation); // обычный предмет
            }
            else if (!isPickup && isRare && rareDropItemPool.Count > 0)
            {
                item = Instantiate(rareDropItemPool[Random.Range(0, rareDropItemPool.Count)], itemSpawn.position, transform.rotation); // редкий предмет
            }
            else if (isPickup && pickupsPool.Length > 0)
            {
                item = Instantiate(pickupsPool[Random.Range(0, pickupsPool.Length)], itemSpawn.position, transform.rotation); // пикап
            }

            // Проверяем, что объект item успешно создан
            if (item != null)
            {
                itemCollider = item.GetComponent<Collider>();
                if (itemCollider != null)
                {
                    itemCollider.enabled = false; // Отключаем коллайдер до покупки
                }
            }

            isSpawned = true; // Отмечаем, что предмет был заспавнен
        }
    }

    // Обрабатываем вход игрока в триггер магазина
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && player.coins >= cost && itemCollider != null)
        {
            player.coins -= cost; // Снимаем стоимость с игрока
            itemCollider.enabled = true; // Включаем коллайдер предмета
        }
    }

    // Загрузка предметов через Addressables
    void LoadDropItems()
    {
        // Загрузка обычных предметов с меткой "Common Items"
        Addressables.LoadAssetsAsync<GameObject>("Common Items", obj =>
        {
            dropItemPool.Add(obj); // Добавляем загруженный объект в пул обычных предметов
        }).Completed += OnCommonItemsLoaded;

        // Загрузка редких предметов с меткой "Rare Items"
        Addressables.LoadAssetsAsync<GameObject>("Rare Items", obj =>
        {
            rareDropItemPool.Add(obj); // Добавляем загруженный объект в пул редких предметов
        }).Completed += OnRareItemsLoaded;
    }

    // Обработка завершения загрузки обычных предметов
    void OnCommonItemsLoaded(AsyncOperationHandle<IList<GameObject>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Обычные предметы успешно загружены. Количество: " + dropItemPool.Count);
            commonItemsLoaded = true; // Отмечаем, что обычные предметы загружены
        }
        else
        {
            Debug.LogError("Ошибка при загрузке обычных предметов.");
        }
    }

    // Обработка завершения загрузки редких предметов
    void OnRareItemsLoaded(AsyncOperationHandle<IList<GameObject>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Редкие предметы успешно загружены. Количество: " + rareDropItemPool.Count);
            rareItemsLoaded = true; // Отмечаем, что редкие предметы загружены
        }
        else
        {
            Debug.LogError("Ошибка при загрузке редких предметов.");
        }
    }
}
