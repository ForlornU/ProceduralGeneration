using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    private readonly OctreeNode root;

    //Test
    public List<Bounds> getAllBounds()
    {
        List<Bounds> bounds = new List<Bounds>();
        GatherBoundsRecursively(root, bounds);
        return bounds;
    }
    private void GatherBoundsRecursively(OctreeNode node, List<Bounds> boundsList)
    {
        if (node.IsLeaf)
        {
            return; // Stop recursion for leaf nodes
        }

        boundsList.Add(node.bounds);

        foreach (var child in node.Children)
        {
            GatherBoundsRecursively(child, boundsList); // Recursively gather from children
        }
    }

    public Octree(Bounds worldBounds)
    {
        root = new OctreeNode(worldBounds);
    }

    public void InsertVoxel(Voxel voxel)
    {
        root.Insert(voxel);
    }

    public void InsertVoxelRange(List<Voxel> voxels)
    {
        foreach (Voxel voxel in voxels)
            InsertVoxel(voxel);
    }

    public Voxel FindVoxel(Vector3 position)
    {
        return root.Find(position);
    }

    public bool VoxelAtPos(Vector3 pos)
    {
        //The pos in a voxel is probably 0,0,0 and not the position we are looking for
        Voxel v = root.Find(pos);
        if(v.position == pos) 
            return true;

        return false;
    }

    public Voxel GetRandomVoxel()
    {
        OctreeNode node = root.Children[Random.Range(0, root.Children.Length - 1)];
        return node.voxels[Vector3.one];
        //return node.voxels.Values[Random.Range(0, root.voxels.Values.Count-1)];
    }

    public List<Voxel> QueryRange(Bounds range)
    {
        return root.Query(range);
    }

    public void Clear()
    {
        root.Clear();
    }
}
