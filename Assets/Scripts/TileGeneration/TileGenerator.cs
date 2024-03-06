using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneratorUI))]
public class TileGenerator : MonoBehaviour
{
    [SerializeField] Transform walker;
    [SerializeField] Transform connectorWalker;
    [SerializeField] GameObject debugCube;
    [SerializeField] GenerationSettings settings;
    int passIndex = 0;

    //Dependencies
    GeneratorUI UI;
    TileDatabase tileDatabase;
    GeneratorAutomota automata;

    //Collections
    List<Connector> connectorsToSpawn = new List<Connector>();
    List<Tile> spawnedTiles = new List<Tile>();
    //Remove later
    List<Vector3> cellPositions = new List<Vector3>();
    List<GameObject> debugCubes = new List<GameObject>();

    //Runtime
    [SerializeField] SpatialHash grid;
    Connector currentConnector;
    Tile lastGeneratedTile;
    int generatedTiles = 0;

    public bool canSpawn { get { return connectorsToSpawn.Count > 0 && generatedTiles < settings.Passes[passIndex].tileCount; } }

    private void Start()
    {
        UI = GetComponent<GeneratorUI>();
        tileDatabase = new TileDatabase();
        automata = transform.GetChild(0).GetComponent<GeneratorAutomota>();
        automata.Init();
    }

    private void Update()
    {
        UI.WriteToUI(connectorsToSpawn.Count, generatedTiles);
    }

    public void StartGeneration()
    {
        InitStart();
        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        //Grid creation pass?
        for (int i = 0; i < settings.Passes.Length; i++)
        {
            PassSettings pass = settings.Passes[i];
            automata.ChangeModule(pass.modulename);
            generatedTiles = 0;
            passIndex = i;

            if (pass.isInstant)
                GenerateInstantly();
            else
                yield return StartCoroutine(GenerateOverTime());
        }
        passIndex = 0;

        //Everything after this is a test, we remove the inner structure of the generation, the "void"
        //Removal of inner void
        Debug.Log($"There are {grid.cells.Count} cells!, lets remove all occupied ones!");
        List<Vector3> posToRemove = new List<Vector3>();
        foreach (Vector3 pos in grid.cells.Keys)
        {
            if (grid.GetCellAtPos(pos).isOccupied)
                posToRemove.Add(pos);
        }
        foreach (Vector3 pos in posToRemove)
        {
            Tile t = grid.GetCellAtPos(pos).occupyingTile;
            t.parentCell.markedAsVoid = true;
            Destroy(t.gameObject);
            grid.RemoveAtPos(pos);
        }

        //Outlying walls pass?
        Debug.Log($"There are {grid.cells.Count} cells left!!");
        foreach (Vector3 pos in grid.cells.Keys)
        {
            Cell c = grid.GetCellAtPos(pos);
            if (c.isOccupied || c.markedAsVoid)
                continue;
            GameObject cube = Instantiate(debugCube, pos, Quaternion.identity);
            debugCubes.Add(cube);
        }


    }

    private void InitStart()
    {
        GameObject firstTile = Instantiate(settings.startTile);
        lastGeneratedTile = firstTile.GetComponent<Tile>();
        lastGeneratedTile.Init();

        connectorsToSpawn.AddRange(lastGeneratedTile.connectors);

        grid.Init(firstTile.GetComponent<Collider>().bounds.extents.x);
        grid.AddTileToGrid(Vector3.zero, firstTile.GetComponent<Tile>());
    }

    private ModuleReferenceData UpdateModuleData(ModuleReferenceData d)
    {
        //d.walkerPosition = walker.position;
        d.walkerPosition = connectorWalker.position;
        d.lastTile = lastGeneratedTile;
        d.connectors = connectorsToSpawn;
        return d;
    }

    private void GenerateInstantly()
    {
        int connectorIndex = 0;
        ModuleReferenceData newData = new ModuleReferenceData();
        newData.connectorsIndex = connectorIndex;
        GameObject matchingTile = null;

        do
        {
            newData = UpdateModuleData(newData);
            connectorIndex = automata.currentModule.Sort(newData);

            if (!canProcessConnector(connectorIndex) || hasMatchingTile(out matchingTile))
                    continue;

            Tile t = CreateTile(matchingTile);
            connectorsToSpawn.AddRange(t.connectors);

            MoveTileToPosition(t.gameObject);
            grid.AddTileToGrid(t.transform.position, t); //assumes its already been moved into pos
            ConnectToSurroundingGrid(t);
            PositionWalkers(t);
        }
        while (canSpawn);

        UI.SetDataText(connectorsToSpawn.Count, generatedTiles);
    }

    IEnumerator GenerateOverTime()
    {
        UI.StartSession();
        int connectorIndex = 0;
        ModuleReferenceData newData = new ModuleReferenceData();
        newData.connectorsIndex = connectorIndex;

        do
        {
            newData = UpdateModuleData(newData);
            connectorIndex = automata.currentModule.Sort(newData);

            if (!canProcessConnector(connectorIndex) || !hasMatchingTile(out GameObject matchingTile))
                continue;

            Tile t = CreateTile(matchingTile);
            connectorsToSpawn.AddRange(t.connectors);

            MoveTileToPosition(t.gameObject);
            grid.AddTileToGrid(t.transform.position, t); //Move first, add second
            ConnectToSurroundingGrid(t);
            PositionWalkers(t);

            yield return new WaitForSeconds(settings.Passes[passIndex].creationspeed);
        }
        while (canSpawn);

        UI.StopSession();
    }

