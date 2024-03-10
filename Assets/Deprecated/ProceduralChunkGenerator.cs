using System.Collections.Generic;
using UnityEngine;

public class ProceduralChunkGenerator : MonoBehaviour
{
    [SerializeField] World world;
    [SerializeField] VoxelGenerator voxelGenerator;
    [SerializeField] int chunkSize; // Assuming same size for both cell and voxel chunks

    //private List<Vector3Int> tunnelPositions; // Stores generated tunnel positions

    //public void GenerateChunk(Vector3 chunkPosition)
    //{
    //    voxelGenerator.StartGeneration(); // Initiate random walk generation

    //    // Wait for random walk to finish (consider using events/callbacks)
    //    while (!voxelGenerator.canSpawn) { }

    //    tunnelPositions = voxelGenerator.GetPositions(); // Get generated tunnel positions
    //    CreateMeshFromVoxels(chunkPosition); // Generate mesh based on tunnel positions
    //}

    //private void CreateMeshFromVoxels(Vector3 chunkPosition)
    //{
    //    // Initialize voxel data for this chunk
    //    bool[,,] isActiveVoxels = new bool[chunkSize, chunkSize, chunkSize];

    //    // Mark voxels based on tunnel positions
    //    foreach (Vector3Int position in tunnelPositions)
    //    {
    //        int x = Mathf.Clamp(position.x, 0, chunkSize - 1); // Clamp to chunk boundaries
    //        int y = Mathf.Clamp(position.y, 0, chunkSize - 1);
    //        int z = Mathf.Clamp(position.z, 0, chunkSize - 1);
    //        isActiveVoxels[x, y, z] = true;
    //    }

    //    //// Create a new Chunk object (consider object pooling)
    //    //Chunk chunk = world.CreateChunk(chunkPosition, chunkSize);

    //    //// Assign voxel activity data to the chunk (might need modifications in Chunk class)
    //    //chunk.SetVoxelActivity(isActiveVoxels);

    //    //// Call the original Chunk.GenerateMesh() to process and generate the final mesh
    //    //chunk.GenerateMesh();
    //}
}
