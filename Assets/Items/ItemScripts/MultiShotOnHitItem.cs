using UnityEngine;
public class MultiShotOnHitItem : StandardItem
{
    public float bulletSpeed = 10f;   // Скорость снарядов
    public float spawnOffset = 5f; // Смещение точки спавна

    // private void Update()
    // {
    //     // Move();
    // }

    protected override void ApplyEffect()
    {
        // Проверяем, есть ли у игрока префаб для вторичного снаряда
        if (player.secondaryBulletPrefab != null)
        {
            // Добавляем новый on-hit эффект в список игрока
            player.OnHitEffects.Add(new MultiShotOnHitEffect(player.secondaryBulletPrefab, player.projectileSpeed, spawnOffset));
            // PlaySound();
            // AddItemToUI();
            // textSlide.ShowItemName("КАЗ", Colors.quality2, "Активная защита");
        }
        else
        {
            Debug.LogError("Player does not have a secondary bullet prefab assigned!");
        }
    }
}