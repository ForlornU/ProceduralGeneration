using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Voxel;

public class VoxelGenerator : MonoBehaviour
{
    [SerializeField] Transform voxelWalker; 
    World world;
    VoxelHash currentChunk;
    [SerializeField] GenerationSettings settings;
    List<Vector3> previousPositions = new List<Vector3>();

    // Runtime variables
    private Voxel currentVoxel;
    [SerializeField] Transform Player;

    public bool canSpawn { get { return previousPositions.Count < settings.maxVoxels; } }

    private void Start()
    {
        if(world == null)
            world = new GameObject("World").AddComponent<World>();

        world.InitOctoTree();

        if (currentChunk == null)
            currentChunk = world.GetChunk(0); //This generates a new chunk

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
        foreach (Vector3 pos in currentChunk.voxels.Keys)
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
        currentChunk.CreateNeighbors(currentVoxel, 0f, true);
        int failCounter = 0;

        while (canSpawn)
        {
            // Perform random walk step
            Vector3 newDirection = RandomWalkDirection();
            Vector3 newPosition = currentVoxel.position + newDirection;

            // Check if new position is within walkable area and not visited
            if (!world.isInTree(newPosition)) //IsWalkable(newPosition))
            {
                currentVoxel = new Voxel(newPosition, VoxelType.Stone);
                AddVoxel(currentVoxel);
                voxelWalker.position = currentVoxel.position;
                failCounter = 0;
            }
            else
            {
                failCounter++;
                if (failCounter >= 5)
                {
                    failCounter = 0;
                    currentVoxel = world.FindInTree(previousPositions[Random.Range(0, previousPositions.Count - 1)]);
                    //currentVoxel = world.tre//world.RandomFromTree();
                    //currentChunk.GetVoxelAtPos(newPosition, out currentVoxel);
                }
            }
            //Create new hash when full
            //if (currentChunk.voxels.Count >= world.maxVoxels)
            //{
            //    currentChunk = world.GetChunk(currentChunk.hash + 1);
            //}

            yield return new WaitForSeconds(settings.creationSpeed);
        }

        Inflate();
        Draw();

        previousPositions.Clear();
        //currentChunk.DrawVoxels(settings.inwardsNormals, settings.material);
        Invoke("SetPlayer", 2f);
    }

    //private void SwitchChunk()
    //{
    //    //currentChunk.DrawVoxels(settings.inwardsNormals, settings.material);
    //    currentChunk = world.GetChunk(currentChunk.hash + 1);
    //}

    private void SetPlayer()
    {
        Player.gameObject.SetActive(true);
        Player.position = Vector3.zero;
        Player.GetComponent<PlayerController>().ResetMovement();
    }

    void Draw()
    {
        foreach(var chunk in world.chunks.Values)
        {
            chunk.DrawVoxels(settings.inwardsNormals, settings.material);
        }
        //currentChunk.DrawVoxels(settings.inwardsNormals, settings.material);
    }

    void Inflate()
    {
        List<Vector3> newNeighborPositions = new List<Vector3>();
        for (int i = 0; i < settings.inflationPasses; i++)
        {
            foreach (var chunk in world.chunks.Values)
            {
                newNeighborPositions.AddRange(chunk.voxels.Keys);

                foreach (var voxel in newNeighborPositions)
                {
                    Voxel newV = new Voxel(voxel, VoxelType.Stone);
                    chunk.CreateNeighbors(newV, settings.noise);
                }

                newNeighborPositions.Clear();
            }
        }

    }

            //if (previousPositions.Count < 1)
            //    previousPositions.AddRange(currentChunk.voxels.Keys);

            //foreach (Vector3 v in previousPositions)
            //{
            //    Voxel voxel = new Voxel(v, VoxelType.Stone);
            //    currentChunk.CreateNeighbors(voxel, settings.noise);
            //}

            //previousPositions.Clear();
    //    }
    //}

    private void AddVoxel(Voxel v)
    {
        //if(currentChunk.AddVoxel(v)) //This is a bool return type
        //if (!world.FindInTree(v.position))
        //{
            world.addToTree(v);
            previousPositions.Add(v.position);
        //}
        //else
        //    Debug.Log("Occupied");
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
        if(currentChunk.voxels.ContainsKey(position)) 
            return false;
        else
            return true;
    }

    public void Clear()
    {
        StopAllCoroutines();
        world.ClearTree();
        //currentChunk.Clear();
        previousPositions.Clear();
        currentVoxel = new Voxel();
        voxelWalker.position = Vector3.zero;
        Player.gameObject.SetActive(false);
    }
}
