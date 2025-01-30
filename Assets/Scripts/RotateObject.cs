using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SmoothRotateObject : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Объект, который будет заспавнен
    private float[] rotationAngles = { 0, 60, 120, 180, 240, 300 }; // Массив углов поворота
    private int currentIndex = 0; // Текущий индекс угла поворота
    public GameObject targetObject; // Родительский объект, который вращается
    public Button rotateLeftButton;
    public Button rotateRightButton;
    public Button startButton;
    public float rotationSpeed = 2f;
    private Quaternion targetRotation;

    public void LoadGame()
    {
        // Проверяем, что массив не пустой и индекс в пределах допустимого
        if (objectsToSpawn != null && objectsToSpawn.Length > 0 && currentIndex >= 0 && currentIndex < objectsToSpawn.Length)
        {
            // Сохраняем имя объекта для спавна
            PlayerPrefs.SetString("ObjectToSpawn", objectsToSpawn[currentIndex].name);
            Debug.Log("Сохранен объект для спавна: " + objectsToSpawn[currentIndex].name);
        }
        else
        {
            Debug.LogError("Ошибка: Некорректный индекс или пустой массив объектов для спавна.");
        }

        // Загружаем сцену
        SceneManager.LoadScene("GameScene");
    }

    void Start()
    {
        rotateLeftButton.onClick.AddListener(RotateLeft);
        rotateRightButton.onClick.AddListener(RotateRight);
        targetRotation = targetObject.transform.rotation;
    }

    void Update()
    {
        // Плавное вращение объекта
        targetObject.transform.rotation = Quaternion.Lerp(targetObject.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        LockStartButton();
    }
    void LockStartButton()
    {
        if (PlayerPrefs.GetInt($"Unlockable{currentIndex}") == 0) startButton.interactable = false;
        else startButton.interactable = true;
    }
    public void CheckUnlocks()
    {
        for (int i = 0; i < 6; i++)
        {
            Debug.Log($"Unlockable{i} - {PlayerPrefs.GetInt($"Unlockable{i}")}");
        }
    }
    public void DeleteUnlocks()
    {
        for (int i = 0; i < 6; i++)
        {
            PlayerPrefs.SetInt($"Unlockable{i}", 0);
        }
        PlayerPrefs.SetInt($"Unlockable0", 1);
    }

    void RotateLeft()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = rotationAngles.Length - 1;
        }

        SetRotation(currentIndex);
    }

    void RotateRight()
    {
        currentIndex++;
        if (currentIndex >= rotationAngles.Length)
        {
            currentIndex = 0;
        }

        SetRotation(currentIndex);
    }

    void SetRotation(int index)
    {
        targetRotation = Quaternion.Euler(0, rotationAngles[index], 0);
        Debug.Log("Установлен угол поворота: " + rotationAngles[index]);
    }
}
