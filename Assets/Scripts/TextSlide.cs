using UnityEngine;
using TMPro;
using System.Collections;

public class TextSlide : MonoBehaviour
{
    // Переменная для хранения TMP_Text компонента
    public TMP_Text slidingText;
    public TMP_Text smolSlidingText;
    // Скорость движения текста
    public float slideSpeed = 500f;
    // Задержка в центре экрана
    public float centerDelay = 2f;
    // Границы экрана
    private float screenWidth;
    void Start()
    {
        GameObject textSlideObject = GameObject.Find("SlideText");
        if (textSlideObject != null)
        {
            slidingText = textSlideObject.GetComponent<TMP_Text>();
        }
        GameObject textSmolSlideObject = GameObject.Find("SmolSlideText");
        if (textSmolSlideObject != null)
        {
            smolSlidingText = textSmolSlideObject.GetComponent<TMP_Text>();
        }
        // Получаем ширину экрана
        screenWidth = Screen.width;
        // Стартуем корутину с анимацией
        // StartCoroutine(SlideText());
    }

    private Coroutine currentSlideRoutine;
    public void ShowItemName(string name, string name2 = "")
    {
        if (currentSlideRoutine != null)
        {
            StopCoroutine(currentSlideRoutine);
        }
        slidingText.text = name;
        smolSlidingText.text = name2;
        currentSlideRoutine = StartCoroutine(SlideText());
    }

    public void ShowItemName(string name, float delay, string name2 = "")
    {
        if (currentSlideRoutine != null)
        {
            StopCoroutine(currentSlideRoutine);
        }
        slidingText.text = name;
        smolSlidingText.text = name2;
        currentSlideRoutine = StartCoroutine(SlideText(delay));
    }

    public void ShowItemName(string name, Color color)
    {
        if (currentSlideRoutine != null)
        {
            StopCoroutine(currentSlideRoutine);
        }
        slidingText.text = name;
        slidingText.color = color;
        currentSlideRoutine = StartCoroutine(SlideText());
    }
    public void ShowItemName(string name, Color color, string name2 = "")
    {
        if (currentSlideRoutine != null)
        {
            StopCoroutine(currentSlideRoutine);
        }
        slidingText.text = name;
        smolSlidingText.text = name2;
        slidingText.color = color;
        currentSlideRoutine = StartCoroutine(SlideText());
    }

    public void ShowItemName(string name, Color color, string name2 = "", Color? color2 = null)
    {
        if (currentSlideRoutine != null)
        {
            StopCoroutine(currentSlideRoutine);
        }
        slidingText.text = name;
        smolSlidingText.text = name2;
        slidingText.color = color;
        smolSlidingText.color = color2 ?? Color.white; // Use Color.white if color2 is not provided
        currentSlideRoutine = StartCoroutine(SlideText());
    }

    // Корутин для перемещения текста
    IEnumerator SlideText()
    {
        // Получаем ширину текста
        float textWidth = slidingText.rectTransform.rect.width;
        // Начальная позиция справа за экраном
        Vector3 startPosition = new Vector3(screenWidth + textWidth, slidingText.rectTransform.anchoredPosition.y, 0);
        slidingText.rectTransform.anchoredPosition = startPosition;
        // Позиция в центре экрана
        Vector3 centerPosition = new Vector3(0, slidingText.rectTransform.anchoredPosition.y, 0);
        // Позиция слева за экраном
        Vector3 endPosition = new Vector3(-screenWidth - textWidth, slidingText.rectTransform.anchoredPosition.y, 0);

        // Двигаем текст вправо -> в центр
        while (Vector3.Distance(slidingText.rectTransform.anchoredPosition, centerPosition) > 0.1f)
        {
            slidingText.rectTransform.anchoredPosition = Vector3.MoveTowards(slidingText.rectTransform.anchoredPosition, centerPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }

        // Задержка на 2 секунды в центре
        yield return new WaitForSeconds(centerDelay);

        // Двигаем текст влево за экран
        while (Vector3.Distance(slidingText.rectTransform.anchoredPosition, endPosition) > 0.1f)
        {
            slidingText.rectTransform.anchoredPosition = Vector3.MoveTowards(slidingText.rectTransform.anchoredPosition, endPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }

        // Возвращаем цвет текста к белому
        slidingText.color = Color.white;
        smolSlidingText.color = Color.white;

        // Останавливаем текущую корутину после завершения
        currentSlideRoutine = null;
    }

    IEnumerator SlideText(float delay)
    {
        // Получаем ширину текста
        float textWidth = slidingText.rectTransform.rect.width;
        // Начальная позиция справа за экраном
        Vector3 startPosition = new Vector3(screenWidth + textWidth, slidingText.rectTransform.anchoredPosition.y, 0);
        slidingText.rectTransform.anchoredPosition = startPosition;
        // Позиция в центре экрана
        Vector3 centerPosition = new Vector3(0, slidingText.rectTransform.anchoredPosition.y, 0);
        // Позиция слева за экраном
        Vector3 endPosition = new Vector3(-screenWidth - textWidth, slidingText.rectTransform.anchoredPosition.y, 0);

        // Двигаем текст вправо -> в центр
        while (Vector3.Distance(slidingText.rectTransform.anchoredPosition, centerPosition) > 0.1f)
        {
            slidingText.rectTransform.anchoredPosition = Vector3.MoveTowards(slidingText.rectTransform.anchoredPosition, centerPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }

        // Задержка на указанное время в центре
        yield return new WaitForSeconds(delay);

        // Двигаем текст влево за экран
        while (Vector3.Distance(slidingText.rectTransform.anchoredPosition, endPosition) > 0.1f)
        {
            slidingText.rectTransform.anchoredPosition = Vector3.MoveTowards(slidingText.rectTransform.anchoredPosition, endPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }

        // Возвращаем цвет текста к белому
        slidingText.color = Color.white;
        smolSlidingText.color = Color.white;

        // Останавливаем текущую корутину после завершения
        currentSlideRoutine = null;
    }
}