    private void PositionWalkers(Tile t)
    {
        walker.position = t.transform.position;
        connectorWalker.position = currentConnector.transform.position;
    }

    public void Clear()
    {
        UI.ClearSession();
        StopAllCoroutines();

        if (spawnedTiles.Count > 0)
        {
            foreach (Tile t in spawnedTiles)
            {
                if(t.parentCell == null)
                {
                    Debug.Log("Tile had no cell to destroy");
                    continue;
                }

                Destroy(t.gameObject);
                grid.ResetGrid();
            }
            spawnedTiles.Clear();
        }

        foreach(GameObject c in debugCubes)
        {
            Destroy(c);
        }
        debugCubes.Clear();
        cellPositions.Clear();

        generatedTiles = 0;
        connectorsToSpawn.Clear();
        currentConnector = null;

        walker.position = Vector3.zero;
        connectorWalker.position = Vector3.zero;
    }

    bool canProcessConnector(int index)
    {
        if (index < 0 || index > connectorsToSpawn.Count-1)
        {
            Debug.Log("Serious issue, index less than zero");
            return false;
        }

        Connector foundConnector = connectorsToSpawn[index];
        connectorsToSpawn.RemoveAt(index);

        Vector3 dir = ConnectorDirToNewCell(foundConnector);
        Cell cellAtPos = grid.GetCellAtPos(foundConnector.transform.position + dir);

        if (foundConnector == null || foundConnector.isOccupied || cellAtPos == null || cellAtPos.isOccupied)
        {
            //Debug.Log($"Error = Found connector is at: {foundConnector.transform.position}, dir is: {dir}, new pos is = {foundConnector.transform.position + dir}");
            //Debug.Log($"connector is null = {foundConnector == null} " +
            //    $"|| connector is occupied = {foundConnector.isOccupied}" +
            //    $"|| Pos exists in grid = {grid.GetCellAtPos(foundConnector.transform.position + dir) != null}" +
            //    $"|| cell is occupied = {grid.GetCellAtPos(foundConnector.transform.position + dir).isOccupied}");
            //Debug.Break();
            return false;
        }

        currentConnector = foundConnector;
        return true;
    }

    Vector3 ConnectorDirToNewCell(Connector connector)
    {
        return (connector.transform.position - connector.parentTile.transform.position).normalized * grid.cellDiameter / 2;
    }

    bool hasMatchingTile(out GameObject result)
    {
        result = null;
        if (tileDatabase.tileDictionary.TryGetValue(currentConnector.connectorID, out string[] options))
        {
            if(options.Length == 0)
            {
                Debug.Log("No options for this connector");
                return false;
            }

            int optionIndex = Random.Range(0, options.Length);
            result = Resources.Load(options[optionIndex]) as GameObject;
            return true;
        }
        else
        {
            Debug.Log("No tile with given id : " + currentConnector.connectorID);
            return false;
        }
    }

    Tile CreateTile(GameObject newTileGO)
    {
        newTileGO = Instantiate(newTileGO, currentConnector.transform.position, Quaternion.identity);
        Tile newTile = newTileGO.GetComponent<Tile>();
        newTile.Init();

        lastGeneratedTile = newTile;
        generatedTiles++;
        spawnedTiles.Add(newTile);

        return newTile;
    }

    void ConnectToSurroundingGrid(Tile tile)
    {
        if(tile.parentCell == null)
        {
            Debug.Log("Tile has no cell!");
            return;
        }

        foreach (Cell neighbor in grid.GetNeighbours(tile.parentCell))
        {
            //Add as neighbors?
        }
    }

    private void MoveTileToPosition(GameObject newTile)
    {
        Vector3 dir = currentConnector.parentTile.transform.position - newTile.transform.position;
        newTile.transform.position -= dir;
    }

    //void ConnectClosestConnectorOnNewTile(Tile t, Connector c)
    //{
    //    // Sort connectors based on their distance from c
    //    t.connectors.Sort((x, y) => Vector3.Distance(c.transform.position, x.transform.position).CompareTo(Vector3.Distance(c.transform.position, y.transform.position)));

    //    Connector closestConnector = t.connectors[0];

    //    MatchConnectors(c, closestConnector);
    //}
    //void TempConnectClosestConnectorOnNewTile(Tile neighbor, Tile placedTile)
    //{
    //    if(neighbor == null)
    //        return;



    //    neighbor.connectors.Sort((x, y) => Vector3.Distance(placedTile.transform.position, x.transform.position).CompareTo(Vector3.Distance(placedTile.transform.position, y.transform.position)));

    //    Connector closestConnector = neighbor.connectors[0];

    //    MatchConnectors(currentConnector, closestConnector);
    //}

    //void MatchConnectors(Connector x, Connector y)
    //{
    //    x.isOccupied = true;
    //    connectorsToSpawn.Remove(x);

    //    y.isOccupied = true;
    //    connectorsToSpawn.Remove(y);

    //    x.connectedTo = y;
    //    y.connectedTo = x;
    //}
}
