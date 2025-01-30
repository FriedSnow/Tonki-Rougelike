using UnityEngine;

public class MultishotItem : StandardItem
{
    public GameObject projectilePrefab;
    private void Update()
    {
        Move();
    }
    protected override void ApplyEffect()
    {
        player.primaryBulletPrefab = projectilePrefab;

        PlaySound();
        AddItemToUI();
        AddAttackSprite(true);
        textSlide.ShowItemName("Multishot!");
    }
}