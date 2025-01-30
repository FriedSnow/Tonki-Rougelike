public class HealthToArmor : StandardItem
{
    // private void Update()
    // {
    //     // Move();
    // }
    protected override void ApplyEffect()
    {
        if (player.health > 10)
        {
            player.AddArmor(player.health - 10);
            player.health -= player.maxHealth - 10;
        }
        // PlaySound();
        // AddItemToUI();
        // textSlide.ShowItemName("Health to Armor!");
        player.Heal(0);
    }
}

