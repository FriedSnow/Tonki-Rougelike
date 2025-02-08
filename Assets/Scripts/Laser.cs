using UnityEngine;
using TMPro; // Подключаем пространство имен для TextMeshPro

public class Laser : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;
    public bool isLock = false;
    private LineRenderer lr;
    [SerializeField] private Transform startPoint;
    private TMP_Text uiTextMeshPro; // Будет найден автоматически
    private Renderer rendererer;

    // Параметры для смещения текста
    [SerializeField] private float textOffsetUp = 1.0f; // Смещение вверх
    [SerializeField] private float offsetFromEndPoint = 0.5f; // Смещение от конца луча вперед/назад

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        rendererer = GetComponent<Renderer>();

        // Ищем объект TextMeshPro по имени "DistanceText" на сцене
        GameObject textObject = GameObject.Find("DistanceText");
        if (textObject != null)
        {
            uiTextMeshPro = textObject.GetComponent<TMP_Text>();
            if (uiTextMeshPro == null)
            {
                Debug.LogWarning("The object 'DistanceText' does not have a TMP_Text component.");
            }
        }
        else
        {
            Debug.LogWarning("Object with name 'DistanceText' not found on the scene.");
        }
    }

    Vector3 endPoint;

    private void Update()
    {
        lr.SetPosition(0, startPoint.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                endPoint = hit.point;

                if (rendererer != null && redMaterial != null && isLock)
                {
                    rendererer.material = redMaterial;
                }

                // Рассчитываем расстояние от начала луча до точки попадания
                float distance = Vector3.Distance(startPoint.position, endPoint);
                UpdateUITextMeshPro(distance, endPoint);
            }
            else
            {
                endPoint = transform.position + transform.forward * 5000;

                // Если нет столкновения, показываем максимальное расстояние
                float maxDistance = 5000f; // Максимальная длина лазера
                UpdateUITextMeshPro(maxDistance, endPoint);
            }
        }
        else
        {
            endPoint = transform.position + transform.forward * 5000;

            if (rendererer != null && greenMaterial != null && isLock)
            {
                rendererer.material = greenMaterial;
            }

            // Если нет попадания, показываем максимальное расстояние
            float maxDistance = 5000f; // Максимальная длина лазера
            UpdateUITextMeshPro(maxDistance, endPoint);
        }

        lr.SetPosition(1, endPoint);
    }

    private void UpdateUITextMeshPro(float distance, Vector3 position)
    {
        if (uiTextMeshPro != null)
        {
            uiTextMeshPro.text = $"{distance:F2}m"; // Устанавливаем текст

            // Вычисляем позицию текста с учетом оффсета
            Vector3 offsetPosition = position + transform.forward * offsetFromEndPoint + Vector3.up * textOffsetUp;
            uiTextMeshPro.transform.position = offsetPosition;
        }
    }

    public Vector3 GetLaserEndPoint()
    {
        return endPoint;
    }
}