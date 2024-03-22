using UnityEngine;

public class PlayerVoxelInteraction : MonoBehaviour
{
    Octree tree;
    [SerializeField] GameObject targetVoxelCursor;
    [SerializeField] GameObject normalCursor;
    [SerializeField] LayerMask hitMask;
    bool canAlter = false;
    Vector3 hitVoxelPosition;
    Vector3 neighborVoxelPosition;

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 3f, hitMask))
        {
            if (tree == null)
                tree = World.Instance.treeReference;

            canAlter = true;
            normalCursor.SetActive(true);
            targetVoxelCursor.SetActive(true);
            PositionCursors(hitInfo);

            if (World.Instance.invertedWorld == true)
            {
                if (Input.GetMouseButtonDown(0) && canAlter)
                {
                    tree.RemoveVoxel(neighborVoxelPosition);
                    tree.CubicQuery(neighborVoxelPosition);
                }
                //Removing a voxel (by adding one)
                else if (Input.GetMouseButton(1) && canAlter)
                {
                    tree.InsertVoxel(new Voxel(hitVoxelPosition, Voxel.VoxelType.Stone));
                    tree.CubicQuery(hitVoxelPosition);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && canAlter)
                {
                    tree.InsertVoxel(new Voxel(neighborVoxelPosition, Voxel.VoxelType.Stone));
                    tree.CubicQuery(neighborVoxelPosition);
                }
                //Removing a voxel (by adding one)
                else if (Input.GetMouseButton(1) && canAlter)
                {
                    tree.RemoveVoxel(hitVoxelPosition);
                    tree.CubicQuery(hitVoxelPosition);
                }
            }

        }
        else
        {
            normalCursor.SetActive(false);
            targetVoxelCursor.SetActive(false);
            canAlter = false;
        }
    }

    void PositionCursors(RaycastHit hitInfo)
    {
        Vector3 intoVoxel = hitInfo.point - hitInfo.normal * 0.5f;
        intoVoxel = snap(intoVoxel);

        Vector3 outOfVoxel = hitInfo.point + hitInfo.normal * 0.5f;
        outOfVoxel = snap(outOfVoxel);

        hitVoxelPosition = intoVoxel;
        neighborVoxelPosition = outOfVoxel;

        targetVoxelCursor.transform.position = intoVoxel;
        normalCursor.transform.position = outOfVoxel;
        normalCursor.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
    }

    private Vector3 snap(Vector3 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;
        pos.z = Mathf.Floor(pos.z) + 0.5f;
        return pos;
    }
}
