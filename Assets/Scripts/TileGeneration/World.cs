using UnityEngine;

public class World : MonoBehaviour
{
    Octree tree;
    public Octree treeReference => tree;

    public static World Instance { get; private set; }
    public Material globalTestMaterial;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        globalTestMaterial = Resources.Load("test") as Material;
    }

    public static GameObject CreateNodeMesh()
    {
        GameObject meshGO = new GameObject("Mesh");
        meshGO.AddComponent<OctreeMesh>();
        return meshGO;
    }

    public static void DestroyNodeMesh(OctreeMesh mesh)
    {
        Destroy(mesh.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (tree != null)
        {
            foreach (var item in tree.getAllBounds())
            {
                Gizmos.DrawWireCube(item.center, item.size);
                Gizmos.DrawSphere(item.center, 0.5f);
            }
        }
    }

    public void DrawWorld()
    {
        tree.DrawAllNodes();
    }

    public void InitOctoTree()
    {
        //Bounds must always be even
        int boundsSize = 352;
        if (boundsSize % 2 != 0)
            boundsSize++;

        if (tree == null)
            tree = new Octree(new Bounds(Vector3.zero, Vector3.one * boundsSize));
    }

    public bool FindInTree(Vector3 pos, out Voxel voxel)
    {
        return tree.FindVoxel(pos, out voxel);
    }

    public void ClearTree()
    {
        tree.Clear();
    }

}