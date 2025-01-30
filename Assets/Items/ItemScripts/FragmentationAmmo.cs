using UnityEngine;

public class FragmentationAmmo : StandardItem
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
    }
}

