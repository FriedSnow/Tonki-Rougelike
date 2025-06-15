using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
public class TankController : MonoBehaviour
{
    // ---------- ---------- STATS ---------- ----------
    [Header("Статы")]
    public int damage = 10;
    public int maxHealth = 50;
    public float maxSpeed = 40f;
    public float fireRate = .5f;
    public float projectileSpeed = 100f;
    public int luck = 0;
    public int armor = 0;
    // ---------- ---------- ---------- ---------- ---------- 
    [Header("Почти статы")]
    public int addedDamage = 0;
    public int baseDamage = 0;
    public int health = 50;
    public int coins = 0;
    public int maxCoins = 99;
    public bool canTakeHealthUp = true;
    public bool canTakeArmor = true;
    // ---------- ---------- ---------- ---------- ---------- 
    [Header("Префабы снарядов")]
    public GameObject primaryBulletPrefab;
    public GameObject secondaryBulletPrefab;
    public GameObject baseBulletPrefab;
    public GameObject startItem;
    // ---------- ---------- ---------- ---------- ---------- 
    [Header("Инвентарь (зачем то)")]
    public List<string> Inventory = new List<string>();
    public List<IOnHitEffect> OnHitEffects = new List<IOnHitEffect>();
    // ---------- ---------- ---------- ---------- ---------- 
    [Header("Остальное")]
    public Texture2D cursorTexture;
    public Transform turret;
    public Transform firePoint;
    public Transform[] firePoints;
    public GameObject hitSoundPrefab;
    public GameObject hitPartsPrefab;
    public GameObject shootParticlesPrefab;
    public TMP_Text healthText;
    public TMP_Text coinsText;
    public TMP_Text firerateText;
    public TMP_Text maxSpeedText;
    public TMP_Text luckText;
    public TMP_Text projectileSpeedText;
    public TMP_Text armorText;
    public TMP_Text damageText;
    public float invincibilityDuration = .5f; // длительность неуязвимости (1 секунда)
    public float rotationSpeed = 10f;
    public float acceleration = 100f;
    public float turnSpeed = 150f;
    [Header("Post Processing")]
    public PostProcessVolume postProcessVolume; // Присвойте ссылку на PostProcessVolume
    private ChromaticAberration chromaticAberration;
    // ---------- ---------- PRIVATE ---------- ---------- 
    private Rigidbody trb;
    private TextSlide textSlide;
    private HealthManager healthManager;
    private float currentSpeed = 0;
    private float nextFireTime = 0f;
    private bool isDestroyed = false;
    private bool isInvincible = false; // флаг для неуязвимости
    private bool isGamepadConnected = false;

