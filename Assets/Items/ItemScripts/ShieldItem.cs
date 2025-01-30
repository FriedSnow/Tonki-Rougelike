using UnityEngine;

public class ShieldItem : StandardItem
{
    public GameObject shieldPrefab;

    // private void Update()
    // {
    //     // Move();
    // }

    protected override void ApplyEffect()
    {
        // Создаем щит
        GameObject shield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);

        // Ищем объект башни в объекте игрока
        Transform turretTransform = player.transform.Find("Turret");

        // Если башня найдена, прикрепляем щит к башне
        if (turretTransform != null)
        {
            shield.transform.SetParent(turretTransform);
        }
        else
        {
            Debug.LogError("Turret not found in player object!");
        }

        // Звуковой эффект и UI
        // PlaySound();
        // AddItemToUI();
        // textSlide.ShowItemName("Shield!");

        // Устанавливаем позицию и вращение щита относительно башни
        shield.transform.localPosition = new Vector3(9, 0.5f, 0);
        shield.transform.localRotation = Quaternion.identity;
    }
}
