using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemSpawner : MonoBehaviour
{
    private List<GameObject>[] dropItemPools = new List<GameObject>[4]; // Пулы для 4 уровней редкости

    public void InitializePools()
    {
        for (int i = 0; i < 4; i++)
        {
            dropItemPools[i] = new List<GameObject>();
        }
    }

    public void LoadDropItems()
    {
        InitializePools();

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
                    Debug.Log($"Предметы с редкостью {currentRarity} успешно загружены.");
                }
                else
                {
                    Debug.LogError($"Ошибка при загрузке предметов с редкостью {currentRarity}.");
                }
            };
        }
    }

    public void SpawnItem(int rarityLevel, Vector3 position)
    {
        if (rarityLevel < 0 || rarityLevel >= 4)
        {
            Debug.LogWarning("Некорректный уровень редкости.");
            return;
        }

        GameObject itemToSpawn = null;
        if (dropItemPools[rarityLevel].Count > 0)
        {
            itemToSpawn = dropItemPools[rarityLevel][Random.Range(0, dropItemPools[rarityLevel].Count)];
        }

        if (itemToSpawn != null)
        {
            Instantiate(itemToSpawn, position, Quaternion.identity);
            Debug.Log($"Спавн предмета редкости {rarityLevel}");
        }
        else
        {
            Debug.LogWarning($"Не удалось найти предмет для спавна с редкостью: {rarityLevel}");
        }
    }
}