    private void Start()
    {
        healthManager = FindObjectOfType<HealthManager>();
        textSlide = FindObjectOfType<TextSlide>();
        trb = GetComponent<Rigidbody>();
        baseBulletPrefab = primaryBulletPrefab;
        baseDamage = damage;
        GetUI();
        trb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (secondaryBulletPrefab == null) secondaryBulletPrefab = primaryBulletPrefab;
        EnableCursor();
        // Автоматически ищем PostProcessVolume в сцене
        postProcessVolume = FindObjectOfType<PostProcessVolume>();

        if (postProcessVolume != null && postProcessVolume.profile.TryGetSettings(out ChromaticAberration ca))
        {
            chromaticAberration = ca;
            chromaticAberration.active = true; // Убедимся, что эффект активен
        }
        else
            UnityEngine.Debug.LogWarning("PostProcessVolume с эффектом Chromatic Aberration не найден!");
            
        if (startItem != null)
            Instantiate(startItem, transform.position + new Vector3(10, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        Debug();
        CheckGamepadConnection();
        if (!isDestroyed)
        {
            Move();
            Aim();
            Shoot(damage);
        }
        CheckUnlockForCoins();
        UpdateUI();
        damage = baseDamage + addedDamage;
    }
    private void CheckGamepadConnection()
    {
        isGamepadConnected = Input.GetJoystickNames().Length > 0 && !string.IsNullOrEmpty(Input.GetJoystickNames()[0]);
    }
    void Aim()
    {
        Vector3 direction;

        if (isGamepadConnected)
        {
            float aimHorizontal = Input.GetAxis("RightStickHorizontal");
            float aimVertical = Input.GetAxis("RightStickVertical");
            direction = new Vector3(aimHorizontal, 0, aimVertical);
        }
        else
        {
            direction = GetMouseDirection();
            direction.y = 0;
        }

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            turret.rotation = Quaternion.Lerp(turret.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    public void Shoot(int damage)
    {
        float leftTrigger = Input.GetAxis("LeftTrigger");
        float rightTrigger = Input.GetAxis("RightTrigger");

        // Проверяем, нажата ли кнопка для стрельбы и прошло ли достаточно времени с последнего выстрела
        if ((Input.GetButton("Fire1") || rightTrigger > 0.1f || leftTrigger > 0.1f) && Time.time >= nextFireTime)
        {
            // Если firePoints содержит элементы, используем их для стрельбы
            if (firePoints != null && firePoints.Length > 0)
            {
                foreach (Transform firePoint in firePoints)
                {
                    // Создаем пулю в каждой точке стрельбы
                    GameObject bullet = Instantiate(primaryBulletPrefab, firePoint.position, firePoint.rotation);
                    bullet.layer = 12; //PlayerBullet
                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = firePoint.forward * projectileSpeed;
                    }

                    // Настройка урона для пули
                    StandardAmmo ammo = bullet.GetComponent<StandardAmmo>();
                    if (ammo != null)
                    {
                        ammo.damage = damage;
                    }

                    // Создание частиц при стрельбе
                    if (shootParticlesPrefab != null)
                    {
                        GameObject shootParticles = Instantiate(shootParticlesPrefab, firePoint.position, firePoint.rotation);
                        Destroy(shootParticles, 2f); // Уничтожаем частицы через 2 секунды
                    }
                    // trb.AddForce(-firePoint.forward * 10000, ForceMode.Impulse);
                }
            }
            // Если firePoints пустой или null, используем firePoint для стрельбы
            else if (firePoint != null)
            {
                // Создаем пулю в точке firePoint
                GameObject bullet = Instantiate(primaryBulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = firePoint.forward * projectileSpeed;
                }

                // Настройка урона для пули
                StandardAmmo ammo = bullet.GetComponent<StandardAmmo>();
                if (ammo != null)
                {
                    ammo.damage = damage;
                }

                // Создание частиц при стрельбе
                if (shootParticlesPrefab != null)
                {
                    GameObject shootParticles = Instantiate(shootParticlesPrefab, firePoint.position, firePoint.rotation);
                    Destroy(shootParticles, 2f); // Уничтожаем частицы через 2 секунды
                }
            }

            // Обновляем время следующего выстрела
            nextFireTime = Time.time + fireRate;
        }
    }
    private Vector3 GetMouseDirection()
    {
        Plane plane = new Plane(Vector3.up, 0);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
            return ray.GetPoint(distance) - turret.position; // Разница между положением башни и целевой позицией
        return Vector3.zero;
    }
    public void Move()
    {
        float moveY = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        if (moveY != 0)
        {
            currentSpeed += moveY * acceleration * Time.deltaTime;
        }
        else
        {
            if (Mathf.Abs(currentSpeed) > 0.01f)
                currentSpeed -= currentSpeed * acceleration * 0.05f * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        if (moveX != 0)
        {
            float loss = Mathf.Abs(currentSpeed) * 0.1f * Time.deltaTime;
            currentSpeed -= loss * Mathf.Sign(currentSpeed);
        }

        Vector3 moveDirection = transform.forward * currentSpeed;
        trb.velocity = new Vector3(moveDirection.x, trb.velocity.y, moveDirection.z);

        if (moveX != 0)
        {
            float turn = moveX * turnSpeed * Time.deltaTime;
            trb.MoveRotation(trb.rotation * Quaternion.Euler(0, turn, 0));
        }
    }
    public void TakeDamage(int damage)
    {
        int _damage = PlayerPrefs.GetInt("MonkeyModeEnabled", 0) == 1 ? 1 : damage;
        damage = _damage;
        // Проверяем неуязвимость и состояние объекта 
        if (!isDestroyed && !isInvincible)
        {
            // damage *= damageMultiplier;
            if (armor > 0)
                armor -= damage;
            else
                health -= damage;

            if (health <= 0 && !isDestroyed)
                Die();

            TankDamage.SpawnHitParticles(hitPartsPrefab, transform, 10);
            GameObject hitSound = Instantiate(hitSoundPrefab, transform.position, transform.rotation);
            Destroy(hitSound, 5f);

            // Обновляем здоровье в интерфейсе через HealthManager
            healthManager.SetCurrentHealth(health);
            healthManager.SetCurrentArmor(armor);

            // Активируем неуязвимость после удара
            StartCoroutine(BecomeInvincible());

            // *** Применяем эффект хроматической аберрации ***
            if (chromaticAberration != null)
            {
                StartCoroutine(ApplyChromaticAberrationEffect());
            }

            // Активируем все эффекты
            foreach (var effect in OnHitEffects)
            {
                ((IOnHitEffect)effect).ApplyEffect(this);
            }
        }
    }

    // Корутина для применения эффекта хроматической аберрации
    private IEnumerator ApplyChromaticAberrationEffect()
    {
        float duration = 0.25f; // Время изменения
        float halfDuration = duration / 2;

        // Сохраняем исходное значение интенсивности
        float originalIntensity = chromaticAberration.intensity.value;

        // Плавно увеличиваем интенсивность до 1
        for (float t = 0f; t < halfDuration; t += Time.deltaTime)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(originalIntensity, .25f, t / halfDuration);
            yield return null;
        }

        // Максимальная интенсивность
        chromaticAberration.intensity.value = .25f;

        // Плавно уменьшаем интенсивность обратно до исходного значения
        for (float t = 0f; t < halfDuration; t += Time.deltaTime)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(.25f, originalIntensity, t / halfDuration);
            yield return null;
        }

        // Возвращаем к исходному значению
        chromaticAberration.intensity.value = originalIntensity;
    }
    public void Heal(int amount)
    {
        if (health < maxHealth)
        {
            health += amount;
            healthManager.SetCurrentHealth(health);
        }
    }
    public void AddArmor(int amount)
    {
        if (armor < maxHealth && canTakeArmor)
        {
            armor += amount;
            healthManager.SetCurrentArmor(armor);
        }
    }
    public void HealthUp(int amount)
    {
        if (canTakeHealthUp)
            maxHealth += amount;
    }
    public void FullHealth()
    {
        health = maxHealth;
    }
    public int CheckLuck(int win, int lose)
    {
        // Генерируем случайное число от 0 до 100
        int rnd = Random.Range(0, 101);

        // Вычисляем шанс успеха как процент от player.luck
        // Предполагается, что player.luck находится в диапазоне от 0 до 100
        if (rnd < luck)
        {
            return win; // Успех! Возвращаем 2
        }

        return lose; // Неудача, возвращаем 0
    }

    private IEnumerator BecomeInvincible()
    {
        isInvincible = true; // Включаем неуязвимость
        yield return new WaitForSeconds(invincibilityDuration); // Ждём заданное время
        isInvincible = false; // Выключаем неуязвимость
    }

    void Die()
    {
        if (!isDestroyed)
        {
            UnityEngine.Debug.Log("ПОМЕР");
            StartCoroutine(GoToMainMenu(3f));
            isDestroyed = true;
        }
        Progression.IncrementProgression(2);
        if (PlayerPrefs.GetInt("Unlockable5") == 0 && Progression.GetProgression(2) >= 10)
        {
            PlayerPrefs.SetInt("Unlockable5", 1);
            textSlide.ShowItemName("Достижение получено!", Color.cyan);
        }
    }

    IEnumerator GoToMainMenu(float delay)
    {
        textSlide.ShowItemName("ПОМЕР", Color.red, "Skill Issue", Color.gray);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateUI()
    {
        healthText.text = $"{health / 10}/{maxHealth / 10}";
        coinsText.text = coins.ToString();
        maxSpeedText.text = maxSpeed.ToString();
        firerateText.text = Math.Round(1f / fireRate, 2).ToString();
        luckText.text = luck.ToString();
        projectileSpeedText.text = projectileSpeed.ToString();
        armorText.text = (armor / 10).ToString();
        damageText.text = damage.ToString();
    }

    void CheckUnlockForCoins()
    {
        if (coins >= 15 && PlayerPrefs.GetInt("Unlockable2") == 0)
        {
            PlayerPrefs.SetInt("Unlockable2", 1);
            textSlide.ShowItemName("Достижение получено!", Color.cyan);
        }
    }
    public void GetUI()
    {
        GameObject textHealthObject = GameObject.Find("HealthText");
        if (textHealthObject != null)
        {
            healthText = textHealthObject.GetComponent<TMP_Text>();
        }
        GameObject textCoinObject = GameObject.Find("CoinText");
        if (textCoinObject != null)
        {
            coinsText = textCoinObject.GetComponent<TMP_Text>();
        }
        GameObject textMaxSpeedObject = GameObject.Find("SpeedText");
        if (textMaxSpeedObject != null)
        {
            maxSpeedText = textMaxSpeedObject.GetComponent<TMP_Text>();
        }
        GameObject textFirerateObject = GameObject.Find("FirerateText");
        if (textFirerateObject != null)
        {
            firerateText = textFirerateObject.GetComponent<TMP_Text>();
        }
        GameObject textLuckObject = GameObject.Find("LuckText");
        if (textLuckObject != null)
        {
            luckText = textLuckObject.GetComponent<TMP_Text>();
        }
        GameObject textProjectileSpeedObject = GameObject.Find("ProjectilespeedText");
        if (textProjectileSpeedObject != null)
        {
            projectileSpeedText = textProjectileSpeedObject.GetComponent<TMP_Text>();
        }
        GameObject textArmorObject = GameObject.Find("ArmorText");
        if (textArmorObject != null)
        {
            armorText = textArmorObject.GetComponent<TMP_Text>();
        }
        GameObject textDamageObject = GameObject.Find("DamageText");
        if (textDamageObject != null)
        {
            damageText = textDamageObject.GetComponent<TMP_Text>();
        }
    }
    private void OnDestroy()
    {
        Cursor.SetCursor(null, new Vector2(32, 32), CursorMode.Auto);
    }
    private void CheckForAngle()
    {
        if (gameObject.transform.rotation.x >= 1f || gameObject.transform.rotation.z >= 1f) transform.Rotate(new Vector3(0, transform.rotation.y, 0));
    }
    public void EnableCursor(bool enabled = true)
    {
        if (enabled)
            Cursor.SetCursor(cursorTexture, new Vector2(32, 32), CursorMode.Auto);
        else
            Cursor.SetCursor(null, new Vector2(32, 32), CursorMode.Auto);
    }
    bool isZawarudo = false;
    public void Debug()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (!isZawarudo)
            {
                Time.timeScale = .1f;
                UnityEngine.Debug.Log("таймстоп");
                isZawarudo = true;
            }
            else
            {
                UnityEngine.Debug.Log("нот таймстоп");
                Time.timeScale = 1;
                isZawarudo = false;
            }
        }
    }
    public interface IOnHitEffect
    {
        void ApplyEffect(TankController player); // Метод, который срабатывает при получении урона
    }
}