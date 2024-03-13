using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Voxel;

public class VoxelGenerator : MonoBehaviour
{
    [SerializeField] Transform voxelWalker;
    [SerializeField] GenerationSettings settings;
    [SerializeField] Transform Player;
    World world;
    Octree tree;

    // Runtime variables
    private Voxel currentVoxel;
    List<Vector3> previousPositions = new List<Vector3>();
    public bool canSpawn { get { return previousPositions.Count < settings.maxVoxels; } }

    private void Start()
    {
        if (world == null)
            world = new GameObject("World").AddComponent<World>();

        world.InitOctoTree();
        tree = world.treeReference;

        Player.gameObject.SetActive(false);
    }

    public void StartGeneration()
    {
        Clear();
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        Vector3 veryFirstVoxelPosition = new Vector3(0.5f,0.5f,0.5f);
        CreateNeighbors(veryFirstVoxelPosition, true, true);         //Create a 9x9 square starting point
        currentVoxel.position = veryFirstVoxelPosition;
        int failCounter = 0;

        while (canSpawn)
        {
            // Perform random walk step
            Vector3 newDirection = RandomWalkDirection();
            Vector3 newPosition = currentVoxel.position + newDirection;
            if (!tree.VoxelAtPos(newPosition))
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
                    tree.FindVoxel(previousPositions[Random.Range(0, previousPositions.Count - 1)], out currentVoxel);
                }
            }

            yield return new WaitForSeconds(settings.creationSpeed);
        }

        Inflate();
        world.DrawWorld();

        //previousPositions.Clear();
        Invoke("SetPlayer", 2f);
    }

    private void SetPlayer()
    {
        Player.gameObject.SetActive(true);
        Player.position = new Vector3(0.5f, 1f, 0.5f);
        Player.GetComponent<PlayerController>().ResetMovement();
    }

    void Inflate()
    {
        for (int i = 0; i < settings.inflationPasses; i++)
        {
            foreach (Vector3 pos in previousPositions)
            {
                CreateNeighbors(pos, false);
            }
        }
    }

    public void CreateNeighbors(Vector3 centre, bool addToPreviousLocations, bool forceCubic = false)
    {
        int radius = 2;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3 pos = centre + new Vector3(x, y, z);

                    if (tree.VoxelAtPos(pos))
                        continue;

                    if (!forceCubic)
                    {
                        if (Random.Range(0f, 1f) < settings.noise)
                        {
                            if (IsDiagonalOrCenter(new Vector3(x, y, z)))
                            {
                                continue;
                            }
                        }
                    }

                    if (addToPreviousLocations)
                        AddVoxel(new Voxel(pos, VoxelType.Stone));
                    else
                        tree.InsertVoxel(new Voxel(pos, VoxelType.Stone));
                }
            }
        }
    }
    bool IsDiagonalOrCenter(Vector3 pos)
    {
        return pos.x * pos.z != 0 || pos.y * pos.z != 0 || pos.x * pos.y != 0 || (pos.x == 0 && pos.y == 0 && pos.z == 0);
    }

    private void AddVoxel(Voxel v)
    {
        tree.InsertVoxel(v);
        previousPositions.Add(v.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (Vector3 pos in previousPositions)
        {
            //Gizmos.DrawCube(pos, Vector3.one);
            Gizmos.DrawWireCube(pos, Vector3.one);
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

    public void Clear()
    {
        StopAllCoroutines();
        world.ClearTree();
        previousPositions.Clear();
        currentVoxel = new Voxel();
        voxelWalker.position = Vector3.one;
        Player.gameObject.SetActive(false);
    }
}
