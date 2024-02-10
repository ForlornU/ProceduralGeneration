using System.Collections.Generic;
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

    //[HideInInspector] public Bounds bounds;

    private void Awake()
    {
        FindMyConnectors();

        if(overrideID)
        {
            foreach (Connector connector in connectors)
            {
                connector.connectorID = tileID;
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

    void FindMyConnectors()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Connector>())
            {
                connectors.Add(child.GetComponent<Connector>());
            }
        }
    }
}
