using UnityEngine;

public class Module_RandomWalk : GenerationModule
{
    [SerializeField, Range(0, 16)] int splitting = 0;

    [SerializeField, Range(0f, 0.1f)] float branchChance = 0f;

    public override int Sort(ModuleReferenceData data)
    {
        //Range is a measure from the last placed tile to 0 which is the first placed tile
        // if random branch, let the range be 0 which is 'any placed tile'
        int lastPlacedTilesRange = data.connectors.Count - (data.lastTile.validTiles + splitting);
        int range = Random.value < branchChance ? 0 : lastPlacedTilesRange;

        // If no valid tiles, again return the full lenght of all tiles to randomize
        int rndIndex = data.lastTile.validTiles <= 0 ? data.connectors.Count - 1 : Random.Range(range, data.connectors.Count);

        return rndIndex;
    }
}
