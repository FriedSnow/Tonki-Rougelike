using UnityEngine;
using UnityEngine.UI; // Для работы с UI
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    public ItemType selectedItem; // Тип предмета (выпадающий список в инспекторе)
    public Sprite itemIconSprite; // Спрайт для отображения миниатюры предмета
    public GameObject coinPickupSoundPrefab;

    private TankController player;
    private static TextSlide textSlide;

    // Ссылка на панель, куда будут добавляться иконки предметов
    private static Transform itemsPanel;

    // Список собранных предметов
    private static List<Sprite> collectedItems = new List<Sprite>();

    public float moveSpeed = 1f;
    public float rotationSpeed = 45f;
    public float minY = 2f;
    public float maxY = 4f;
    private bool isMovingDown = true;

    private void Update()
    {
        Move();
    }
    private void Start()
    {
        player = FindObjectOfType<TankController>();

        // Ищем панель для предметов только один раз, если она еще не найдена
        if (itemsPanel == null)
        {
            itemsPanel = GameObject.Find("ItemsPanel").transform; // Ищем панель по имени
        }

        // Ищем TextSlide для отображения текста
        if (textSlide == null)
        {
            textSlide = FindObjectOfType<TextSlide>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (selectedItem)
            {
                case ItemType.allStatsUp: //
                    ApplyAllStatsUp();
                    break;

                case ItemType.healthUp: //
                    player.maxHealth++;
                    textSlide.ShowItemName("Health UP\n+1");
                    break;

                case ItemType.speedUp: //
                    player.maxSpeed += 5;
                    player.turnSpeed += 10;
                    textSlide.ShowItemName("Speed UP\n+5");
                    break;

                case ItemType.fireRateUp: //
                    if (player.fireRate > .01f)
                        player.fireRate *= .9f;
                    textSlide.ShowItemName("Fire rate UP\n+10%");
                    break;

                case ItemType.luckUp:  //
                    player.luck += 5;
                    textSlide.ShowItemName("Luck UP\n+5");
                    break;

                case ItemType.projectileSpeedUp:
                    player.projectileSpeed += 25;
                    textSlide.ShowItemName("Shot speed UP\n+25");
                    break;

                case ItemType.damageUp: //
                    player.projectileSpeed += 25;
                    textSlide.ShowItemName("Shot speed UP\n+25");
                    break;

                case ItemType.moneyPower: //
                    // player.damage += player.coins;
                    textSlide.ShowItemName("Money = power!");
                    break;
            }
            AddItemToUI();
            PlaySoundAndDie();
        }
    }

    private void ApplyAllStatsUp()
    {
        player.damage += 1;
        player.maxHealth++;
        player.maxSpeed += 5;
        player.turnSpeed += 10;
        if (player.fireRate > .1f)
            player.fireRate *= .9f;
        player.projectileSpeed += 25;
        player.health++;
        textSlide.ShowItemName("All stats UP");

        if (PlayerPrefs.GetInt("Unlockable3") == 0)
        {
            PlayerPrefs.SetInt("Unlockable3", 1);
            textSlide.ShowItemName("New Tank Unlocked\nfound legendary");
        }
    }

    private void AddItemToUI()
    {
        if (itemIconSprite != null && itemsPanel != null)
        {
            // Создаем новый UI элемент Image
            GameObject newItemIcon = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

            // Устанавливаем спрайт для иконки
            Image itemImage = newItemIcon.GetComponent<Image>();
            itemImage.sprite = itemIconSprite;

            // Привязываем иконку к панели
            newItemIcon.transform.SetParent(itemsPanel, false);

            // Добавляем в список собранных предметов
            collectedItems.Add(itemIconSprite);
        }
    }

    public void PlaySoundAndDie()
    {
        if (coinPickupSoundPrefab != null)
        {
            GameObject coinPickupSound = Instantiate(coinPickupSoundPrefab, transform.position, transform.rotation);
            Destroy(coinPickupSound, 3f);
        }
        Destroy(gameObject);
    }


    void Move()
    {
        // Плавное перемещение по оси Y
        transform.Translate(Vector3.up * (isMovingDown ? -moveSpeed : moveSpeed) * Time.deltaTime);

        // Проверка на достижение минимальной/максимальной позиции по оси Y
        if (isMovingDown && transform.position.y <= minY + 5)
        {
            isMovingDown = false;
        }
        else if (!isMovingDown && transform.position.y >= maxY)
        {
            isMovingDown = true;
        }

        // Плавное вращение вокруг оси Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
        public enum ItemType
    {
        allStatsUp,
        healthUp,
        speedUp,
        fireRateUp,
        projectileSpeedUp,
        luckUp,
        moneyPower,
        damageUp
    }
}
