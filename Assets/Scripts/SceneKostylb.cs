using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneKostylb : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(1);
    }
    void Start()
    {
        StartCoroutine(bebebe());
    }
    IEnumerator bebebe()
    {
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene(1);
    }
}
