using UnityEngine;
public class GlassCannon : StandardItem
{
    protected override void ApplyEffect()
    {
        player.FullHealth();
        // player.Heal(-(player.maxHealth - 10));
        // player.health -= player.maxHealth - 10;
        player.TakeDamage(player.maxHealth - 10);
        player.canTakeHealthUp = false;
        player.maxHealth = 10;
    }
}

