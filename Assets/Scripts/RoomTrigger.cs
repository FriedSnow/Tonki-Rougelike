using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public Transform roomCenter; // Ссылка на Transform центра комнаты (задать через инспектор)

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что триггер пересек именно игрок (проверяем по тегу "Player")
        if (other.CompareTag("Player"))
        {
            // Находим контроллер камеры на сцене и перемещаем камеру
            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController != null)
            {
                cameraController.MoveCameraToRoom(roomCenter.position);
            }
        }
    }
}
