using UnityEngine;

public class PlayerVoxelInteraction : MonoBehaviour
{
    Octree tree;
    Camera cam;
    [SerializeField] GameObject targetVoxelCursor;
    [SerializeField] GameObject normalCursor;
    bool canAlter = false;
    [SerializeField] LayerMask hitMask;
    Vector3 hitVoxelPosition;
    Vector3 neighborVoxelPosition;

    // Start is called before the first frame update
    void Start()
    {
        tree = World.Instance.treeReference;
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 3f, hitMask))
        {
            canAlter = true;
            normalCursor.SetActive(true);
            targetVoxelCursor.SetActive(true);

            //Centre of hit voxel
            hitVoxelPosition = hitInfo.point - hitInfo.normal * 0.5f; 
            hitVoxelPosition = snap(hitVoxelPosition);

            //Centre of potential neighbor voxel
            neighborVoxelPosition = hitInfo.point + hitInfo.normal * 0.5f;
            neighborVoxelPosition = snap(neighborVoxelPosition);

            targetVoxelCursor.transform.position = hitVoxelPosition;
            normalCursor.transform.position = neighborVoxelPosition;
            normalCursor.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
        }
        else
        {
            normalCursor.SetActive(false);
            targetVoxelCursor.SetActive(false);
            canAlter = false;
        }

        //Adding a voxel (by removing)
        if (Input.GetMouseButtonDown(0) && canAlter)
        {
            if (tree == null)
                tree = World.Instance.treeReference;
            tree.RemoveVoxel(neighborVoxelPosition);
        }
        //Removing a voxel (by adding one)
        else if (Input.GetMouseButton(1) && canAlter)
        {
            if (tree == null)
                tree = World.Instance.treeReference;
            tree.InsertVoxel(new Voxel(hitVoxelPosition, Voxel.VoxelType.Stone));
        }
    }

    private Vector3 snap(Vector3 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;
        pos.z = Mathf.Floor(pos.z) + 0.5f;
        return pos;
    }
}
