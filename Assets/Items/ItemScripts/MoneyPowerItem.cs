public class MoneyPowerItem : StandardItem
{
    public bool isCollected = false;
    private void Update()
    {
        if (isCollected) player.damage = player.baseDamage + player.addedDamage + player.coins;
        // Move();
    }
    protected override void ApplyEffect()
    {
        isCollected = true;
        // PlaySound();
        // AddItemToUI();
        // textSlide.ShowItemName("Money = Power!");
    }
}

