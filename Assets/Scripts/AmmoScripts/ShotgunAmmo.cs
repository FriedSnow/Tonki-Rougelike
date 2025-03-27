using System.Collections;
using UnityEngine;

public class ShotgunAmmo : StandardAmmo
{
    public int fragmentCount = 3; // Количество фрагментов
    public GameObject fragmentPrefab; // Префаб фрагмента
    public float fragmentSpeed = 5f; // Скорость фрагментов
    public float fragmentLifetime = 5f; // Время жизни фрагментов
    public Vector3 offset; // Смещение при спавне фрагментов
    public float detonationDistance = 10f; // Расстояние для взрыва
    public float spreadAngle = 15f; // Максимальный угол отклонения для фрагментов
    private TankController player; // Ссылка на игрока
    private bool hasExploded = false; // Флаг, указывающий на то, что снаряд уже взорвался
    private Collider ammoCollider; // Коллайдер снаряда

    private void Start()
    {
        player = FindObjectOfType<TankController>();
        ammoCollider = GetComponent<Collider>(); // Получаем коллайдер снаряда
        fragmentPrefab = player.secondaryBulletPrefab;
    }

    private void Update()
    {
        // Проверяем расстояние до игрока
        if (player != null && !hasExploded)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer >= detonationDistance)
            {
                Explode();
            }
        }
    }

    // Функция для создания фрагментов
    private void SpawnFragments()
    {
        Vector3 currentDirection = transform.forward; // Текущее направление снаряда
        for (int i = 0; i < fragmentCount; i++)
        {
            // Создаём фрагмент
            GameObject fragment = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
            fragment.layer = 12; // Присваиваем фрагменту нужный слой
            // Рассчитываем случайный угол отклонения для фрагмента
            float randomSpreadX = Random.Range(-spreadAngle, spreadAngle);
            float randomSpreadY = Random.Range(-spreadAngle, spreadAngle);
            Quaternion randomRotation = Quaternion.Euler(randomSpreadX, randomSpreadY, 0);
            Vector3 fragmentDirection = randomRotation * currentDirection;
            // Устанавливаем ориентацию фрагмента
            Quaternion fragmentRotation = Quaternion.LookRotation(fragmentDirection);
            fragment.transform.rotation = fragmentRotation;
            // Устанавливаем позицию фрагмента с учётом смещения
            Vector3 spawnPosition = transform.position + offset;
            fragment.transform.position = spawnPosition;
            // Устанавливаем скорость фрагмента
            Rigidbody fragmentRb = fragment.GetComponent<Rigidbody>();
            float randomSpeedMultiplier = Random.Range(0.5f, 1.5f); // Добавляем случайный множитель скорости
            if (fragmentRb != null)
            {
                fragmentRb.velocity = fragmentDirection.normalized * fragmentSpeed * randomSpeedMultiplier;
            }
            // Игнорируем столкновения между фрагментами и снарядом
            Collider fragmentCollider = fragment.GetComponent<Collider>();
            if (fragmentCollider != null && ammoCollider != null)
            {
                Physics.IgnoreCollision(fragmentCollider, ammoCollider);
            }
            // Уничтожаем фрагмент через заданное время
            Destroy(fragment, fragmentLifetime);
        }
    }

    // Взрыв снаряда
    private void Explode()
    {
        hasExploded = true;
        // Создаем эффект взрыва
        GameObject explosion = Instantiate(explosionPatricles, transform.position, transform.rotation);
        Destroy(explosion, 5f);
        // Спавним фрагменты
        SpawnFragments();
        // Уничтожаем снаряд
        Destroy(gameObject);
    }

    // Обрабатываем столкновение снаряда
    private void OnCollisionEnter(Collision collision)
    {
        DamageManager.DealDamage(collision, damage);
        GameObject explosion = Instantiate(explosionPatricles, transform.position, transform.rotation);
        Destroy(explosion, 5f);
        Destroy(gameObject);
    }
}