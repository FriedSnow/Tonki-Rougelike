using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    // TextSlide textSlide;
    public enum DamageType
    {
        regular,
        explosive
    }
    private void Start()
    {
        // textSlide = FindObjectOfType<TextSlide>();
    }
    /// <summary>
    /// Метод нанесения урона по collision
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="damage"></param>
    /// <param name="damageType"></param>
    public static void DealDamage(Collision collision, int damage, DamageType damageType = DamageType.regular)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TankController player = collision.collider.GetComponent<TankController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        if (collision.collider.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        if (collision.collider.CompareTag("Breakable"))
        {
            BreakableObject breakable = collision.collider.GetComponent<BreakableObject>();
            if (breakable != null)
            {
                breakable.TakeDamage(damage);
            }
        }
    }
    /// <summary>
    /// Метод нанесения урона по collider
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="damage"></param>
    /// <param name="damageType"></param>
    public static void DealDamage(Collider collider, int damage, DamageType damageType = DamageType.regular)
    {
        if (collider.CompareTag("Player"))
        {
            TankController player = collider.GetComponent<TankController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        if (collider.CompareTag("Enemy"))
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        if (collider.CompareTag("Breakable"))
        {
            BreakableObject breakable = collider.GetComponent<BreakableObject>();
            if (breakable != null)
            {
                if (damageType == DamageType.explosive)
                    breakable.TakeDamage(damage * 2);
                else
                    breakable.TakeDamage(damage);
            }
        }
    }
}
