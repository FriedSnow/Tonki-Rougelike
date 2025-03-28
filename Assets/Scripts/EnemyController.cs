using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyType enemyType = EnemyType.regular;
    public int health = 3;
    public GameObject[] dropPickupPool; // Подобранные предметы
    public GameObject turret; // Ссылка на башню танка
    public GameObject bulletPrefab; // Пуля, которую будет стрелять враг
    public GameObject hitSoundPrefab;
    public GameObject teleportPrefab;
    public GameObject shootParticlesPrefab;
    public GameObject destroyParticlesPrefab;
    public GameObject hitPartsPrefab;
    public Transform firePoint; // Точка стрельбы
    public float turretRotationSpeed = 5f; // Скорость поворота башни
    public float moveSpeed = 3f; // Скорость движения
    public float bodyRotationSpeed = 90f; // Скорость вращения корпуса танка
    public float decisionTime = 2f; // Время между изменениями направления
    public float fireRate = 1f; // Задержка между выстрелами
    public float projectileSpeed = 10f; // Скорость полета снаряда
    private float nextFireTime = 0f; // Время до следующего выстрела
    private bool isDestroyed = false;
    private TankController player;
    private Rigidbody rb;
    private float decisionTimer;
    private Quaternion targetRotationBody; // Целевой поворот корпуса
    private TextSlide textSlide;
    private BossHealth bossHealth;
    private ItemSpawner itemSpawner; // Экземпляр класса ItemSpawner

    void Start()
    {
        bossHealth = FindObjectOfType<BossHealth>();
        textSlide = FindObjectOfType<TextSlide>();
        player = FindObjectOfType<TankController>();
        rb = GetComponent<Rigidbody>();
        decisionTimer = decisionTime;
        // Создаем экземпляр ItemSpawner и загружаем предметы
        itemSpawner = new ItemSpawner();
        itemSpawner.LoadDropItems();
        // Начальное значение целевого поворота корпуса
        targetRotationBody = transform.rotation;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (bossHealth != null)
        {
            switch (enemyType)
            {
                case EnemyType.firstBoss:
                    bossHealth.ShowHealthBar(health, "Maus");
                    moveSpeed *= .25f;
                    bodyRotationSpeed *= .25f;
                    fireRate *= 2;
                    turretRotationSpeed *= .5f;
                    decisionTime *= 3;
                    projectileSpeed *= 1.5f;
                    textSlide.ShowItemName("Maus", 3f, "Холодильник");
                    break;

                case EnemyType.secondBoss:
                    bossHealth.ShowHealthBar(health, "2");
                    moveSpeed *= .25f;
                    bodyRotationSpeed *= .25f;
                    turretRotationSpeed *= .5f;
                    decisionTime *= 3;
                    projectileSpeed *= 1.5f;
                    textSlide.ShowItemName("второй чел", 3f, "второй");
                    break;

                case EnemyType.thirdBoss:
                    bossHealth.ShowHealthBar(health, "3");
                    decisionTime *= 3;
                    projectileSpeed *= 1.5f;
                    textSlide.ShowItemName("третий чел", 3f, "опасный я хз");
                    break;
            }
        }
        transform.rotation = Quaternion.Euler(transform.rotation.x, Random.Range(0, 5) * 90f, transform.rotation.z);
    }

    void Update()
    {
        RotateTurretTowardsPlayer();
        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0)
        {
            DecideMovement();
            decisionTimer = decisionTime; // Сброс таймера
        }
        Move();
        ShootAtPlayer();
    }

    void RotateTurretTowardsPlayer()
    {
        if (player == null) return;
        Vector3 turretDirectionToPlayer = player.transform.position - turret.transform.position;
        turretDirectionToPlayer.y = 0; // Игнорируем разницу по высоте
        Quaternion targetRotationTurret = Quaternion.LookRotation(turretDirectionToPlayer);
        turret.transform.rotation = Quaternion.RotateTowards(
            turret.transform.rotation,
            targetRotationTurret,
            turretRotationSpeed * Time.deltaTime
        );
    }

    void DecideMovement()
    {
        float randomRotation = Random.Range(-150f, 150f);
        targetRotationBody = Quaternion.Euler(0, randomRotation, 0) * transform.rotation;
    }

    void Move()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotationBody,
            bodyRotationSpeed * Time.deltaTime
        );
        Vector3 moveDirection = transform.forward * moveSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }

    void ShootAtPlayer()
    {
        if (Time.time >= nextFireTime)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.layer = 14; //EnemyBullet
            Rigidbody rbBullet = bullet.GetComponent<Rigidbody>();
            rbBullet.velocity = firePoint.forward * projectileSpeed;
            if (shootParticlesPrefab != null)
            {
                GameObject shootParticles = Instantiate(shootParticlesPrefab, firePoint.position, firePoint.rotation);
                Destroy(shootParticles, 2f);
            }
            nextFireTime = Time.time + fireRate;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (enemyType != EnemyType.regular && bossHealth != null)
        {
            bossHealth.UpdateHealthBar(health);
        }
        if (health <= 0 && !isDestroyed)
        {
            Die();
        }
        // TankDamage.SpawnHitParticles(hitPartsPrefab, transform, 10);
        GameObject hitSound = Instantiate(hitSoundPrefab, transform.position, transform.rotation);
        Destroy(hitSound, 5f);
    }

    void Die()
    {
        TankDamage.SpawnHitParticles(hitPartsPrefab, transform, enemyType == EnemyType.regular ? 15 : 50, 50);
        if (enemyType != EnemyType.regular && bossHealth != null)
        {
            Instantiate(teleportPrefab, transform.position, Quaternion.identity);
            bossHealth.HideHealthBar();
            Progression.IncrementProgression(0);
        }
        if (enemyType == EnemyType.regular)
        {
            Progression.IncrementProgression(1);
        }
        // Debug.Log("ПОМЕР но не я");
        if (destroyParticlesPrefab != null)
        {
            GameObject destroyParticles = Instantiate(destroyParticlesPrefab, transform.position + new Vector3(0, 2, 0), new Quaternion(0, Random.Range(0, 90), 0, Random.Range(0, 90)));
            Destroy(destroyParticles, 3f);
        }
        isDestroyed = true;
        Destroy(gameObject, .1f);

        // Логика для спавна предметов
        SpawnLoot();

        if (enemyType == EnemyType.firstBoss)
        {
            if (PlayerPrefs.GetInt("Unlockable4") == 0)
            {
                PlayerPrefs.SetInt("Unlockable4", 1);
                textSlide.ShowItemName("Достижение получено", Color.cyan);
            }
        }
    }

    private void SpawnLoot()
    {
        float rnd = Random.Range(0, 101);// - player.luck;
        Debug.Log($"Число генерации - {rnd}");
        int rarityLevel = GetRarityLevel(rnd);
        // Debug.Log($"Определенная редкость - {rarityLevel}");

        if (enemyType == EnemyType.regular)     //regular enemy loot 
        {
            if (rarityLevel == 0)
            {
                itemSpawner.SpawnItem(0, transform.position);
            }
            else if (rarityLevel == 1)
            {
                itemSpawner.SpawnItem(1, transform.position);
            }
            else if (rarityLevel == 2)
            {
                itemSpawner.SpawnItem(2, transform.position);
            }
            else
            {
                if (dropPickupPool != null && dropPickupPool.Length > 0)
                    Instantiate(dropPickupPool[Random.Range(0, dropPickupPool.Length)], transform.position, Quaternion.identity);
            }
        }
        else                                    //boss enemy loot
        {
            if (rarityLevel == 2 || rarityLevel == 1 || rarityLevel == 0)
            {
                itemSpawner.SpawnItem(2, transform.position);
            }
            else if (rarityLevel == 3)
            {
                itemSpawner.SpawnItem(3, transform.position);
            }
            else
            {
                Debug.LogWarning("Неожиданный уровень редкости для босса.");
            }
        }
    }

    private int GetRarityLevel(float rnd)
    {
        if (rnd < 10)
            return 2; // Качество 2
        else if (rnd < 50)
            return 1; // Качество 1
                      //сюда
        else
            return 3; // Не выпадает из врагов
    }

    public enum EnemyType
    {
        regular,
        firstBoss,
        secondBoss,
        thirdBoss
    }
}