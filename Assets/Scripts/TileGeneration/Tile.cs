using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector] public List<Connector> connectors = new List<Connector>();
    [SerializeField] bool overrideID = true;
    [SerializeField] uint tileID = 0;

    [Header("wfc test")]
    ///options
    bool collapsed = false;
    int entropy = 0;

    public void Init()
    {
        EnableConnectors();
        if (overrideID)
        {
            foreach (Connector connector in connectors)
            {
                connector.connectorID = tileID;
                connector.parentTile = this;
            }
        }
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
