using UnityEngine;
using static TankController;

public class MultiShotOnHitEffect : IOnHitEffect
{
    private GameObject bulletPrefab; // Префаб снаряда
    private float bulletSpeed;       // Скорость снарядов
    private int numBullets = 10;     // Количество снарядов
    private float spawnOffset = 5f;  // Смещение точки спавна от игрока

    public MultiShotOnHitEffect(GameObject bulletPrefab, float bulletSpeed, float spawnOffset = 1f)
    {
        this.bulletPrefab = bulletPrefab;
        this.bulletSpeed = bulletSpeed;
        this.spawnOffset = spawnOffset;
    }

    public void ApplyEffect(TankController player)
    {
        Debug.Log("Multi-shot effect triggered!");

        // Выпускаем снаряды вокруг игрока
        float angleStep = 360f / numBullets; // Угол между снарядами
        float angle = 0f;

        for (int i = 0; i < numBullets; i++)
        {
            // Рассчитываем направление для каждого снаряда
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirZ = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 bulletDirection = new Vector3(bulletDirX, 0, bulletDirZ).normalized;

            // Рассчитываем точку спавна снаряда (с учетом смещения)
            Vector3 spawnPosition = player.transform.position + bulletDirection * spawnOffset;

            // Создаем снаряд
            GameObject bullet = GameObject.Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                bulletRb.velocity = bulletDirection * bulletSpeed;
            }

            // Поворачиваем снаряд в направлении движения (опционально)
            bullet.transform.rotation = Quaternion.LookRotation(bulletDirection);

            // Увеличиваем угол для следующего снаряда
            angle += angleStep;
        }
    }
}