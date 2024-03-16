using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    public bool drawn = false;

    public void DrawVoxels(Dictionary<Vector3, Voxel> voxels)
    {
        meshVoxels = voxels; //This just points to the same collection, reference

        CheckComponents();
        ClearMesh();
        ProcessVoxels();

        transform.position = new Vector3 (-0.5f, -0.5f, -0.5f); //Account for voxel 0.5f offset, temp fix?
        Mesh mesh = new Mesh();
        mesh.name = "VoxelMesh";
        if (vertices.Count > 65536)//mesh.vertexCount > 65536)
        {
            Debug.Log("Verticies above limit! - " + vertices.Count + ". Switching to uint32");
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.triangles = mesh.triangles.Reverse().ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        meshRenderer.material = World.Instance.globalTestMaterial;
        drawn = true;
    }



    public void DrawSection()
    {

    }

    private void ClearMesh()
    {
        if (meshFilter.mesh)
        {
            meshFilter.mesh.Clear(false); // Clear existing mesh data, preserving vertex layout
        }

        // Reset vertex and triangle collections for new data
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    private void ProcessVoxels()
    {
        int nrOfFaces = 6;
        foreach (Voxel voxel in meshVoxels.Values)
        {
            bool voxelIsAir = false;

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
                {
                    AddFaceData(voxel.position.x, voxel.position.y, voxel.position.z, faceDir);
                    voxelIsAir= true;
                }
                    //AddFaceData((int)voxel.position.x, (int)voxel.position.y, (int)voxel.position.z, faceDir); // Method to add mesh data for the visible face
            }

            //if (voxelIsAir)
            //    voxel.type = Voxel.VoxelType.Air;
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

    //private void AddFaceData(int x, int y, int z, int faceIndex)
    private void AddFaceData(float x, float y, float z, int faceIndex)
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
        bool localNeighbor = meshVoxels.TryGetValue(pos, out Voxel localNeighborVoxel); //&& localNeighborVoxel.isActive;
        bool hasGlobalNeighbor;

        if(World.Instance.FindInTree(pos, out Voxel GlobalNeighbor))
            hasGlobalNeighbor = true;
        else
            hasGlobalNeighbor = false;

        return localNeighbor || hasGlobalNeighbor;
    }

}
