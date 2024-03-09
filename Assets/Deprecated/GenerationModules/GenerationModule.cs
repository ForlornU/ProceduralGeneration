using System.Collections.Generic;
using UnityEngine;

public class GenerationModule : MonoBehaviour
{
    int maxtiles;

    public void InitModule(int max)
    {
        maxtiles = max;
    }

    public virtual int Sort(ModuleReferenceData data)
    {
        return 0;
    }

    public void ExitModule()
    {

    }
}

public class ModuleReferenceData
{
    //public List<Connector> connectors;
    public int connectorsIndex;
    public Vector3 walkerPosition;
   // public Tile lastTile;
}
