using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class StandardItem : MonoBehaviour
{
    // public int amount;
    protected static TankController player;
    [SerializeField] private Sprite itemIconSprite;
    private Transform itemsPanel;
    private Transform primaryAttackPanel;
    private Transform secondaryAttackPanel;
    protected static TextSlide textSlide;
    [SerializeField] private GameObject coinPickupSoundPrefab;
    private List<Sprite> collectedItems = new List<Sprite>();
    private bool isMovingDown = false;
    private float moveSpeed = 1.0f;
    private float rotationSpeed = 100.0f;
    private float minY = 0.0f;
    private float maxY = 10.0f;
    Vector3 scale;
    bool collected = false;

    private void Awake()
    {
        if (itemsPanel == null)
        {
            GameObject panelObject = GameObject.Find("ItemsPanel");
            if (panelObject != null)
            {
                itemsPanel = panelObject.transform;
            }
            else
            {
                Debug.LogError("ItemsPanel not found!");
            }
        }

        // Corrected condition: should be "primaryAttackPanel == null" to assign it correctly
        if (primaryAttackPanel == null)
        {
            GameObject primaryPanelObject = GameObject.Find("PrimaryPanel");
            if (primaryPanelObject != null)
            {
                primaryAttackPanel = primaryPanelObject.transform;
            }
            else
            {
                Debug.LogError("PrimaryPanel not found!");
            }
        }

        // Corrected condition: should be "secondaryAttackPanel == null" to assign it correctly
        if (secondaryAttackPanel == null)
        {
            GameObject secondaryPanelObject = GameObject.Find("SecondaryPanel");
            if (secondaryPanelObject != null)
            {
                secondaryAttackPanel = secondaryPanelObject.transform;
            }
            else
            {
                Debug.LogError("SecondaryPanel not found!");
            }
        }

        if (textSlide == null)
        {
            textSlide = FindObjectOfType<TextSlide>();
            if (textSlide == null)
            {
                Debug.LogError("TextSlide not found!");
            }
        }

        scale = transform.localScale;
    }


    protected static void GetPlayer()
    {
        if (player == null)
        {
            player = FindObjectOfType<TankController>();

            if (player == null)
            {
                Debug.LogError("Player (TankController) not found in the scene!");
            }
        }
        if (textSlide == null)
        {
            textSlide = FindObjectOfType<TextSlide>();

            if (textSlide == null)
            {
                Debug.LogError("TextSlide not found in the scene!");
            }
        }

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player == null)
            {
                GetPlayer();
            }

            if (player != null)
            {
                ApplyEffect();
                MoveToPlayer();
            }
            else
            {
                Debug.LogError("Player (TankController) is still null.");
            }
        }
    }

    protected abstract void ApplyEffect();
    protected void AddAttackSprite(bool first, bool second = false)
    {
        // Добавляем в primaryAttackPanel
        if (itemIconSprite != null && primaryAttackPanel != null && first)
        {
            // Находим старый значок, если он существует, и удаляем его
            Transform existingIcon = primaryAttackPanel.Find("ItemIcon");
            if (existingIcon != null)
            {
                Destroy(existingIcon.gameObject);
            }

            // Создаем новую иконку
            GameObject newItemIcon = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Image itemImage = newItemIcon.GetComponent<Image>();
            itemImage.sprite = itemIconSprite;
            newItemIcon.transform.SetParent(primaryAttackPanel, false);
        }
        else if (first)
        {
            Debug.LogError("ItemIconSprite или PrimaryPanel не установлен!");
        }

        // Добавляем в secondaryAttackPanel
        if (itemIconSprite != null && secondaryAttackPanel != null && second)
        {
            // Находим старый значок, если он существует, и удаляем его
            Transform existingIcon = secondaryAttackPanel.Find("ItemIcon");
            if (existingIcon != null)
            {
                Destroy(existingIcon.gameObject);
            }

            // Создаем новую иконку
            GameObject newItemIcon = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Image itemImage = newItemIcon.GetComponent<Image>();
            itemImage.sprite = itemIconSprite;
            newItemIcon.transform.SetParent(secondaryAttackPanel, false);
        }
        else if (second)
        {
            Debug.LogError("ItemIconSprite или SecondaryPanel не установлен!");
        }
    }


    protected void AddItemToUI()
    {
        if (itemIconSprite != null && itemsPanel != null)
        {
            GameObject newItemIcon = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Image itemImage = newItemIcon.GetComponent<Image>();
            itemImage.sprite = itemIconSprite;
            newItemIcon.transform.SetParent(itemsPanel, false);

            RectTransform rectTransform = newItemIcon.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;

            collectedItems.Add(itemIconSprite);
        }
        else
        {
            Debug.LogError("ItemIconSprite or ItemsPanel is not set!");
        }
        player.Inventory.Add(name);
    }

    public void PlaySound()
    {
        if (coinPickupSoundPrefab != null)
        {
            GameObject coinPickupSound = Instantiate(coinPickupSoundPrefab, transform.position, transform.rotation);
            Destroy(coinPickupSound, 3f);
        }
    }

    protected void Move()
    {
        if (!collected)
        {
            transform.Translate(Vector3.up * (isMovingDown ? -moveSpeed : moveSpeed) * Time.deltaTime);

            if (isMovingDown && transform.position.y <= minY + 5)
            {
                isMovingDown = false;
            }
            else if (!isMovingDown && transform.position.y >= maxY)
            {
                isMovingDown = true;
            }

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

    }

    protected void MoveToPlayer()
    {
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero;
        HideItem();
    }

    protected void HideItem()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        scale = transform.localScale;
        scale.x = .01f;
        scale.y = .01f;
        scale.z = .01f;
        transform.localScale = scale;

        collected = true;

        GameObject pointLight = transform.Find("DownLight")?.gameObject;
        if (pointLight != null)
        {
            // Debug.Log("DownLight found, turning off the light.");
            pointLight.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DownLight not found in the item's hierarchy.");
        }
    }
    // public void Stack()
    // {
    //     amount += amount;
    // }
}