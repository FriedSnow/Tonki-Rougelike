using UnityEngine;

public class StatChangeItem : StandardItem
{
    public int damageChange;
    public int maxHealthChange;
    public float maxSpeedChange;
    public float fireRateChange;
    public float projectileSpeedChange;
    public int luckChange;
    public int healthChange;
    public int armorChange;
    public int coinsChange;

    public string message1 = "-";
    public Colors.QualityColor color1Selection = Colors.QualityColor.q0; // Выбор цвета через перечисление
    [HideInInspector] public Color color1 = Color.white;

    public string message2 = "-";
    public Colors.QualityColor color2Selection = Colors.QualityColor.q0; // Выбор цвета через перечисление
    [HideInInspector] public Color color2 = Color.white;

    private void Update()
    {
        Move();
    }

    protected override void ApplyEffect()
    {
        // Применяем выбранные цвета
        color1 = Colors.GetColorByQuality(color1Selection);
        color2 = Colors.GetColorByQuality(color2Selection);

        player.addedDamage += damageChange;
        player.maxHealth += maxHealthChange;
        player.maxSpeed += maxSpeedChange; 
        player.turnSpeed += maxSpeedChange * 2;
        player.fireRate += fireRateChange;
        player.projectileSpeed += projectileSpeedChange;
        player.luck += luckChange;
        player.Heal(healthChange);
        player.AddArmor(armorChange);
        player.coins += coinsChange;
        PlaySound();
        AddItemToUI();
        textSlide.ShowItemName(message1, color1, message2, color2);
    }
}