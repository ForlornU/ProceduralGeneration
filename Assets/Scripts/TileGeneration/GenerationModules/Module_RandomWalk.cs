using UnityEngine;

public class Module_RandomWalk : GenerationModule
{
    [SerializeField, Range(0, 16)] int splitting = 0;

    [SerializeField, Range(0f, 0.1f)] float branchChance = 0f;
    [SerializeField, Range(1, 999)] int minBranchLength = 10;
    int branchCounter = 0;

    public override int Sort(ModuleReferenceData data)
    {
        int max = data.connectors.Count - 1;
        int min = Mathf.Clamp(data.connectors.Count - (data.lastTile.validTiles + splitting), 0, data.connectors.Count);

        branchCounter = Mathf.Clamp(branchCounter - 1, 0, 999);

        //If branch, return full length of all connectors, essentially a single random placement
        if (Random.value < branchChance && branchCounter < 1)
            min = 0;

        return Random.Range(min, max);
    }
}
