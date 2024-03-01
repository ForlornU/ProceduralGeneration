using UnityEngine;

public class Module_RandomWalk : GenerationModule
{
    [SerializeField, Range(0, 16)] int splitting = 0;

    [SerializeField, Range(0f, 0.1f)] float branchChance = 0f;

    public override int Sort(ModuleReferenceData data)
    {
        //Range is a measure from the last placed tile to 0 which is the first placed tile. if random branch, 0 is 'any placed tile'
        int lastPlacedTilesRange = data.connectors.Count - (data.lastTile.validTiles + splitting);
        int range;

        if (Random.value < branchChance)
            range = 0;
        else
            range = lastPlacedTilesRange;

        // If no valid tiles, again return the full lenght of all tiles to randomize
        int rndIndex;
        if (data.lastTile.validTiles <= 0)
            rndIndex = data.connectors.Count - 1;
        else
            rndIndex = Random.Range(range, data.connectors.Count);

        return rndIndex;
    }
}
