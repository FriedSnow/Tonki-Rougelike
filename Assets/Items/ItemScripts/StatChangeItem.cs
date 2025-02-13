using System.Collections.Generic;
using System.Linq;
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

    public bool canBeMaxed = false;
    // [HideInInspector]
    public int maxAmount;

    private void Update()
    {
        Move();
    }

    protected override void ApplyEffect()
    {
        if (!canBeMaxed || (canBeMaxed && player.Inventory.Count(item => item == this.gameObject.name) < maxAmount))
        {
            player.addedDamage += damageChange;
            player.HealthUp(maxHealthChange);
            player.maxSpeed += maxSpeedChange;
            player.turnSpeed += maxSpeedChange * 2;
            player.fireRate += fireRateChange;
            player.projectileSpeed += projectileSpeedChange;
            player.luck += luckChange;
            player.Heal(healthChange);
            player.AddArmor(armorChange);
            player.coins += coinsChange;
        }

        color1 = Colors.GetColorByQuality(color1Selection);
        color2 = Colors.GetColorByQuality(color2Selection);
        PlaySound();
        AddItemToUI();
        textSlide.ShowItemName(message1, color1, message2, color2);
        Debug.Log($"{this.gameObject.name} - {player.Inventory.Count(item => item == this.gameObject.name)}");
    }
}