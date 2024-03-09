using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Generational Settings", order = 0)]

public class GenerationSettings : ScriptableObject
{
    [Header("Generation Settings")]
    //public ResultType type;
    //public PassSettings[] passes;

    public int maxVoxels = 5000;
    public float creationSpeed = 0.01f;
    public int inflationPasses = 2;
    public bool inwardsNormals = true;
    [Range(0f, 1f)] public float noise = 0.2f;
    public Material material;
}

[System.Serializable]
public struct PassSettings
{
    public string modulename;
    [Range(1, 9999)] public int maxCount;
    //public bool isInstant;
    [Range(0, 5)] public float creationspeed;
}

public enum ResultType { Inner, Outer, Both };