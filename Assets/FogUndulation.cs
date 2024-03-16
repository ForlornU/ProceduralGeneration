using UnityEngine;

public class FogUndulation : MonoBehaviour
{
    [SerializeField] float minDensity;
    [SerializeField] float maxDensity;
    [SerializeField] float frequency = 1f;
    void Update()
    {
        // Scale Time.deltaTime for frequency control
        float noiseInput = frequency * Time.deltaTime;
        float undulation = Mathf.Clamp(Mathf.PerlinNoise(noiseInput, noiseInput), 0f, 1f);
        float newFogDensity = Mathf.Lerp( minDensity, maxDensity, undulation);
        RenderSettings.fogDensity = newFogDensity;
    }
}
