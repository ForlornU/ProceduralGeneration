using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    bool shareID = false;
    [SerializeField]
    private uint id;

    public List<Connector> connectors = new List<Connector>();
    public Bounds bounds;

    private void Awake()
    {
        FindMyConnectors();
        bounds = GetComponent<Collider>().bounds;

        if (shareID)
            SetIDOnConnectors();

    }

    void FindMyConnectors()
    {
        Debug.Log("Finding connectors");
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Connector>())
            {
                connectors.Add(child.GetComponent<Connector>());
            }
        }
    }

    void SetIDOnConnectors()
    {
        Debug.Log("Setting ID on connectors");
        foreach (Connector connector in connectors)
        {
            connector.setID(id);
        }
    }
 
}
