using System.Collections.Generic;
using UnityEngine;

public class ExplosiveAmmo : StandardAmmo
{
    public float explosionRadius = 5f; // Радиус взрыва
    public GameObject explosionSpherePrefab; // Префаб сферы взрыва

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHit)
        {
            // Наносим урон цели, которая была поражена
            DamageManager.DealDamage(collision, damage);

            // Создаем эффект взрыва
            GameObject explosion = Instantiate(explosionPatricles, transform.position, transform.rotation);
            Destroy(explosion, 5f);

            // Уничтожаем текущий снаряд
            Destroy(gameObject);

            // Помечаем, что снаряд уже попал
            isHit = true;

            // Обрабатываем взрывное повреждение
            Explode();
        }
    }

    private void Explode()
    {
        // Получаем все коллайдеры в радиусе взрыва
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            // Пропускаем сам снаряд
            if (hitCollider.gameObject == gameObject)
                continue;

            // Наносим урон объекту, если это подходящая цель (например, танк или другой объект)
            if (hitCollider.CompareTag("Enemy") || hitCollider.CompareTag("Player") || hitCollider.CompareTag("Breakable"))
            {
                DamageManager.DealDamage(hitCollider, damage, DamageManager.DamageType.explosive);
            }

            // Применяем силу от взрыва к объекту, если у него есть Rigidbody
            Rigidbody targetRigidbody = hitCollider.GetComponent<Rigidbody>();
            if (targetRigidbody != null)
            {
                Vector3 explosionDirection = hitCollider.transform.position - transform.position;
                float distance = explosionDirection.magnitude;
                float forceMagnitude = Mathf.Clamp(10f * (1 - distance / explosionRadius), 0f, 10f); // Уменьшение силы с увеличением расстояния
                targetRigidbody.AddExplosionForce(forceMagnitude, transform.position, explosionRadius);
            }
        }

        // Создаем и отображаем сферу взрыва
        ShowExplosionSphere();

        // Визуализация радиуса взрыва для дебага
        DebugExplosionRadius();
    }

    private void ShowExplosionSphere()
    {
        if (explosionSpherePrefab != null)
        {
            GameObject explosionSphere = Instantiate(explosionSpherePrefab, transform.position, Quaternion.identity);
            SphereCollider sphereCollider = explosionSphere.GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                sphereCollider.radius = explosionRadius;
            }

            // Устанавливаем срок жизни сферы взрыва
            Destroy(explosionSphere, 2f);
        }
    }

    private void DebugExplosionRadius()
    {
        // Определяем количество точек для отрисовки окружности
        int numberOfPoints = 50;
        float angleStep = 360f / numberOfPoints;

        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 pointOnCircle = new Vector3(Mathf.Cos(angle) * explosionRadius, 0, Mathf.Sin(angle) * explosionRadius);
            pointOnCircle += transform.position;

            // Отрисовка линии от центра взрыва до точки на окружности
            Debug.DrawLine(transform.position, pointOnCircle, Color.red, 2f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}