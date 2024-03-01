using UnityEngine;

public class Module_RandomWalk : GenerationModule
{
    [SerializeField, Range(0, 16)] int splitting = 0;

    [SerializeField, Range(0f, 0.1f)] float branchChance = 0f;

    public override int Sort(ModuleReferenceData data)
    {
        int range = Random.value < branchChance ? range = 0 : data.connectors.Count - (data.lastTile.validTiles + splitting);

        int rndIndex = Random.Range(range, data.connectors.Count);

        if (data.lastTile.validTiles <= 0)
            rndIndex = data.connectors.Count - 1;

        return rndIndex;
    }
}
