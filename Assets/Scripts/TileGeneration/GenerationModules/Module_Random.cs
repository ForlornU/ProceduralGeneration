using System.Collections.Generic;
using UnityEngine;

public class Module_Random : GenerationModule
{
    public override int Sort(ModuleReferenceData data)
    {
        return Random.Range(0, data.connectors.Count - 1);
    }
}


