using System.Collections;
using UnityEngine;

public class CumulativeAmmo : StandardAmmo
{
    public GameObject fragmentPrefab;
    public float fragmentSpeed = 5f;
    public float fragmentLifetime = 5f;
    public Vector3 vOffset;
    public float distanceToSpawn = 2f; // Установите желаемое расстояние
    TankController player;
    private void Start()
    {
        player = FindObjectOfType<TankController>();
        fragmentSpeed = player.projectileSpeed;
    }

    private void SpawnFragments()
    {
        fragmentPrefab = player.secondaryBulletPrefab;

        // Определяем позицию спавна фрагмента на заданном расстоянии вперед
        Vector3 spawnPosition = transform.position + transform.forward * distanceToSpawn + vOffset;

        GameObject fragment = Instantiate(fragmentPrefab, spawnPosition, transform.rotation);
        Debug.Log($"Фрагмент создан: {fragment != null}");
        fragment.layer = 6;

        Rigidbody fragmentRb = fragment.GetComponent<Rigidbody>();
        if (fragmentRb != null)
        {
            fragmentRb.velocity = transform.forward.normalized * fragmentSpeed;
        }
        // Уничтожаем фрагмент через заданное время
        Destroy(fragment, fragmentLifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamageManager.DealDamage(collision, damage);
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