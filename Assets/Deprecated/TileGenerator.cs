//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(GeneratorUI))]
//public class TileGenerator : MonoBehaviour
//{
//    [SerializeField] World world;
//    [SerializeField] GenerationSettings settings;

//    //Dependencies
//    GeneratorUI UI;
//    GeneratorAutomota automata;

//    //Runtime
//    [SerializeField] SpatialHash grid;
//    Tile lastGeneratedTile;
//    int generatedVoxels = 0;
//    int passIndex = 0;

//    public bool canSpawn { get { return generatedVoxels < settings.passes[passIndex].maxCount; } }

//    private void Start()
//    {
//        UI = GetComponent<GeneratorUI>();
//        automata = transform.GetChild(0).GetComponent<GeneratorAutomota>();
//        automata.Init();
//    }

//    public void StartGeneration()
//    {
//        world.CreateChunk(Vector3.zero);
//        InitStart();
//        StartCoroutine(Generate());
//    }

//    public IEnumerator Generate()
//    {
//        generatedVoxels = 0;
//        passIndex = 0;

//        //Create Inner
//        for (int i = 0; i < settings.passes.Length; i++)
//        {
//            PassSettings pass = settings.passes[i];
//            automata.ChangeModule(pass.modulename);
//            passIndex = i;

//            //if (pass.isInstant)
//            //    GenerateInstantly();
//            //else
//                yield return StartCoroutine(GenerateOverTime());
//        }

//        if (settings.type != ResultType.Inner)
//            CreateOuter();

//        if (settings.type == ResultType.Outer)
//            RemoveInner();
//    }

//    private void CreateOuter()
//    {
//        Debug.Log($"Creating Outer!");
//        foreach (Vector3 pos in grid.cells.Keys)
//        {
//            Cell c = grid.GetCellAtPos(pos);
//            if (c.isOccupied || c.markedAsVoid)
//                continue;
//            GameObject cube = Instantiate(debugCube, pos, Quaternion.identity);
//            debugCubes.Add(cube);
//        }
//    }

//    private void RemoveInner()
//    {
//        Debug.Log($"There are {grid.cells.Count} cells!, lets remove all occupied ones!");
//        List<Vector3> posToRemove = new List<Vector3>();
//        foreach (Vector3 pos in grid.cells.Keys)
//        {
//            if (grid.GetCellAtPos(pos).isOccupied)
//                posToRemove.Add(pos);
//        }
//        foreach (Vector3 pos in posToRemove)
//        {
//            Tile t = grid.GetCellAtPos(pos).occupyingTile;
//            t.parentCell.markedAsVoid = true;
//            Destroy(t.gameObject);
//            grid.RemoveAtPos(pos);
//        }
//    }

//    private void InitStart()
//    {
//        //GameObject firstTile = Instantiate(settings.sizeTile);
//        //lastGeneratedTile = firstTile.GetComponent<Tile>();
//        //lastGeneratedTile.Init();

//        //connectorsToSpawn.AddRange(lastGeneratedTile.connectors);

//        //grid.AddTileToGrid(Vector3.zero, firstTile.GetComponent<Tile>());
//    }

//    private ModuleReferenceData UpdateModuleData(ModuleReferenceData d)
//    {
//        //d.walkerPosition = walker.position;
//        d.walkerPosition = connectorWalker.position;
//        d.lastTile = lastGeneratedTile;
//        d.connectors = connectorsToSpawn;
//        return d;
//    }

//    private void GenerateInstantly()
//    {
//        int connectorIndex = 0;
//        ModuleReferenceData newData = new ModuleReferenceData();
//        newData.connectorsIndex = connectorIndex;
//        do
//        {
//            newData = UpdateModuleData(newData);
//            connectorIndex = automata.currentModule.Sort(newData);

//            if (!canProcessConnector(connectorIndex) || hasMatchingTile(out GameObject matchingTile))
//                    continue;

//            Tile t = CreateTile(matchingTile);
//            connectorsToSpawn.AddRange(t.connectors);

//            MoveTileToPosition(t.gameObject);
//            grid.AddTileToGrid(t.transform.position, t); //assumes its already been moved into pos
//            ConnectToSurroundingGrid(t);
//            PositionWalkers(t);
//        }
//        while (canSpawn);

//        UI.SetDataText(connectorsToSpawn.Count, generatedTiles);
//    }

