public class GlassCannon : StandardItem
{
    private bool isCollected = false;
    private void Update()
    {
        if (isCollected)
        {
            player.maxHealth = 10;
        }
    }
    protected override void ApplyEffect()
    {
        player.health = 10;
        isCollected = true;
        player.Heal(0);
        // PlaySound();
        // AddItemToUI();
    }
}

