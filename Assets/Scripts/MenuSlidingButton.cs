using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSlidingButton : MonoBehaviour
{
    [Header("Настройки")]
    public Vector3 moveOffset = new Vector3(1, 0, 0); // Сдвиг по оси X
    public float moveSpeed = 5.0f; // Скорость движения
    public GameObject selectSoundPrefab;
    public enum ButtonType
    {
        start,
        choose,
        exit
    }
    public ButtonType buttonType;

    private Vector3 _originalPosition;
    private bool _isHovered = false;

    void Start()
    {
        _originalPosition = transform.position;

        // Инициализация PlayerPrefs при первом запуске
        if (!PlayerPrefs.HasKey("Unlockable0"))
        {
            for (int i = 0; i < 6; i++)
            {
                PlayerPrefs.SetInt($"Unlockable{i}", i == 0 ? 1 : 0);
            }
            PlayerPrefs.Save();
        }
    }

    void OnMouseEnter()
    {
        _isHovered = true;
        GameObject selectSound = Instantiate(selectSoundPrefab);
        Destroy(selectSound, 1f);
    }

    void OnMouseExit()
    {
        _isHovered = false;
    }

    void OnMouseDown()
    {
        switch (buttonType)
        {
            case ButtonType.start:
                PlayerPrefs.SetString("ObjectToSpawn", "Tank-1");
                SceneManager.LoadScene("GameScene");
                break;

            case ButtonType.choose:
                SceneManager.LoadScene("ChooseScene");
                break;

            case ButtonType.exit:
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
        }
    }

    void Update()
    {
        Vector3 targetPosition = _isHovered ?
            _originalPosition + moveOffset :
            _originalPosition;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }
}