using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector] public List<Connector> connectors = new List<Connector>();
    [SerializeField] bool overrideID = true;
    [SerializeField] uint tileID = 0;

    [Header("wfc test")]
    [SerializeField] bool randomdlyRotate90DegreesTest = false;
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
        

        if (randomdlyRotate90DegreesTest && Random.value > 0.5f)
            transform.Rotate(transform.up, 90f);
            
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
