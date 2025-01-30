using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject[] objects; // Массив объектов, которые могут быть заспавнены

    void Start()
    {
        // Получите имя объекта из PlayerPrefs
        string objectName = PlayerPrefs.GetString("ObjectToSpawn");

        // Найдите и заспавните объект
        foreach (GameObject obj in objects)
        {
            if (obj.name == objectName)
            {
                Instantiate(obj, spawnPoint.position, spawnPoint.rotation); // Замените Vector3.zero и Quaternion.identity на нужные вам значения
                break;
            }
        }

        // Очистите PlayerPrefs, чтобы избежать повторного спауна при последующих загрузках
        PlayerPrefs.DeleteKey("ObjectToSpawn");
    }
}
