using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // UI Меню паузы
    private bool isPaused = false; // Статус игры
    private TankController player;
    public string roomTag = "Room"; // Тег для поиска комнат
    public Vector3 offset = new Vector3(0, 5, 0); // Оффсет для телепортации (например, чтобы игрок не оказывался под полом)
    private void Start()
    {
        player = FindObjectOfType<TankController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Menu"))
        {
            player = FindObjectOfType<TankController>();
            if (isPaused)
            {
                Resume();
                player.EnableCursor();
            }
            else
            {
                Pause();
                player.EnableCursor(false);
            }
        }
        // Debug.Log(player.gameObject.transform.position);
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);  // Скрыть меню паузы
        Time.timeScale = 1f;           // Возобновить время
        isPaused = false;              // Установить статус паузы в false
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);   // Показать меню паузы
        Time.timeScale = 0f;           // Остановить время
        isPaused = true;               // Установить статус паузы в true
    }
    public void Unstuck()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag(roomTag);

        if (rooms.Length == 0)
        {
            Debug.LogWarning($"No rooms found with tag: {roomTag}");
            return;
        }

        GameObject nearestRoom = null;
        float shortestDistance = Mathf.Infinity;

        // Проходим по всем комнатам и ищем ближайшую
        foreach (var room in rooms)
        {
            float distance = Vector3.Distance(player.transform.position, room.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestRoom = room;
            }
        }

        if (nearestRoom != null)
        {
            // Вычисляем позицию для телепортации с учётом оффсета
            Vector3 teleportPosition = nearestRoom.transform.position + offset;

            // Телепортируем игрока
            player.transform.position = teleportPosition;
            player.transform.rotation = Quaternion.identity;

            Debug.Log($"Player teleported to nearest room: {nearestRoom.name}, Position: {teleportPosition}");
        }
        else
        {
            Debug.LogError("Failed to find the nearest room.");

        }
    }
    private void OnDestroy()
    {
        Resume();
    }
}
