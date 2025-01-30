using UnityEngine;

public class TankDamage : MonoBehaviour
{
    public static void SpawnHitParticles(GameObject gearPrefab, Transform tankTransform, int numberOfGears = 5)
    {
        for (int i = 0; i < numberOfGears; i++)
        {
            // Создаем экземпляр шестеренки
            GameObject gear = Instantiate(gearPrefab, tankTransform.position, Quaternion.identity);

            // Генерируем случайное направление вверх
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f));
            randomDirection.Normalize();

            // Генерируем случайную скорость
            float randomSpeed = Random.Range(10, 100);

            // Добавляем силу, чтобы шестеренка вылетела из танка
            Rigidbody rb = gear.GetComponent<Rigidbody>();
            rb.AddForce(randomDirection * randomSpeed, ForceMode.VelocityChange);

            // Генерируем случайные углы поворота
            float randomAngleX = Random.Range(-180f, 180f);
            float randomAngleY = Random.Range(-180f, 180f);
            float randomAngleZ = Random.Range(-180f, 180f);
            gear.transform.Rotate(new Vector3(randomAngleX, randomAngleY, randomAngleZ));

            Destroy(gear, 1f);
        }
    }
}