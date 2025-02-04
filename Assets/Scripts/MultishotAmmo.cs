using System.Collections;
using UnityEngine;

public class MultishotAmmo : StandardAmmo
{
    public int projectileCount = 5;          // Количество снарядов
    public GameObject projectilePrefab;      // Префаб снаряда
    public float projectileSpeed = 100f;     // Скорость снарядов
    public float projectileLifetime = 5f;    // Время жизни снарядов
    public float spacing = 2f;               // Расстояние между снарядами
    public float backwardFactor = .75f;      // Отступ назад         
    TankController player;

    private void Start()
    {
        player = FindObjectOfType<TankController>();
        // Вызываем метод для выпуска снарядов
        LaunchProjectilesInLine();
        damage = player.damage;
        projectileSpeed = player.projectileSpeed;
        projectilePrefab = player.secondaryBulletPrefab;
    }

    // Метод для выпуска снарядов в виде стрелочки
    private void LaunchProjectilesInLine()
    {
        projectileCount += player.CheckLuck(2,0);
        Vector3 mainDirection = transform.forward;
        Vector3 startPosition = transform.position;
        float totalWidth = (projectileCount - 1) * spacing;  // Полная ширина ряда
        float halfWidth = totalWidth / 2f;  // Половина ширины для смещения

        for (int i = 0; i < projectileCount; i++)
        {
            // Вычисляем смещение по горизонтали
            float horizontalOffset = i * spacing - halfWidth;
            // Вычисляем смещение назад в зависимости от расстояния до центра
            float backwardOffset = Mathf.Abs(horizontalOffset) * backwardFactor;
            // Вычисляем финальную позицию спавна снаряда
            Vector3 spawnPosition = startPosition + transform.right * horizontalOffset - transform.forward * backwardOffset;

            // Запускаем снаряд с нужным направлением
            LaunchProjectile(spawnPosition, mainDirection);
        }
    }

    private void LaunchProjectile(Vector3 position, Vector3 direction)
    {
        projectilePrefab = player.secondaryBulletPrefab;
        // Создаем снаряд
        GameObject projectile = Instantiate(projectilePrefab, position, Quaternion.LookRotation(direction));
        projectile.layer = 6;

        // Устанавливаем направление и скорость
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            projectileSpeed = player.projectileSpeed;
            rb.velocity = direction.normalized * projectileSpeed;
        }
        // Уничтожаем снаряд через заданное время
        Destroy(projectile, projectileLifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHit)
        {
            DamageManager.DealDamage(collision, damage);
            GameObject explosion = Instantiate(explosionPatricles, transform.position, transform.rotation);
            Destroy(explosion, 5f);
            Destroy(gameObject);
            isHit = true;
        }
    }

}