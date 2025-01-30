using UnityEngine;

public class Room : MonoBehaviour
{
    // Флаги для определения, в каком направлении есть выходы (направление выхода)
    public bool hasTopExit;
    public bool hasBottomExit;
    public bool hasLeftExit;
    public bool hasRightExit;

    // Метод для проверки, можно ли соединить две комнаты
    public bool CanConnect(Room otherRoom, Vector2Int direction)
    {
        if (direction == Vector2Int.up) return hasTopExit && otherRoom.hasBottomExit;
        if (direction == Vector2Int.down) return hasBottomExit && otherRoom.hasTopExit;
        if (direction == Vector2Int.left) return hasLeftExit && otherRoom.hasRightExit;
        if (direction == Vector2Int.right) return hasRightExit && otherRoom.hasLeftExit;
        return false;
    }
}
