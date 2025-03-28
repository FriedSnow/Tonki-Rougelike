using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneKostylb : MonoBehaviour
{
    public VideoPlayer vp;
    void Awake()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f); // Значение по умолчанию: 0.5
        vp.SetDirectAudioVolume(0, savedVolume * .1f);
        AudioListener.volume = savedVolume;
    }
    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f); // Значение по умолчанию: 0.5
        vp.SetDirectAudioVolume(0, savedVolume * .1f);
        AudioListener.volume = savedVolume;
        StartCoroutine(bebebe());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(1);
    }
    IEnumerator bebebe()
    {
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene(1);
    }
}
