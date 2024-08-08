using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isOccupied = false;

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
}
