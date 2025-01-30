using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Объект, который будет заспавнен
    static string qwa; 
    public void LoadGame1()
    {
        PlayerPrefs.SetString("ObjectToSpawn", objectsToSpawn[0].name);
        // Debug.Log(objectsToSpawn[0].name);
        SceneManager.LoadScene("GameScene");
        if (!PlayerPrefs.HasKey("Unlockable0"))
        {
            for (int i = 0; i < 6; i++)
            {
                PlayerPrefs.SetInt($"Unlockable{i}", 0);
            }
            PlayerPrefs.SetInt($"Unlockable0", 1);
        }
        qwa = objectsToSpawn[0].name;
    }
    
    public void LoadGame2()
    {
        SceneManager.LoadScene("ChooseScene");
        if (!PlayerPrefs.HasKey("Unlockable0"))
        {
            for (int i = 0; i < 6; i++)
            {
                PlayerPrefs.SetInt($"Unlockable{i}", 0);
            }
            PlayerPrefs.SetInt($"Unlockable0", 1);
        }
    }
}
