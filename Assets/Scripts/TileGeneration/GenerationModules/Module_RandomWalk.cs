using System.Collections.Generic;
using UnityEngine;

public class Module_RandomWalk : GenerationModule
{
    //Stack<Connector> connectors = new Stack<Connector>();

    [SerializeField] int randombranches = 0;

    public override int Sort(ModuleReferenceData data)
    {
        //PushStartingConnectors(connectors);


        //if (canProcessConnector(this.connectors.Pop()))
        //    Debug.Log("Valid connector");

        //return Random.Range(data.connectors.Count -6, data.connectors.Count - 1);//connectorsToSpawn[connectorIndex].parentTile.connectors.Count - 1); // randomize between the closest options
        Vector3 pos;
         
        if (data.lastTile == null)
        {
            pos = data.walkerPosition;
        }
        else
        {
            pos = data.lastTile.transform.position;
        }

        data.connectors.Sort((x, y) => Vector3.Distance(x.transform.position, pos).CompareTo(Vector3.Distance(y.transform.position, pos)));

        int roof =  data.connectors[data.connectorsIndex].parentTile.connectors.Count + randombranches;
        roof = Mathf.Clamp(roof, 0, data.connectors.Count -1);
        return Random.Range(0, roof);
        //return 0;
    }

    //private void PushStartingConnectors(List<Connector> startingConnectors)
    //{
    //    foreach (var connector in startingConnectors)
    //    {
    //        connectors.Push(connector);
    //    }

    //}

    //bool canProcessConnector(Connector c)
    //{
    //    currentConnector = c;

    //    This happens thousands of times, so we don't want to log it. But also find a way to avoid it
    //    if (c == null || c.isOccupied)
    //        return false;

    //    return true;
    //}
}
