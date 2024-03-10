using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OctreeMesh : MonoBehaviour
{
    Dictionary<Vector3, Voxel> meshVoxels = new Dictionary<Vector3, Voxel>();
    //Drawing
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    //Components
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public void InitMesh()
    {
        
    }

    public void DrawVoxels(Dictionary<Vector3, Voxel> voxels)//bool reverseNormals, Material mat)
    {
        meshVoxels = voxels; //This just points to the same collection, reference

        CheckComponents();
        ProcessVoxels();

        Mesh mesh = new Mesh();
        mesh.name = "VoxelMesh";
        if (vertices.Count > 65536)//mesh.vertexCount > 65536)
        {
            Debug.Log("Verticies above limit! - " + vertices.Count + ". Switching to uint32");
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        //if (reverseNormals)
            mesh.triangles = mesh.triangles.Reverse().ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals(); // Important for lighting

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        meshRenderer.material = World.Instance.globalTestMaterial;
        //meshRenderer.material = mat;//material; //World.Instance.VoxelMaterial;
    }

    private void ProcessVoxels()
    {
        int nrOfFaces = 6;
        foreach (Voxel voxel in meshVoxels.Values)
        {
            if (!voxel.isActive)
                continue;

            bool[] facesVisible = new bool[nrOfFaces];
            for (int faceDir = 0; faceDir < nrOfFaces; faceDir++)
            {
                int offsetX = 0, offsetY = 0, offsetZ = 0;

                // Determine offsets based on face direction
                switch (faceDir)
                {
                    case 0: // Top
                        offsetY = 1;
                        break;
                    case 1: // Bottom
                        offsetY = -1;
                        break;
                    case 2: // Left
                        offsetX = -1;
                        break;
                    case 3: // Right
                        offsetX = 1;
                        break;
                    case 4: // Front
                        offsetZ = 1;
                        break;
                    case 5: // Back
                        offsetZ = -1;
                        break;
                }

                // Calculate neighboring voxel position
                Vector3 neighborPos = new Vector3(voxel.position.x + offsetX, voxel.position.y + offsetY, voxel.position.z + offsetZ);

                // Check if voxel exists and is active
                bool isFaceVisible = IsFaceVisible(neighborPos);//voxels.TryGetValue(neighborPos, out Voxel neighborVoxel) && neighborVoxel.isActive;
                if (!isFaceVisible)
                    AddFaceData((int)voxel.position.x, (int)voxel.position.y, (int)voxel.position.z, faceDir); // Method to add mesh data for the visible face
            }
        }
    }

    private void CheckComponents()
    {
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    private void AddFaceData(int x, int y, int z, int faceIndex)
    {
        // Based on faceIndex, determine vertices and triangles
        // Add vertices and triangles for the visible face
        // Calculate and add corresponding UVs

        if (faceIndex == 0) // Top Face
        {
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 1) // Bottom Face
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x, y, z + 1));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 2) // Left Face
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 3) // Right Face
        {
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 4) // Front Face
        {
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
        }

        if (faceIndex == 5) // Back Face
        {
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));

        }
        AddTriangleIndices();
    }

    private void AddTriangleIndices()
    {
        int vertCount = vertices.Count;

        // First triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 3);
        triangles.Add(vertCount - 2);

        // Second triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 2);
        triangles.Add(vertCount - 1);
    }

    private bool IsFaceVisible(Vector3 pos)
    {
        bool localNeighbor = meshVoxels.TryGetValue(pos, out Voxel neighborVoxel) && neighborVoxel.isActive;
        //bool globalNeighbor = World.Instance.isInTree(pos);
        //Voxel neighbor = World.Instance.FindInTree(pos);
        bool hasGlobalNeighbor;

        if(World.Instance.FindInTree(pos, out Voxel neighbor))
            hasGlobalNeighbor = true;
        else
            hasGlobalNeighbor = false;

        bool globalNeighbor = hasGlobalNeighbor && neighbor.isActive;

        return localNeighbor && globalNeighbor;
    }

    //public void CreateNeighbors(Voxel voxel, float noise, bool forceCubic = false)
    //{
    //    for (int x = -1; x <= 1; x++)
    //    {
    //        for (int y = -1; y <= 1; y++)
    //        {
    //            for (int z = -1; z <= 1; z++)
    //            {
    //                Vector3 pos = voxel.position + new Vector3(x, y, z);

    //                if (meshVoxels.ContainsKey(pos))
    //                    continue;

    //                if (!forceCubic)
    //                {
    //                    if (Random.Range(0f, 1f) < noise)
    //                    {
    //                        if (IsDiagonalOrCenter(new Vector3(x, y, z)))
    //                            continue;
    //                    }
    //                }

    //                //AddSingleVoxel(pos);
    //            }
    //        }
    //    }
    //}

    bool IsDiagonalOrCenter(Vector3 pos)
    {
        return pos.x * pos.z != 0 || pos.y * pos.z != 0 || pos.x * pos.y != 0 || (pos.x == 0 && pos.y == 0 && pos.z == 0);
    }
}
