using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private uint id;

    void FindMyConnectors()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Connector>())
            {
                child.GetComponent<Connector>().setID(id);
            }
        }
    }
}
