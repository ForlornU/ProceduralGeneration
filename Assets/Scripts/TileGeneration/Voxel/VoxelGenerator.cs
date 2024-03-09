using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Voxel;

public class VoxelGenerator : MonoBehaviour
{
    [SerializeField] Transform voxelWalker; 
    VoxelHash chunk;
    [SerializeField] GenerationSettings settings;
    List<Vector3> previousPositions = new List<Vector3>();

    // Runtime variables
    private Voxel currentVoxel;
    [SerializeField] Transform Player;

    public bool canSpawn { get { return chunk.voxels.Count < settings.maxVoxels; } }

    private void Start()
    {
        if (chunk == null)
            chunk = new GameObject("SpatialHash").AddComponent<VoxelHash>();

        Player.gameObject.SetActive(false);
    }

    public void StartGeneration()
    {
        Clear();
        StartCoroutine(Generate());
    }

    public List<Vector3Int> GetPositions()
    {
        List<Vector3Int> vector3IntList = new List<Vector3Int>();
        foreach (Vector3 pos in chunk.voxels.Keys)
        {
            vector3IntList.Add(Vector3Int.FloorToInt(pos));
        }
        return vector3IntList;
    }

    IEnumerator Generate()
    {
        //Create a 9x9 square starting point
        currentVoxel = new Voxel(Vector3.zero, VoxelType.Stone); // Assuming a tunnel type
        AddVoxel(currentVoxel);
        chunk.CreateNeighbors(currentVoxel, 0f, true);
        int failCounter = 0;

        while (canSpawn)
        {
            // Perform random walk step
            Vector3 newDirection = RandomWalkDirection();
            Vector3 newPosition = currentVoxel.position + newDirection;

            // Check if new position is within walkable area and not visited
            if (IsWalkable(newPosition))
            {
                currentVoxel = new Voxel(newPosition, VoxelType.Stone);
                AddVoxel(currentVoxel);
                voxelWalker.position = currentVoxel.position;
            }
            else
            {
                failCounter++;
                if (failCounter >= 5)
                {
                    failCounter = 0;
                    currentVoxel = chunk.GetVoxelAtPos(newPosition);
                }
            }

            yield return new WaitForSeconds(settings.creationSpeed);
        }

        Inflate();

        previousPositions.Clear();
        chunk.DrawVoxels(settings.inwardsNormals, settings.material);
        Invoke("SetPlayer", 2f);
    }

    private void SetPlayer()
    {
        Player.gameObject.SetActive(true);
        Player.position = Vector3.zero;
    }

    void Inflate()
    {
        for (int i = 0; i < settings.inflationPasses; i++)
        {
            if (previousPositions.Count < 1)
                previousPositions.AddRange(chunk.voxels.Keys);

            foreach (Vector3 v in previousPositions)
            {
                Voxel voxel = new Voxel(v, VoxelType.Stone);
                chunk.CreateNeighbors(voxel, settings.noise);
            }

            previousPositions.Clear();
        }
    }

    private void AddVoxel(Voxel v)
    {
        if(chunk.AddVoxel(v)) //This is a bool return type
            previousPositions.Add(v.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (Vector3 pos in previousPositions)
        {
            Gizmos.DrawCube(pos, Vector3.one);
        }
    }

    private Vector3 RandomWalkDirection()
    {
        int d = Random.Range(0,6);
        Vector3 result = Vector3.forward;

        switch (d)
        {
            case 0: result = Vector3.right; break;
            case 1: result = Vector3.up; break;
            case 2: result = Vector3.down; break;
            case 3: result = Vector3.left; break;
            case 4: result = Vector3.forward; break;
            case 5: result = Vector3.back; break;
        }

        return result;
    }

    private bool IsWalkable(Vector3 position)
    {
        if(chunk.voxels.ContainsKey(position)) 
            return false;
        else
            return true;
    }

    public void Clear()
    {
        StopAllCoroutines();
        chunk.Clear();
        currentVoxel = new Voxel();
        voxelWalker.position = Vector3.zero;
    }
}
