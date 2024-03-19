using UnityEngine;

public class FogUndulation : MonoBehaviour
{
    [SerializeField] float minDensity;
    [SerializeField] float maxDensity;
    [SerializeField] float frequency = 1f;
    void Update()
    {
        // Scale Time.deltaTime for frequency control
        float undulation = Mathf.Clamp(Mathf.PerlinNoise(frequency * Time.deltaTime, frequency * Time.time), 0f, 1f);
        RenderSettings.fogDensity = Mathf.Lerp(minDensity, maxDensity, undulation);
    }
}
