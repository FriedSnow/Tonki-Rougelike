public class AdrenalineItem : StandardItem
{
    public bool isCollected = false;
    private void Update()
    {
        if (isCollected) player.addedDamage = (player.maxHealth/player.health);
        Move();
    }
    protected override void ApplyEffect()
    {
        PlaySound();
        AddItemToUI();
        isCollected = true;
        textSlide.ShowItemName("Адреналин");
    }
}

