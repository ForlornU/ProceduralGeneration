using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    private readonly OctreeNode root;

    public Octree(Bounds worldBounds)
    {
        root = new OctreeNode(worldBounds, 0, this);
    }

    #region BoundingBox Drawing
    public List<Bounds> getAllBounds()
    {
        List<Bounds> bounds = new List<Bounds>();
        GatherBoundsRecursively(root, bounds);
        return bounds;
    }
    private void GatherBoundsRecursively(OctreeNode node, List<Bounds> boundsList)
    {
        if (node == null)
            return;

        boundsList.Add(node.bounds);

        if (node.Children == null)
            return;

        if (node.Children.Length > 0)
        {
            foreach (var child in node.Children)
            {
                GatherBoundsRecursively(child, boundsList); // Recursively gather from children
            }
        }
    }
    #endregion

    //Render the entire tree
    public void DrawAllNodes()
    {
        root.Draw();
    }

    public void InsertVoxel(Voxel voxel)
    {
        root.Insert(voxel);
    }

    internal void RemoveVoxel(Vector3 pos)
    {
        root.RemoveAt(pos);
    }

    public void InsertVoxelRange(List<Voxel> voxels)
    {
        foreach (Voxel voxel in voxels)
            InsertVoxel(voxel);
    }

    //Finds overlapping octants
    public void CubicQuery(Vector3 position)
    {
        Bounds queryBounds = new Bounds(position, new Vector3(3, 3, 3));

        //Find all the affected meshes
        List<OctreeMesh> list = new List<OctreeMesh>();

        // Start the query from the root node
        QueryCubicRange(queryBounds, root, list);

        foreach (OctreeMesh mesh in list)
        {
            mesh.DrawSection();
        }
    }

    private void QueryCubicRange(Bounds queryBounds, OctreeNode node, List<OctreeMesh> meshes)
    {

        if (!node.bounds.Intersects(queryBounds))
        {
            return;
        }
        else
        {

            // If it's a leaf node, check the voxel and update mesh if needed
            if (node.IsLeaf && node.mesh != null && node.mesh.drawn)
            {
                meshes.Add(node.mesh);
            }
            else
            {
                // Recursively query child nodes that might be within the range
                for (int i = 0; i < 8; i++)
                {
                    QueryCubicRange(queryBounds, node.Children[i], meshes);
                }
            }
        }
    }

    public bool FindVoxel(Vector3 position, out Voxel foundVoxel)
    {
        if(root.Find(position, out foundVoxel))
            return true;
        else
            return false;
    }

    public bool VoxelAtPos(Vector3 pos)
    {
        if(root.Find(pos, out Voxel v))
            return true;
        else
            return false;
    }

    public void Clear()
    {
        root.Clear();
    }

    //Dynamically grow or shrink the tree, need to redraw entire tree?
    public void Grow()
    {
        //List<Voxel> voxels = new List<Voxel>();
        //GetAllVoxels(root, voxels);

        //Destroy all old nodes
        //Create new root with double the bounds
        //Move bounds center to centre of generation
        //Insert voxels into new octree
        //Redraw entire octree
    }

    public void Shrink()
    {
        /*
         * Recreate the tree with a smaller size
         */
    }
}
