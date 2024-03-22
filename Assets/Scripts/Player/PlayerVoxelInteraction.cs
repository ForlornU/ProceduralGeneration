using UnityEngine;

public class PlayerVoxelInteraction : MonoBehaviour
{
    Octree tree;
    [SerializeField] GameObject targetVoxelCursor;
    [SerializeField] GameObject normalCursor;
    bool canAlter = false;
    [SerializeField] LayerMask hitMask;
    Vector3 hitVoxelPosition;
    Vector3 neighborVoxelPosition;

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 3f, hitMask))
        {
            canAlter = true;
            normalCursor.SetActive(true);
            targetVoxelCursor.SetActive(true);


            bool insideWorld = World.Instance.invertedWorld;

            //Centre of hit voxel
            if(insideWorld)
            {
                hitVoxelPosition = hitInfo.point - hitInfo.normal * 0.5f;
                hitVoxelPosition = snap(hitVoxelPosition);

                //Centre of potential neighbor voxel
                neighborVoxelPosition = hitInfo.point + hitInfo.normal * 0.5f;
                neighborVoxelPosition = snap(neighborVoxelPosition);

                targetVoxelCursor.transform.position = hitVoxelPosition;
                normalCursor.transform.position = neighborVoxelPosition;
                normalCursor.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);

                //Adding a voxel (by removing)
                if (Input.GetMouseButtonDown(0) && canAlter)
                {
                    if (tree == null)
                        tree = World.Instance.treeReference;
                    tree.RemoveVoxel(neighborVoxelPosition);
                    tree.CubicQuery(neighborVoxelPosition);
                }
                //Removing a voxel (by adding one)
                else if (Input.GetMouseButton(1) && canAlter)
                {
                    if (tree == null)
                        tree = World.Instance.treeReference;
                    tree.InsertVoxel(new Voxel(hitVoxelPosition, Voxel.VoxelType.Stone));
                    tree.CubicQuery(hitVoxelPosition);
                }
            }
            else
            {
                neighborVoxelPosition = hitInfo.point + hitInfo.normal * 0.5f;
                hitVoxelPosition = hitInfo.point - hitInfo.normal * 0.5f;

                hitVoxelPosition = snap(hitVoxelPosition);
                neighborVoxelPosition = snap(neighborVoxelPosition);

                targetVoxelCursor.transform.position = hitVoxelPosition;
                normalCursor.transform.position = neighborVoxelPosition;
                normalCursor.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);

                //Adding a voxel (by removing)
                if (Input.GetMouseButtonDown(0) && canAlter)
                {
                    if (tree == null)
                        tree = World.Instance.treeReference;
                    tree.InsertVoxel(new Voxel(neighborVoxelPosition, Voxel.VoxelType.Stone));
                    tree.CubicQuery(neighborVoxelPosition);

                }
                //Removing a voxel (by adding one)
                else if (Input.GetMouseButton(1) && canAlter)
                {
                    if (tree == null)
                        tree = World.Instance.treeReference;
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


    private Vector3 snap(Vector3 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;
        pos.z = Mathf.Floor(pos.z) + 0.5f;
        return pos;
    }
}
