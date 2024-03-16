using UnityEngine;

public class PlayerVoxelInteraction : MonoBehaviour
{
    Octree tree;
    [SerializeField] Transform Cursor;

    // Start is called before the first frame update
    void Start()
    {
        tree = World.Instance.treeReference;
    }

    // Update is called once per frame
    void Update()
    {

        // Get camera forward direction
        Vector3 forward = transform.forward;

        // Calculate target position 2 meters in front of the camera
        Vector3 targetPosition = transform.position + forward * 2.0f;

        // Round down to nearest half meter for each axis
        targetPosition.x = Mathf.Floor(targetPosition.x) + 0.5f;
        targetPosition.y = Mathf.Floor(targetPosition.y) + 0.5f;
        targetPosition.z = Mathf.Floor(targetPosition.z) + 0.5f;

        Cursor.position = targetPosition;

        if (Input.GetMouseButton(0))
        {
            if (tree == null)
                tree = World.Instance.treeReference;
            tree.RemoveVoxel(targetPosition);
        }
        else if (Input.GetMouseButton(1))
        {
            if (tree == null)
                tree = World.Instance.treeReference;
            tree.InsertVoxel(new Voxel(targetPosition, Voxel.VoxelType.Stone));
        }
    }
}
