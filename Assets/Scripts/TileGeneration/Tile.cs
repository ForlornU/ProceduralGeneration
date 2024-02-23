using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector] public List<Connector> connectors = new List<Connector>();
    [SerializeField] bool overrideID = true;
    [SerializeField] uint tileID = 0;

    public int validTiles {  get { return ValidTiles(); } }

    [Header("wfc test")]
    ///options
    bool collapsed = false;
    int entropy = 0;

    public void Init()
    {
        EnableConnectors();

        foreach (Connector connector in connectors)
        {
            if (overrideID)
                connector.connectorID = tileID;
            connector.parentTile = this;
        }           
    }

    int ValidTiles()
    {
        int x = 0;
        foreach (Connector connector in connectors)
        {
            if(!connector.isOccupied)
            {
                x++;
            }
        }
        return x;
    }

    int GetEntropy()
    {
        return 0;
    }

    void UpdateValues()
    {
        collapsed = entropy == 1;
    }

    void TryCollapse()
    {
        //Select a tile from available options
        //Collapse the tile
    }

    void EnableConnectors()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Connector>())
            {
                connectors.Add(child.GetComponent<Connector>());
            }
        }
    }

    public void ForgetConnections()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Connector>())
            {
                Connector connector = child.GetComponent<Connector>();
                connector.isOccupied = false;
                connector.connectedTo = null;
            }
        }
    }
}
