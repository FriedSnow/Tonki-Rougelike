using System.Collections;
using UnityEngine;

public class FragmentingAmmo : StandardAmmo
{
    public int fragmentCount = 3;
    public GameObject fragmentPrefab;
    public float fragmentSpeed = 5f;
    public float fragmentLifetime = 5f;
    public Vector3 offset;
    public bool isBoss = false;
    TankController player;
    private void Start()
    {
        player = FindObjectOfType<TankController>();
    }

    private void SpawnFragments()
    {
        fragmentCount += player.CheckLuck(2, 0);
        
        float angleStep = 360f / fragmentCount; // Угол между фрагментами
        Vector3 currentDirection = transform.forward; // Текущее направление снаряда
        float distance = 1.0f; // Расстояние от центра попадания до фрагмента

        if (!isBoss)
            fragmentPrefab = player.secondaryBulletPrefab;

        for (int i = 0; i < fragmentCount; i++)
        {
            // Рассчитываем направление для каждого фрагмента
            Quaternion rotation = Quaternion.Euler(0, angleStep * i, 0); // Поворот вокруг оси Y
            Vector3 fragmentDirection = rotation * currentDirection; // Направление фрагмента

            // Создаём фрагмент с нужной ориентацией
            Quaternion fragmentRotation = Quaternion.LookRotation(fragmentDirection);
            GameObject fragment = Instantiate(fragmentPrefab, transform.position, fragmentRotation);

            fragment.layer = 6; // Устанавливаем нужный слой для фрагмента

            // Устанавливаем позицию фрагмента с учётом смещения
            Vector3 spawnPosition = transform.position + fragmentDirection.normalized * distance + offset;
            fragment.transform.position = spawnPosition;

            // Устанавливаем скорость фрагмента
            Rigidbody fragmentRb = fragment.GetComponent<Rigidbody>();
            if (fragmentRb != null)
            {
                fragmentRb.velocity = fragmentDirection.normalized * fragmentSpeed; // Движение в направлении
            }
            else
            {
                Debug.LogError($"Фрагмент {i} не имеет компонента Rigidbody!");
            }

            // Уничтожаем фрагмент через заданное время
            Destroy(fragment, fragmentLifetime);
        }
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

            SpawnFragments();
        }
    }
}