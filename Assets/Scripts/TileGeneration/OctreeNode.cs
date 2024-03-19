using System.Collections.Generic;
using UnityEngine;

public class OctreeNode
{
    int depth;
    public Bounds bounds { get; private set; }
    public bool IsLeaf { get; private set; }
    public Dictionary<Vector3, Voxel> voxels;
    public OctreeNode[] Children { get; private set; }
    int capacity;
    const float minBoundsSize = 7.0f;
    public OctreeMesh mesh;
    const int maxCapacity = 5000;
    Octree tree;
    bool invertNormals;

    public OctreeNode(Bounds bounds, int depth, Octree tree, bool invertNormals)
    {
        mesh = World.CreateNodeMesh().GetComponent<OctreeMesh>();
        mesh.name = mesh.name + "_" + depth;

        this.bounds = bounds;

        IsLeaf = true;
        Children = null;
        voxels = new Dictionary<Vector3, Voxel>();
        capacity = (int)Mathf.Clamp((bounds.size.x * bounds.size.x * bounds.size.x) / 3, minBoundsSize, maxCapacity);
        //Debug.Log("New quadrant with size : " + capacity);
        this.depth = depth;
        this.tree = tree;
        this.invertNormals = invertNormals;
    }

    bool CanSubdivide()
    {
        return (bounds.extents.x > minBoundsSize);
    }

    private bool ShouldSubdivide()
    {
        return voxels.Count >= capacity;
    }

    private void Subdivide()
    {
        // Calculate child node bounds based on parent and octant index
        float halfSize = bounds.extents.x;
        float quarterSize = bounds.extents.x / 2.0f;

        IsLeaf = false;
        Children = new OctreeNode[8];

        // Create child nodes
        for (int i = 0; i < 8; i++)
        {
            Vector3 childCenter = bounds.center;

            childCenter.x += (i & 4) > 0 ? quarterSize : -quarterSize;
            childCenter.y += (i & 2) > 0 ? quarterSize : -quarterSize;
            childCenter.z += (i & 1) > 0 ? quarterSize : -quarterSize;

            Children[i] = new OctreeNode(new Bounds(childCenter, new Vector3(halfSize, halfSize, halfSize)), depth + 1, tree, invertNormals);
        }

        // Redistribute existing voxels among child nodes
        foreach (Voxel voxel in voxels.Values)
        {
            int childIndex = GetOctantIndex(voxel.position);
            Children[childIndex].Insert(voxel);
        }

        // Clear the voxels collection of the parent node
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
            tree.Grow();
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

    public void RemoveAt(Vector3 position)
    {
        if (IsLeaf)
        {
            if (voxels.ContainsKey(position))
            {
                voxels.Remove(position);
            }
        }
        else
        {
            int childIndex = GetOctantIndex(position);
            Children[childIndex].RemoveAt(position);
        }

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

    //public List<Voxel> Query(Bounds range)
    //{
    //    List<Voxel> voxels = new List<Voxel>();
    //    if (bounds.Intersects(range))
    //    {
    //        if (IsLeaf)
    //        {
    //            //if (Voxel != null && range.Contains(Voxel.Position))
    //            //{
    //            //    voxels.Add(Voxel);
    //            //}
    //        }
    //        else
    //        {
    //            for (int i = 0; i < 8; i++)
    //            {
    //                if (Children[i].bounds.Intersects(range))
    //                {
    //                    voxels.AddRange(Children[i].Query(range));
    //                }
    //            }
    //        }
    //    }
    //    return voxels;
    //}

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
        if (IsLeaf)
        {
            mesh.DrawVoxels(voxels, invertNormals);
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
