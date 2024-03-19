using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    public bool canSpawn { get { return previousPositions.Count < settings.voxelsToCreate; } }

    private void Start()
    {
        if (world == null)
            world = new GameObject("World").AddComponent<World>();

        world.InitOctoTree(settings.InsideWorld);
        tree = world.treeReference;

        Player.gameObject.SetActive(false);

        StartGeneration();
    }

    public void StartGeneration()
    {
        Clear();
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        Vector3 veryFirstVoxelPosition = new Vector3(0.5f,0.5f,0.5f);
        CreateNeighbors(veryFirstVoxelPosition, true, settings.startBlockSize, true);         //Create a 9x9 square starting point
        currentVoxel.position = veryFirstVoxelPosition;
        int attempts = 0;

        while (canSpawn)
        {
            // Perform random walk step
            Vector3 newPosition = currentVoxel.position + RandomWalkDirection();
            if (!tree.VoxelAtPos(newPosition))
            {
                currentVoxel = new Voxel(newPosition, VoxelType.Stone);
                AddVoxel(currentVoxel);

                voxelWalker.position = currentVoxel.position;
                attempts = 0;
            }
            else
            {
                attempts++;
                if (attempts >= 5)
                {
                    attempts = 0;
                    tree.FindVoxel(previousPositions[Random.Range(0, previousPositions.Count - 1)], out currentVoxel);
                }
            }

            yield return new WaitForSeconds(settings.creationSpeed);
        }

        Inflate();

        if (settings.InsideWorld)
            PlaceTorchesInside();
        else
            PlaceTorchesOutside();

        world.DrawWorld();
        if(!settings.debug)
            previousPositions.Clear();
        Invoke("SetPlayer", 1f);
        voxelWalker.gameObject.SetActive(false);
    }

    private void SetPlayer()
    {
        Player.gameObject.SetActive(true);
        Player.position = new Vector3(0.5f, 1f, 0.5f);
        Player.GetComponent<PlayerController>().ResetMovement();
    }

    void Inflate()
    {
        foreach (Vector3 pos in previousPositions)
        {
            if (Random.value < settings.noise)
                CreateNeighbors(pos, false, settings.inflationPasses);
        }
    }

    void PlaceTorchesInside()
    {
        int numTorches = Mathf.RoundToInt(previousPositions.Count * settings.torchesDistribution) + 1;
        Vector3 lastPosition = Vector3.zero;

        for (int i = 0; i < numTorches; i++)
        {
            int verticalChange = 10;
            int randomIndex = Random.Range(0, previousPositions.Count - 1);
            Vector3 targetPosition = previousPositions[randomIndex];
            Vector3 floor = targetPosition;
            floor.y -= 1;

            if (Vector3.Distance(targetPosition, lastPosition) < verticalChange*1.5f)
                continue;

            if(settings.debug)
                Instantiate(Resources.Load<GameObject>("TorchStartCube"), targetPosition, Quaternion.identity);

            // Look down until we reach floor
            while (tree.VoxelAtPos(floor))
            {
                floor.y--;
                verticalChange--;
            }

            if (verticalChange < 0) //If we moved too far, skip
                continue;

            targetPosition = floor;
            targetPosition.y++;
            lastPosition = targetPosition;

            Instantiate(Resources.Load<GameObject>("Torch"), targetPosition, Quaternion.identity);
        }
    }

    void PlaceTorchesOutside()
    {
        int numTorches = Mathf.RoundToInt(previousPositions.Count * settings.torchesDistribution) + 1;
        Vector3 lastPosition = Vector3.zero;

        for (int i = 0; i < numTorches; i++)
        {
            int verticalChange = 10;
            int randomIndex = Random.Range(0, previousPositions.Count - 1);
            Vector3 targetPosition = previousPositions[randomIndex];
            Vector3 floor = targetPosition;
            floor.y += verticalChange;

            if (Vector3.Distance(targetPosition, lastPosition) < verticalChange * 1.5f)
                continue;

            if (settings.debug)
                Instantiate(Resources.Load<GameObject>("TorchStartCube"), targetPosition, Quaternion.identity);

            // Look down until we reach floor
            while (!tree.VoxelAtPos(floor))
            {
                floor.y--;
                verticalChange--;
            }

            if (verticalChange < 0) //If we moved too far, skip
                continue;

            targetPosition = floor;
            targetPosition.y++;
            lastPosition = targetPosition;

            Instantiate(Resources.Load<GameObject>("Torch"), targetPosition, Quaternion.identity);
        }
    }

    public void CreateNeighbors(Vector3 centre, bool addToPreviousLocations, int radius, bool forceCubic = false)
    {
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
                        if (Random.value < settings.noise)
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
        int d = Random.Range(0, 6);

        Vector3 result = Vector3.forward;
        Vector3 radial = RadialBias();

        switch (d)
        {
            case 0: result = Vector3.right; break;
            case 1: result = Vector3.up; break;
            case 2: result = Vector3.down; break;
            case 3: result = Vector3.left; break;
            case 4: result = Vector3.forward; break;
            case 5: result = Vector3.back; break;
        }

        if (Random.value < settings.radialBias)
            result = radial;

        return result;
    }

    private Vector3 RadialBias()
    {
        //Bias test
        Vector3 dirFromCenter = (currentVoxel.position - Vector3.zero).normalized;

        float[] dotProducts = new float[6];
        dotProducts[0] = Vector3.Dot(dirFromCenter, Vector3.forward);
        dotProducts[1] = Vector3.Dot(dirFromCenter, Vector3.right);
        dotProducts[2] = Vector3.Dot(dirFromCenter, Vector3.up);
        dotProducts[3] = Vector3.Dot(dirFromCenter, Vector3.left);
        dotProducts[4] = Vector3.Dot(dirFromCenter, Vector3.back);
        dotProducts[5] = Vector3.Dot(dirFromCenter, Vector3.down);

        int closestIndex = Mathf.FloorToInt(Mathf.Max(dotProducts));//Mathf.RoundToInt(Mathf.Max(dotProducts));
        Vector3 closestDirection = closestIndex switch
        {
            0 => Vector3.forward,
            1 => Vector3.right,
            2 => Vector3.up,
            3 => Vector3.left,
            4 => Vector3.back,
            5 => Vector3.down,
            _ => throw new System.NotImplementedException(),
        };

        return closestDirection;
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
