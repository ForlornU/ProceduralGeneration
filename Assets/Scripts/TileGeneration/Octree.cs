using System;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    private readonly OctreeNode root;

    public Octree(Bounds worldBounds)
    {
        root = new OctreeNode(worldBounds, 0, this);
    }

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

    public void InsertVoxel(Voxel voxel)
    {
        root.Insert(voxel);
    }

    internal void RemoveVoxel(Vector3 pos)
    {
        root.RemoveAt(pos);
        //DrawAllNodes();
    }
    //public void InsertAndDraw(Voxel voxel)
    //{
    //    root.Insert(voxel);
    //    //Draw the surrounding area?
    //}

    public void InsertVoxelRange(List<Voxel> voxels)
    {
        foreach (Voxel voxel in voxels)
            InsertVoxel(voxel);
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

    public List<Voxel> QueryRange(Bounds range)
    {
        return root.Query(range);
    }

    public void Clear()
    {
        root.Clear();
    }

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

    void GetAllVoxels(OctreeNode node, List<Voxel> newVoxels)
    {
        if (node == null)
            return;

        newVoxels.AddRange(node.voxels.Values);

        foreach (var child in node.Children)
        {
            GetAllVoxels(child, newVoxels);
        }
        
    }


}
