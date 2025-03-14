using UnityEngine;

public class ShrinkObject : MonoBehaviour
{
    public Transform targetObject; // Объект, который будет уменьшаться
    public float shrinkDuration = 2f; // Время, за которое объект должен исчезнуть
    private float startTime;
    private Vector3 originalScale;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("Целевой объект не назначен!");
            return;
        }

        // Сохраняем начальный масштаб объекта
        originalScale = targetObject.localScale;
        startTime = Time.time;
    }

    void Update()
    {
        if (targetObject == null) return;

        float currentTime = Time.time - startTime;
        
        // Проверяем, закончилось ли время уменьшения
        if (currentTime >= shrinkDuration)
        {
            targetObject.localScale = Vector3.zero; // Полностью уменьшаем объект до нуля
            enabled = false; // Останавливаем выполнение скрипта
            return;
        }

        // Вычисляем текущий масштаб линейно
        float t = currentTime / shrinkDuration;
        targetObject.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
    }
}