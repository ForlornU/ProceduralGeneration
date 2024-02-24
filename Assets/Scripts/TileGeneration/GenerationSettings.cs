using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "ScriptableObjects/Generational Modifier", order = 0)]

public class GenerationSettings : ScriptableObject
{
    public PassSettings[] Passes;
}

[System.Serializable]
public struct PassSettings
{
    public string modulename;
    public int tileCount;
    public bool isInstant;
    [Range(0, 5)] public float creationspeed;
}
