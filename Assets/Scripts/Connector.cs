using UnityEngine;

public class Connector : MonoBehaviour
{
    uint id = 1;

    public bool isOccupied { get; set; }

    public void setID(uint _id)
    {
        id = _id;
    }

    public uint getID()
    {
        return id;
    }
}
