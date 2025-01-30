using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthManager : MonoBehaviour
{
    public Transform healthPanel; // Панель, на которой будут спавниться иконки
    public Transform armorPanel; // Панель, на которой будут спавниться иконки
    public Sprite heartSprite; // Спрайт иконки сердца
    public Sprite armorSprite; // Спрайт иконки сердца
    private int currentHealth;
    private int currentArmor;

    private TankController player;

    // Используем Coroutine для отложенной инициализации
    private IEnumerator Start()
    {
        // Ожидаем один кадр, чтобы убедиться, что все объекты инициализированы
        yield return null;

        player = FindObjectOfType<TankController>();
        if (player == null)
        {
            Debug.LogError("TankController не найден на сцене!");
            yield break; // Останавливаем выполнение, если игрок не найден
        }

        if (healthPanel == null)
        {
            GameObject panelObject = GameObject.Find("HealthPanel");
            if (panelObject != null)
            {
                healthPanel = panelObject.transform;
            }
            else
            {
                Debug.LogError("HealthPanel не найден!");
                yield break; // Останавливаем выполнение, если панель не найдена
            }
        }

        if (armorPanel == null)
        {
            GameObject panelObject = GameObject.Find("ArmorPanel");
            if (panelObject != null)
            {
                armorPanel = panelObject.transform;
            }
            else
            {
                Debug.LogError("ArmorPanel не найден!");
                yield break; // Останавливаем выполнение, если панель не найдена
            }
        }

        currentHealth = player.health; // Проверяем, что игрок найден до инициализации здоровья
        currentArmor = player.armor;
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        foreach (Transform child in healthPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in armorPanel)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentHealth / 10; i++)
        {
            AddHeartIconToUI();
        }

        for (int i = 0; i < currentArmor / 10; i++)
        {
            AddArmorIconToUI();
        }
    }

    public void SetCurrentHealth(int health)
    {
        currentHealth = health;
        UpdateHealthUI();
    }
    public void SetCurrentArmor(int armor)
    {
        currentArmor = armor;
        UpdateHealthUI();
    }

    protected void AddHeartIconToUI()
    {
        if (heartSprite != null && healthPanel != null)
        {
            GameObject newHeartIcon = new GameObject("HeartIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Image heartImage = newHeartIcon.GetComponent<Image>();
            heartImage.sprite = heartSprite;
            newHeartIcon.transform.SetParent(healthPanel, false);

            RectTransform rectTransform = newHeartIcon.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            Debug.LogError("HeartSprite или HealthPanel не назначены!");
        }


    }
    void AddArmorIconToUI()
    {
        if (armorSprite != null && armorPanel != null)
        {
            GameObject newArmorIcon = new GameObject("ArmorIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Image armorImage = newArmorIcon.GetComponent<Image>();
            armorImage.sprite = armorSprite;
            newArmorIcon.transform.SetParent(armorPanel, false);

            RectTransform rectTransform = newArmorIcon.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            Debug.LogError("ArmorSprite или ArmorPanel не назначены!");
        }
    }
}
