using UnityEngine;

public class Connector : MonoBehaviour
{
    public Tile parentTile;
    public uint connectorID;

    public bool isOccupied;// { get; set; }
    public Connector connectedTo;
}
