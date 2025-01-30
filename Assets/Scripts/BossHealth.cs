using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    Slider healthBar;
    TMP_Text nameText;
    void Start()
    {
        GameObject sliderObject = GameObject.Find("HealthSlider");
        if (sliderObject != null)
        {
            healthBar = sliderObject.GetComponent<Slider>();
            healthBar.gameObject.SetActive(false); // Hide the slider initially
        }
        GameObject textObject = GameObject.Find("NameText");
        if (textObject != null)
        {
            nameText = textObject.GetComponent<TMP_Text>();
            nameText.gameObject.SetActive(false); // Hide the slider initially
        }
    }

    public void ShowHealthBar(int maxHealth, string name = "")
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
            healthBar.gameObject.SetActive(true);
        }
        if (nameText != null)
        {
            nameText.gameObject.SetActive(true);
            nameText.text = name;
        }
    }

    public void UpdateHealthBar(int currentHealth)
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    public void HideHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
        if (nameText != null)
        {
            nameText.gameObject.SetActive(false);
            nameText.text = null;
        }
    }
}