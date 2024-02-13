using UnityEngine;

public class CameraLook : MonoBehaviour
{
    //[SerializeField] private Camera _camera;
    [SerializeField] Transform cursor;
    Vector3 centerposition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        centerposition = (Vector3.zero + cursor.position) * 0.5f;

        Quaternion newRotation = Quaternion.LookRotation(centerposition - transform.position, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.smoothDeltaTime);
    }
}
