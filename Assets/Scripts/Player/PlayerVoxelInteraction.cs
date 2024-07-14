using UnityEngine;

public class PlayerVoxelInteraction : MonoBehaviour
{
    Octree tree;
    [SerializeField] GameObject targetVoxelCursor;
    [SerializeField] GameObject normalCursor;
    [SerializeField] LayerMask hitMask;
    [SerializeField] ParticleSystem blockBreak;

    Vector3 hitVoxelPosition;
    Vector3 neighborVoxelPosition;

    // Update is called once per frame
    void Update()
    {
        ActivateCursors(false);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 3f, hitMask))
        {
            if (tree == null)
                tree = World.Instance.treeReference;

            ActivateCursors(true);
            PositionCursors(hitInfo);

            //Change Behaviour depending on outer/inner generation
            if (World.Instance.invertedWorld == true)
            {
                Click(hitVoxelPosition, neighborVoxelPosition, 0, 1);
            }
            else
            {
                Click(neighborVoxelPosition, hitVoxelPosition, 1, 0);
            }
        }
    }

    void ActivateCursors(bool value)
    {
        normalCursor.SetActive(value);
        targetVoxelCursor.SetActive(value);
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

    void Click(Vector3 add, Vector3 remove, int button1, int button2)
    {
        if (Input.GetMouseButtonDown(button1))
        {
            tree.RemoveVoxel(remove);
            tree.CubicQuery(remove);

            blockBreak.transform.position = remove;
            blockBreak.Play();
        }
        //Removing a voxel (by adding one)
        else if (Input.GetMouseButtonDown(button2))
        {
            tree.InsertVoxel(new Voxel(add, Voxel.VoxelType.Stone));
            tree.CubicQuery(add);

            blockBreak.transform.position = add;
            blockBreak.Play();
        }
    }
}
