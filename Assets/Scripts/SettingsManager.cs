using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider volumeSlider; // Ссылка на слайдер громкости
    public TMP_Text volumeValueText; // Ссылка на текст для отображения значения
    public Toggle monkeyModeToggle; // Ссылка на чекбокс

    private void Start()
    {
        // Загрузка сохраненных настроек при старте
        LoadSettings();
    }

    private void Update()
    {
        // Обновление значения громкости в реальном времени
        if (volumeSlider != null)
        {
            AudioListener.volume = volumeSlider.value;
            volumeValueText.text = $"{Mathf.Round(volumeSlider.value * 100)}";
        }
    }

    public void SaveSettings()
    {
        // Сохранение настроек
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetInt("MonkeyModeEnabled", monkeyModeToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadSettings()
    {
        // Загрузка настроек
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f); // Значение по умолчанию: 0.5
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;
            volumeValueText.text = "Volume: " + Mathf.Round(savedVolume * 100) / 100f;
        }

        if (monkeyModeToggle != null)
        {
            bool isExtraFeatureEnabled = PlayerPrefs.GetInt("MonkeyModeEnabled", 0) == 1;
            monkeyModeToggle.isOn = isExtraFeatureEnabled;
        }
    }
}