using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Generational Settings", order = 0)]

public class GenerationSettings : ScriptableObject
{
    [Header("World")]
    [Tooltip("Power of 2 values only, 64, 128, 512, 1024 etc.")] public int OctreeSize = 512;
    public int voxelsToCreate = 5000;
    [Range(0f, 1f)] public float creationSpeed = 0.01f;

    [Tooltip("Size of block +1. For example 12: 13^3 = 2197 Voxels in a cube. This is also affected by inflation")]
    public int startBlockSize = 1;
    public int inflationPasses = 2;
    public bool InsideWorld = false;

    [Header("Randomness")]
    [Range(0f, 1f)] public float noise = 0.2f;
    [Range(-0.5f, 0.5f)] public float radialBias = 0.1f;
    public float branchChance = 0.1f;

    [Header("Torches")]
    [Range(0.001f, 0.1f)] public float torchesDistribution = 0.02f;
    [Range(0f, 128f)] public int maxTorches = 64;

    [Header("Debug")]
    public bool debug = true;
}
