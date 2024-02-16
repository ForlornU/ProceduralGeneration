using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] Transform cursor;

    //Follower
    Vector3 centerposition;
    Transform smoothFollower;
    Vector3 smoothVelocity = Vector3.zero;

    //Perlin noise
    [SerializeField] bool randomFOVNoise;
    public float noiseScale = 0.1f; // Scale of the Perlin noise
    public float speed = 1f; // Speed of change
    
    //FOV
    [SerializeField, Range(0.1f, 1f)] float minZoomDistanceMult = .25f;
    [SerializeField, Range(1.05f, 5f)] float maxZoomDistanceMult = 3f;
    [SerializeField, Range(0.25f, 1f)] float followingStrictness = 0.6f;

    float startdist;
    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        smoothFollower = new GameObject("CursorFollow").transform;
        smoothFollower.transform.position = centerposition;

        startdist = Vector3.Distance(transform.position, smoothFollower.position);
    }

    void Update()
    {

        centerposition = (Vector3.zero + cursor.position) * 0.75f;
        smoothFollower.position = Vector3.SmoothDamp(smoothFollower.position, centerposition, ref smoothVelocity, 1.5f);

        Quaternion newRotation = Quaternion.LookRotation(smoothFollower.position - transform.position, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.smoothDeltaTime);
        transform.parent.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime);

        ZoomView();
    }

    private void ZoomView()
    {
        float dist = Vector3.Distance(transform.position, smoothFollower.position);
        float t = Mathf.InverseLerp(startdist*minZoomDistanceMult, startdist*maxZoomDistanceMult, dist);

        if(randomFOVNoise)
            t += Mathf.PerlinNoise(Time.time * speed, 0f) * noiseScale;

        cam.fieldOfView = Mathf.Lerp(90, 10, t);
    }
}
