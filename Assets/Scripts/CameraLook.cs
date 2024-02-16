using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] Transform cursor;
    Vector3 centerposition;
    Transform smoothFollower;
    Vector3 smoothVelocity = Vector3.zero;

    private void Start()
    {
        smoothFollower = new GameObject("CursorFollow").transform;
        smoothFollower.transform.position = centerposition;
    }

    void Update()
    {

        centerposition = (Vector3.zero + cursor.position) * 0.5f;
        smoothFollower.position = Vector3.SmoothDamp(smoothFollower.position, centerposition, ref smoothVelocity, 1.5f);

        Quaternion newRotation = Quaternion.LookRotation(smoothFollower.position - transform.position, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.smoothDeltaTime);
    }
}
