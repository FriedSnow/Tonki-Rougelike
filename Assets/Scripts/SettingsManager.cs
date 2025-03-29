using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeValueText;
    public Toggle monkeyModeToggle;
    public Toggle cameraAngleToggle;
    public Toggle perspectiveToggle;

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
        PlayerPrefs.SetInt("AngleCameraEnabled", cameraAngleToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("PerspectiveEnabled", perspectiveToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadSettings()
    {
        // Загрузка настроек
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f); // Значение по умолчанию: 0.5
            AudioListener.volume = savedVolume;
            volumeSlider.value = savedVolume;
            volumeValueText.text = "Volume: " + Mathf.Round(savedVolume * 100) / 100f;
        }

        if (monkeyModeToggle != null)
        {
            bool isExtraFeatureEnabled = PlayerPrefs.GetInt("MonkeyModeEnabled", 0) == 1;
            monkeyModeToggle.isOn = isExtraFeatureEnabled;
        }
        if (cameraAngleToggle != null)
        {
            bool isAngleCameraEnabled = PlayerPrefs.GetInt("AngleCameraEnabled", 0) == 1;
            cameraAngleToggle.isOn = isAngleCameraEnabled;
        }
        if (perspectiveToggle != null)
        {
            bool isPerspectiveEnabled = PlayerPrefs.GetInt("PerspectiveEnabled", 1) == 1;
            perspectiveToggle.isOn = isPerspectiveEnabled;
        }
    }
}