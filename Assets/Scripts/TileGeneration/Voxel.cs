using UnityEngine;

public struct Voxel
{
    public Vector3 position;
    //public bool isActive;
    public VoxelType type;

    public enum VoxelType
    {
        Air,    // Represents empty space
        Stone,  // Represents stone block
                // Add more types as needed
    }

    public Voxel(Vector3 position, VoxelType type) //, bool isActive = true)
    {
        this.position = position;
        this.type = type;
        //this.isActive = isActive;
    }
    //public Voxel(Vector3 position)
    //{
    //    this.position = position;
    //}
}