using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    private readonly OctreeNode root;

    //Test, draw the octree
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
    //EndTest

    //Render all the voxels
    public void DrawAllNodes()
    {
        root.Draw();
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

    public bool FindVoxel(Vector3 position, out Voxel foundVoxel)
    {
        //Voxel foundVoxel = new Voxel();
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

    //public bool ValidVoxelPlacement(Vector3 pos)
    //{

    //}

    //public Voxel GetRandomVoxel()
    //{
    //    OctreeNode node = root.Children[Random.Range(0, root.Children.Length - 1)];
    //    return node.voxels[Vector3.one];
    //    //return node.voxels.Values[Random.Range(0, root.voxels.Values.Count-1)];
    //}

    public List<Voxel> QueryRange(Bounds range)
    {
        return root.Query(range);
    }

    public void Clear()
    {
        root.Clear();
    }
}
