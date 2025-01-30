using UnityEngine;

public class AmmoSwapItem : StandardItem
{
    public GameObject projectilePrefab;
    public bool isSecondary;
    public bool isMoving = false;
    private void Update()
    {
        if (isMoving)
            Move();
    }
    protected override void ApplyEffect()
    {
        if (isSecondary)
        {
            if (player.primaryBulletPrefab.name == "Bullet")
                player.primaryBulletPrefab = projectilePrefab;
            player.secondaryBulletPrefab = projectilePrefab;
            AddAttackSprite(false, true);
        }
        else
        {
            player.primaryBulletPrefab = projectilePrefab;
            AddAttackSprite(true);
        }
    }
}

