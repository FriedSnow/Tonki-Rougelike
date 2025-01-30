using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // UI Меню паузы
    private bool isPaused = false; // Статус игры
    private TankController player;
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
    private void OnDestroy()
    {
        Resume();
    }
}
