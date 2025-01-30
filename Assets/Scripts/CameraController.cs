using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera; // Основная камера
    public float cameraMoveSpeed = 5f; // Скорость перемещения камеры

    private void Start()
    {
        // Найдем основную камеру на сцене
        mainCamera = Camera.main;
    }

    // Метод для перемещения камеры в центр комнаты
    public void MoveCameraToRoom(Vector3 roomCenter)
    {
        // Оставляем текущую высоту и глубину камеры, двигаем только по горизонтали (по оси x и z)
        Vector3 targetPosition = new Vector3(roomCenter.x, mainCamera.transform.position.y, roomCenter.z);

        // Запускаем Coroutine для плавного перемещения камеры
        StartCoroutine(MoveCameraSmoothly(targetPosition));
    }

    // Coroutine для плавного перемещения камеры
    private System.Collections.IEnumerator MoveCameraSmoothly(Vector3 targetPosition)
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.01f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraMoveSpeed * Time.deltaTime);
            yield return null;
        }

        // Устанавливаем камеру точно в центр, чтобы избежать неточностей
        mainCamera.transform.position = targetPosition;
    }
}
