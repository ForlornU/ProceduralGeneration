using UnityEngine;

public class Module_RandomWalk : GenerationModule
{
    [SerializeField, Range(0, 99)] int additionalBranches = 0;

    public override int Sort(ModuleReferenceData data)
    {
        int range = data.connectors.Count - (data.lastTile.validTiles + additionalBranches);

        int rndIndex = Random.Range(range, data.connectors.Count);

        if (data.lastTile.validTiles <= 0)
            rndIndex = data.connectors.Count - 1;

        return rndIndex;
    }
}
