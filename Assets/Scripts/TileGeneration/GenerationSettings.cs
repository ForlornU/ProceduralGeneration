using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Generational Settings", order = 0)]

public class GenerationSettings : ScriptableObject
{
    [Header("Generation Settings")]
    public GameObject startTile;
    public PassSettings[] Passes;
}

[System.Serializable]
public struct PassSettings
{
    public string modulename;
    public string optionalDescription;
    [Range(1, 9999)] public int tileCount;
    public bool isInstant;
    [Range(0, 5)] public float creationspeed;
}
