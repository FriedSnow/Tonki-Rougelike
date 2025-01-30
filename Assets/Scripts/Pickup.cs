using UnityEngine;
public class Pickup : MonoBehaviour
{
    public PickupType selectedPickup; // Выпадающий список в инспекторе
    public GameObject coinPickupSoundPrefab;
    public int amount = 1;
    TankController player;
    private void Start()
    {
        player = FindObjectOfType<TankController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (selectedPickup)
            {
                case PickupType.coin:
                    if (player.coins < player.maxCoins)
                    {
                        PlaySoundAndDie();
                        player.coins += amount;
                    }
                    break;
                case PickupType.gear:
                    if (player.health < player.maxHealth)
                    {
                        PlaySoundAndDie();
                        player.Heal(amount);
                    }
                    break;
                case PickupType.armor:
                    PlaySoundAndDie();
                    player.AddArmor(amount);
                    break;
            }
            return;
        }
    }
    public enum PickupType
    {
        coin,
        gear,
        armor
    }
    public void PlaySoundAndDie()
    {
        if (coinPickupSoundPrefab != null)
        {
            GameObject coinPickupSound = Instantiate(coinPickupSoundPrefab, transform.position, transform.rotation);
            Destroy(coinPickupSound, 3f);
        }
        Destroy(gameObject);
    }
}
