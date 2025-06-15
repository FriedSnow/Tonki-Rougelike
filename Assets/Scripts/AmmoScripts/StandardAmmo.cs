using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardAmmo : MonoBehaviour
{
    public int damage = 1;
    public GameObject explosionPatricles;
    public bool isHit = false;
    TextSlide textSlide;
    void Start()
    {
        textSlide = FindObjectOfType<TextSlide>();
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 10f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            if (PlayerPrefs.GetInt("Unlockable1") == 0)
            {
                PlayerPrefs.SetInt("Unlockable1", 1);
                textSlide.ShowItemName("Достижение получено!", Color.cyan);
            }
        }
        if (!isHit)
        {
            DamageManager.DealDamage(collision, damage);
            GameObject explosion = Instantiate(explosionPatricles, transform.position, transform.rotation);
            Destroy(explosion, 1f);
            Destroy(gameObject);
            isHit = true;
        }
    }

}
