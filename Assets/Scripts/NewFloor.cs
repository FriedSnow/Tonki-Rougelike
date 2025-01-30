using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewFloor : MonoBehaviour
{
    private TankController player; // Ссылка на игрока
    GameObject startRoom;
    private void Start()
    {
        player = FindObjectOfType<TankController>();
        startRoom = GameObject.Find("StartRoom");
        player.transform.position = startRoom.transform.position + new Vector3(0, 10, 0);
    }
}
