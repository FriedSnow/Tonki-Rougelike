using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShopItem : MonoBehaviour
{
    public bool isRare; // Оставим для совместимости, но будем использовать редкость из пула
    public bool isPickup;
    public int cost = 1;
    public Transform itemSpawn;
    public List<GameObject>[] dropItemPools = new List<GameObject>[4]; // Пулы для 4 уровней редкости
    public GameObject[] pickupsPool;
    private TankController player;
    private GameObject item;
    private Collider itemCollider;
    private bool isSpawned = false;
    private bool[] itemsLoaded = new bool[4]; // Массив для отслеживания загрузки каждого пула

    private void Awake()
    {
        // Инициализируем пуллы
        for (int i = 0; i < 4; i++)
        {
            dropItemPools[i] = new List<GameObject>();
        }
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
        // Ждём завершения загрузки всех предметов
        yield return new WaitUntil(() => itemsLoaded[0] && itemsLoaded[1] && itemsLoaded[2] && itemsLoaded[3]);
        // Проверяем и выполняем спавн предмета после загрузки
        if (!isSpawned)
        {
            int rarityLevel = GetRarityLevel();
            if (rarityLevel >= 0 && rarityLevel < 4)
            {
                if (dropItemPools[rarityLevel].Count > 0)
                {
                    item = Instantiate(dropItemPools[rarityLevel][Random.Range(0, dropItemPools[rarityLevel].Count)], itemSpawn.position, transform.rotation); // Предмет определенной редкости
                }
            }
            else if (rarityLevel == -1 && pickupsPool.Length > 0)
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
        // Загрузка предметов для каждого уровня редкости
        for (int rarityLevel = 0; rarityLevel < 4; rarityLevel++)
        {
            int currentRarity = rarityLevel;
            string label = $"{rarityLevel}QualityItems";

            Addressables.LoadAssetsAsync<GameObject>(label, obj =>
            {
                dropItemPools[currentRarity].Add(obj);
            }).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Предметы с редкостью {currentRarity} успешно загружены. Количество: {dropItemPools[currentRarity].Count}");
                    itemsLoaded[currentRarity] = true; // Отмечаем, что предметы данного уровня редкости загружены
                }
                else
                {
                    Debug.LogError($"Ошибка при загрузке предметов с редкостью {currentRarity}.");
                }
            };
        }
    }

    // Получение уровня редкости с учетом заданных вероятностей
    private int GetRarityLevel()
    {
        float rnd = Random.Range(0f, 100f);

        if (rnd < 10)
            return 0; // Качество 0 (10% шанс)
        else if (rnd < 40) // 10 + 30 = 40
            return 1; // Качество 1 (30% шанс)
        else if (rnd < 60) // 40 + 20 = 60
            return 2; // Качество 2 (20% шанс)
        else if (rnd < 70) // 60 + 10 = 70
            return 3; // Качество 3 (10% шанс)
        else
            return -1; // Выбор случайного предмета из pickupsPool (30% шанс)
    }
}