//    IEnumerator GenerateOverTime()
//    {
//        UI.StartSession();
//        int connectorIndex = 0;
//        ModuleReferenceData newData = new ModuleReferenceData();
//        newData.connectorsIndex = connectorIndex;

//        do
//        {
//            newData = UpdateModuleData(newData);
//            connectorIndex = automata.currentModule.Sort(newData);

//            if (!canProcessConnector(connectorIndex) || !hasMatchingTile(out GameObject matchingTile))
//                continue;

//            Tile t = CreateTile(matchingTile);
//            connectorsToSpawn.AddRange(t.connectors);

//            MoveTileToPosition(t.gameObject);
//            grid.AddTileToGrid(t.transform.position, t); //Move first, add second
//            Chunk activeChunk = world.GetChunkAt(t.transform.position);
//            //activeChunk.
//            ConnectToSurroundingGrid(t);
//            PositionWalkers(t);

//            yield return new WaitForSeconds(settings.passes[passIndex].creationspeed);
//        }
//        while (canSpawn);

//        UI.StopSession();
//    }

//    private void PositionWalkers(Tile t)
//    {
//        walker.position = t.transform.position;
//        connectorWalker.position = currentConnector.transform.position;
//    }

//    public void Clear()
//    {
//        UI.ClearSession();
//        StopAllCoroutines();

//        if (spawnedTiles.Count > 0)
//        {
//            foreach (Tile t in spawnedTiles)
//            {
//                if(t.parentCell == null)
//                {
//                    Debug.Log("Tile had no cell to destroy");
//                    continue;
//                }

//                Destroy(t.gameObject);
//                grid.ResetGrid();
//            }
//            spawnedTiles.Clear();
//        }

//        foreach(GameObject c in debugCubes)
//        {
//            Destroy(c);
//        }
//        debugCubes.Clear();
//        cellPositions.Clear();

//        generatedTiles = 0;
//        connectorsToSpawn.Clear();
//        currentConnector = null;

//        walker.position = Vector3.zero;
//        connectorWalker.position = Vector3.zero;
//    }

//    bool canProcessConnector(int index)
//    {
//        if (index < 0 || index > connectorsToSpawn.Count-1)
//        {
//            Debug.Log("Serious issue, index less than zero");
//            return false;
//        }

//        Connector foundConnector = connectorsToSpawn[index];
//        connectorsToSpawn.RemoveAt(index);

//        Vector3 dir = ConnectorDirToNewCell(foundConnector);
//        Cell cellAtPos = grid.GetCellAtPos(foundConnector.transform.position + dir);

//        if (foundConnector == null || foundConnector.isOccupied || cellAtPos == null || cellAtPos.isOccupied)
//        {
//            return false;
//        }

//        currentConnector = foundConnector;
//        return true;
//    }

//    Vector3 ConnectorDirToNewCell(Connector connector)
//    {
//        return (connector.transform.position - connector.parentTile.transform.position).normalized * grid.cellDiameter / 2;
//    }

//    bool hasMatchingTile(out GameObject result)
//    {
//        result = null;
//        if (tileDatabase.tileDictionary.TryGetValue(currentConnector.connectorID, out string[] options))
//        {
//            if(options.Length == 0)
//            {
//                Debug.Log("No options for this connector");
//                return false;
//            }

//            int optionIndex = Random.Range(0, options.Length);
//            result = Resources.Load(options[optionIndex]) as GameObject;
//            return true;
//        }
//        else
//        {
//            Debug.Log("No tile with given id : " + currentConnector.connectorID);
//            return false;
//        }
//    }

//    Tile CreateTile(GameObject newTileGO)
//    {
//        newTileGO = Instantiate(newTileGO, currentConnector.transform.position, Quaternion.identity);
//        Tile newTile = newTileGO.GetComponent<Tile>();
//        newTile.Init();

//        lastGeneratedTile = newTile;
//        generatedTiles++;
//        spawnedTiles.Add(newTile);

//        return newTile;
//    }

//    void ConnectToSurroundingGrid(Tile tile)
//    {
//        if(tile.parentCell == null)
//        {
//            Debug.Log("Tile has no cell!");
//            return;
//        }

//        foreach (Cell neighbor in grid.GetNeighbours(tile.parentCell))
//        {
//            //Add as neighbors?
//        }
//    }

//    private void MoveTileToPosition(GameObject newTile)
//    {
//        Vector3 dir = currentConnector.parentTile.transform.position - newTile.transform.position;
//        newTile.transform.position -= dir;
//    }

//}
