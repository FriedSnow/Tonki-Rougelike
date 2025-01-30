using System.Collections;
using UnityEngine;

public class RocketAmmo : StandardAmmo
{
    TankController player;
    Rigidbody rb;
    public float speed = 10f; // начальная скорость ракеты
    public float acceleration = 5f; // ускорение ракеты

    private void Start()
    {
        player = FindObjectOfType<TankController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Увеличиваем скорость ракеты
        speed += acceleration + Time.deltaTime;
        rb.velocity = transform.forward * speed; // Постоянно устанавливаем скорость вперед
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHit)
        {
            DamageManager.DealDamage(collision, player.damage);
            GameObject explosion = Instantiate(explosionPatricles, transform.position, transform.rotation);
            Destroy(explosion, 5f);
            Destroy(gameObject);
            isHit = true;
        }
    }
}