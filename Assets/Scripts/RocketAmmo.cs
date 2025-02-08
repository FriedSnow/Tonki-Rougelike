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
        speed += acceleration * Time.deltaTime;

        // Используем MovePosition для более стабильного движения
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHit && collision.collider.gameObject.layer != LayerMask.NameToLayer("IgnoreCollision"))
        {
            if (player != null && explosionPatricles != null)
            {
                DamageManager.DealDamage(collision, player.damage);
                GameObject explosion = Instantiate(explosionPatricles, transform.position, transform.rotation);
                if (explosion != null)
                {
                    Destroy(explosion, 5f);
                }
            }
            Destroy(gameObject);
            isHit = true;
        }
    }
}