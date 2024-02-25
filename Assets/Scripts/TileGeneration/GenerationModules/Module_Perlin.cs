using UnityEngine;

public class Module_Perlin : GenerationModule
{
    public float noiseScale = 0.1f; // Scale factor for Perlin noise

    public override int Sort(ModuleReferenceData data)
    {
        float perlinValue = Mathf.PerlinNoise(Time.time * noiseScale, 0f);

        return Mathf.FloorToInt(perlinValue * data.connectors.Count-1);
    }
}
