using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject[] enemies; // Префабы врагов для спауна
    public Transform[] spawnPoints; // Точки спауна врагов
    public GameObject[] roomBarriers; // Объекты, которые выступают и стенами, и дверями
    public GameObject closeSoundPrefab; // 
    public GameObject openSoundPrefab; // 
    public GameObject flagPrefab; // 
    public bool isCleared = false; // Флаг зачистки комнаты
    public bool isShop = false; // Флаг зачистки комнаты
    private List<GameObject> spawnedEnemies = new List<GameObject>(); // Список заспавненных врагов

    public float doorCloseDelay = 1f; // Задержка закрытия дверей (в секундах)
    public float doorOpenDelay = 1f;  // Задержка открытия дверей (в секундах)

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что игрок вошел в комнату, и она не была ранее зачищена
        if (other.CompareTag("Player") && !isCleared)
        {
            StartRoomEvent();
        }
    }

    // Метод для начала события в комнате (вход игрока)
    void StartRoomEvent()
    {
        SpawnEnemies();
        // Спавним врагов

        if (!isShop)
        {
            // Закрываем выходы с задержкой
            StartCoroutine(CloseDoorsWithDelay());
        }
        else
            isCleared = true;

        if (flagPrefab != null)
            Instantiate(flagPrefab, transform.position + new Vector3(70, 0, 35), transform.rotation);
        // Слушаем уничтожение врагов
        StartCoroutine(CheckForRoomClear());
    }

    // Coroutine для закрытия дверей с задержкой
    System.Collections.IEnumerator CloseDoorsWithDelay()
    {
        // Ждем заданную задержку перед закрытием дверей
        yield return new WaitForSeconds(doorCloseDelay);

        // Закрываем двери (активируем барьеры)
        SetRoomBarriersActive(true);
        GameObject sound = Instantiate(closeSoundPrefab, transform.position, transform.rotation);
        Destroy(sound, 5f);
        // Debug.Log("Doors closed after delay!");
    }

    // Coroutine для открытия дверей с задержкой
    System.Collections.IEnumerator OpenDoorsWithDelay()
    {
        // Ждем заданную задержку перед открытием дверей
        yield return new WaitForSeconds(doorOpenDelay);

        // Открываем двери (убираем барьеры)
        SetRoomBarriersActive(false);
        GameObject sound = Instantiate(openSoundPrefab, transform.position, transform.rotation);
        Destroy(sound, 5f);

        // Помечаем комнату как зачищенную

        isCleared = true;
        // Debug.Log("Doors opened after delay!");
    }

    // Метод для включения/выключения барьеров (закрытие и открытие комнаты)
    void SetRoomBarriersActive(bool active)
    {
        foreach (GameObject barrier in roomBarriers)
        {
            barrier.SetActive(active); // Активируем или деактивируем барьеры
        }
    }

    // Спавн врагов в комнате
    void SpawnEnemies()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            // Спавним случайного врага в каждой точке спауна
            GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            spawnedEnemies.Add(spawnedEnemy);
        }
    }

    // Coroutine для проверки зачистки комнаты
    System.Collections.IEnumerator CheckForRoomClear()
    {
        while (spawnedEnemies.Count > 0)
        {
            // Удаляем уничтоженных врагов из списка
            spawnedEnemies.RemoveAll(enemy => enemy == null);

            yield return new WaitForSeconds(.5f); // Проверяем каждую секунду
        }

        // Когда все враги уничтожены, открываем двери с задержкой
        StartCoroutine(OpenDoorsWithDelay());
    }
}

