using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] Transform cursor;
    Vector3 centerposition;

    void Update()
    {
        centerposition = (Vector3.zero + cursor.position) * 0.5f;

        Quaternion newRotation = Quaternion.LookRotation(centerposition - transform.position, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.smoothDeltaTime);
    }
}
