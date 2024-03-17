using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Generational Settings", order = 0)]

public class GenerationSettings : ScriptableObject
{
    [Header("Generation Settings")]
    [Tooltip("value + 3 ^ 3 is the starting block of voxels where the player spawns. 1 = 27, 2 = 125, 3 = 216.. This is also affected by inflation")]
    public int startBlockSize = 1;
    public int inflationPasses = 2;

    public int voxelsToCreate = 5000;
    [Range(0f, 1f)]public float creationSpeed = 0.01f;
    [Range(0f, 1f)] public float noise = 0.2f;


    [Header("Torches")]
    [Range(0.001f, 0.1f)] public float torchesDistribution = 0.02f;

    [Header("Debug")]
    public bool debug = true;
}
