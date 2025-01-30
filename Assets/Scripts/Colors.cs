using UnityEngine;

public class Colors : MonoBehaviour
{
    // Задаем цвета, используя метод Color32 для шестнадцатеричных значений
    public static Color quality0 = new Color32(0xC8, 0xC8, 0xC8, 0xFF); // C8C8C8
    public static Color quality1 = new Color32(0x50, 0xE6, 0x64, 0xFF); // 50E664
    public static Color quality2 = new Color32(0xA1, 0x5C, 0xE6, 0xFF); // A15CE6
    public static Color quality3 = new Color32(0xFF, 0xCF, 0x34, 0xFF); // FFCF34

    // Определяем перечисление для выбора цветов
    public enum QualityColor
    {
        q0,
        q1,
        q2,
        q3
    }

    // Метод для получения цвета по выбранному значению перечисления
    public static Color GetColorByQuality(QualityColor quality)
    {
        switch (quality)
        {
            case QualityColor.q0:
                return quality0;
            case QualityColor.q1:
                return quality1;
            case QualityColor.q2:
                return quality2;
            case QualityColor.q3:
                return quality3;
            default:
                return Color.white; // Возвращаем белый цвет по умолчанию, если ничего не выбрано
        }
    }
}