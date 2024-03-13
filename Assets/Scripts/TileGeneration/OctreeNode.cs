using System.Collections.Generic;
using UnityEngine;

public class OctreeNode
{
    public Bounds bounds { get; private set; }
    public bool IsLeaf { get; private set; }
    public Dictionary<Vector3, Voxel> voxels;
    public OctreeNode[] Children { get; private set; }
    int capacity;
    const float minBoundsSize = 2f;
    OctreeMesh mesh;
    const int maxCapacity = 12000;

    public OctreeNode(Bounds bounds)
    {
        mesh = World.CreateNodeMesh().GetComponent<OctreeMesh>();
        this.bounds = bounds;
        IsLeaf = true;
        Children = null;
        voxels = new Dictionary<Vector3, Voxel>();
        capacity = (int)Mathf.Clamp((bounds.size.x * bounds.size.x * bounds.size.x)/3, minBoundsSize, maxCapacity);
        //capacity = (int)(bounds.size.x * bounds.size.x * bounds.size.x) / 3;
        Debug.Log("New quadrant with size : " + capacity);
    }

    bool CanSubdivide()
    {
        float quarterSize = bounds.extents.x / 2.0f;
        return (quarterSize > minBoundsSize);
    }

    private void Subdivide()
    {
        // Calculate child node bounds based on parent and octant index
        float halfSize = bounds.extents.x;
        float quarterSize = bounds.extents.x / 2.0f;
        Vector3 center = bounds.center;

        Debug.Log("Subdivided!");
        IsLeaf = false;
        Children = new OctreeNode[8];

        for (int i = 0; i < 8; i++)
        {
            Vector3 childCenter = new Vector3(
                center.x + (i & 4) > 0 ? quarterSize : -quarterSize, // Correct x-axis offset
                center.y + (i & 2) > 0 ? quarterSize : -quarterSize, // Correct y-axis offset
                  center.z + (i & 1) > 0 ? quarterSize : -quarterSize); // Correct z-axis offset

            Children[i] = new OctreeNode(new Bounds(childCenter, new Vector3(halfSize, halfSize, halfSize)));
        }

        foreach(Voxel voxel in voxels.Values)
        {
            int childIndex = GetOctantIndex(voxel.position);
            Children[childIndex].Insert(voxel);
        }

        voxels.Clear();
    }

    private int GetOctantIndex(Vector3 position)
    {
        int index = 0;
        if (position.x > bounds.center.x) index |= 4;
        if (position.y > bounds.center.y) index |= 2;
        if (position.z > bounds.center.z) index |= 1;
        return index;
    }

    public void Insert(Voxel voxel)
    {
        if (!bounds.Contains(voxel.position))
        {
            return;
        }

        if (IsLeaf)
        {
            if (ShouldSubdivide() && CanSubdivide())
            {
                Subdivide();
                Insert(voxel);
            }
            else
            {
                voxels.TryAdd(voxel.position, voxel);
            }
        }
        else
        {
            //Find leaf
            int childIndex = GetOctantIndex(voxel.position);
            Children[childIndex].Insert(voxel);
        }
    }

    private bool ShouldSubdivide()
    {
        return voxels.Count >= capacity;
    }

    public bool Find(Vector3 position, out Voxel foundVoxel)
    {
        foundVoxel = new Voxel();
        bool found = false;
        if (!bounds.Contains(position))
        {
            return false;
        }

        if (IsLeaf)
        {
            if (voxels.ContainsKey(position))
            {
                foundVoxel = voxels[position];
                found = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            int childIndex = GetOctantIndex(position);
            found = Children[childIndex].Find(position, out foundVoxel);
        }

        return found;
    }

    public List<Voxel> Query(Bounds range)
    {
        List<Voxel> voxels = new List<Voxel>();
        //if (bounds.Intersects(range))
        //{
        //    if (IsLeaf)
        //    {
        //        if (Voxel != null && range.Contains(Voxel.Position))
        //        {
        //            voxels.Add(Voxel);
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < 8; i++)
        //        {
        //            if (Children[i].bounds.Intersects(range))
        //            {
        //                voxels.AddRange(Children[i].Query(range));
        //            }
        //        }
        //    }
        //}
        return voxels;
    }

    public void Clear()
    {
        if (IsLeaf)
        {
            voxels.Clear();
            //World.DestroyNodeMesh(mesh);
            //Redraw entire octree?
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                Children[i].Clear();
            }
        }
    }

    //Drawing
    public void Draw()
    {
        if(IsLeaf)
        {
            mesh.DrawVoxels(voxels);
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                Children[i].Draw();
            }
        }
    }


}
