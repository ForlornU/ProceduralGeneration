using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneratorUI))]
public class TileGenerator : MonoBehaviour
{
    [SerializeField] Transform cursor;

    //Dependencies
    GeneratorUI UI;
    TileDatabase tileDatabase;
    GeneratorAutomota automata;

    //Collections
    List<Connector> connectorsToSpawn = new List<Connector>();
    List<Tile> spawnedTiles = new List<Tile>();

    //Runtime
    Connector currentConnector;
    Tile StartingTile;
    Tile lastGeneratedTile;
    int generatedTiles = 0;
    float breakpoint = 0f;
    public bool canSpawn { get { return connectorsToSpawn.Count > 0 && generatedTiles < UI.maxSliderValue; } }

    private void Start()
    {
        UI = GetComponent<GeneratorUI>();
        tileDatabase = new TileDatabase();
        automata = transform.GetChild(0).GetComponent<GeneratorAutomota>();
        automata.Init();
        UI.SetGenerationOptions(automata.GetAllModuleNames());
        FindStartingConnectors(); //Only do this once, add rest manually
    }

    private void Update()
    {
        UI.WriteToUI(connectorsToSpawn.Count, generatedTiles);
        //automata.ChangeModule(UI.GetCurrentModule);
    }

    public void Generate()
    {
        FindStartingConnectors();
        automata.ChangeModule(UI.GetCurrentModule);
        breakpoint = UI.breakPointValue;

        if (UI.isInstant)
            GenerateInstantly();
        else
            StartCoroutine(GenerateOverTime());
    }

    private ModuleReferenceData UpdateModuleData(ModuleReferenceData d)
    {
        d.walkerPosition = cursor.position;
        d.lastTile = lastGeneratedTile;
        d.connectors = connectorsToSpawn;
        return d;
    }

    private void GenerateInstantly()
    {
        int connectorIndex = 0;
        ModuleReferenceData newData = new ModuleReferenceData();
        newData.connectorsIndex = connectorIndex;

        do
        {
            newData = UpdateModuleData(newData);
            connectorIndex = automata.currentModule.Sort(newData);

            if (!canProcessConnector(connectorIndex))
                continue;

            if (hasMatchingTile(out GameObject matchingTile))
            {
                CreateTile(matchingTile);
            }
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
            yield return null; // Always wait one frame
            newData = UpdateModuleData(newData);
            connectorIndex = automata.currentModule.Sort(newData);

            if (!canProcessConnector(connectorIndex))
                continue;

            if (hasMatchingTile(out GameObject matchingTile))
            {
                CreateTile(matchingTile);
            }

            Breakpoint();

            yield return new WaitForSeconds(UI.TimeSliderValue);
        }

        while (canSpawn);

        UI.StopSession();
    }

    private void Breakpoint()
    {
        breakpoint++;
        if (UI.breakpointToggle.isOn && breakpoint >= UI.maxSliderValue * UI.breakPointValue)
        {
            automata.ChangeModuleRandom();
            breakpoint = 0;
        }
    }

    public void ClearOldTiles()
    {
        UI.ClearSession();

        if (spawnedTiles.Count > 0)
        {
            foreach (Tile t in spawnedTiles)
            {
                Destroy(t.gameObject);
            }
            spawnedTiles.Clear();
        }

        generatedTiles = 0;
        connectorsToSpawn.Clear();
        currentConnector = null;

        if(StartingTile != null)
            StartingTile.ForgetConnections();

        cursor.position = Vector3.zero;
    }

    /// <summary>
    /// Finds all the current open connectors in the world, but the starting tile assumes there is only one
    /// </summary>
    void FindStartingConnectors()
    {
        GameObject[] foundTiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tileObject in foundTiles)
        {
            Tile tile = tileObject.GetComponent<Tile>();
            StartingTile = tile;
            tile.Init();
            lastGeneratedTile = tile;

            foreach (Connector connector in tile.connectors)
            {
                if (connector.isOccupied)
                    continue;

                connectorsToSpawn.Add(connector);
            }
        }
    }

    bool canProcessConnector(int index)
    {
        Connector c = connectorsToSpawn[index];
        connectorsToSpawn.RemoveAt(index);

        //This happens thousands of times, so we don't want to log it. But also find a way to avoid it
        if (c == null || c.isOccupied)
        {
            //Debug.Log("Connector is null or occupied");
            return false;
        }

        currentConnector = c;
        return true;
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
            Resources.Load(options[optionIndex]);
            result = Resources.Load(options[optionIndex]) as GameObject;
            return true;
        }
        else
        {
            Debug.Log("No tile with given id : " + currentConnector.connectorID);
            return false;
        }
    }

    void CreateTile(GameObject newTile)
    {
        newTile = Instantiate(newTile, currentConnector.transform.position, Quaternion.identity);
        Tile tile = newTile.GetComponent<Tile>();
        tile.Init();
        lastGeneratedTile = tile;

        cursor.position = currentConnector.transform.position;

        generatedTiles++;
        spawnedTiles.Add(tile);

        if (tile.connectors.Count == 0)
            return;

        connectorsToSpawn.AddRange(tile.connectors);

        MoveTileToPosition(newTile);
        ConnectClosestConnectorOnNewTile(tile, currentConnector);
        ConnectToSurroundingTiles(tile);
    }

    private void ConnectToSurroundingTiles(Tile tile)
    {
        foreach (Connector connector in tile.connectors)
        {
            if (connector.isOccupied)
                continue;

            Vector3 pos = tile.transform.position - (tile.transform.position - connector.transform.position) * 2;
            pos.y += 10f;

            //Debug.DrawRay(connector.transform.position, Vector3.down * 10f, Color.yellow, 3f);
            //Debug.DrawLine(connector.transform.position, pos, Color.blue, 3f);
            //Debug.DrawRay(pos, Vector3.down * 10f, Color.green, 3f);

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 20f)) //SphereCast(pos, 1f, Vector3.down, out RaycastHit hit))
            {
                Tile hitTile = hit.collider.GetComponent<Tile>();
                ConnectClosestConnectorOnNewTile(hitTile, connector);
            }
        }
    }

    private void MoveTileToPosition(GameObject newTile)
    {
        Vector3 dir = currentConnector.parentTile.transform.position - newTile.transform.position;
        newTile.transform.position -= dir;
    }

    void ConnectClosestConnectorOnNewTile(Tile t, Connector c)
    {
        float closestDistance = Mathf.Infinity;
        Connector closestConnector = null;

        foreach (Connector connector in t.connectors)
        {
            float dist = Vector3.Distance(c.transform.position, connector.transform.position);
            if(dist < closestDistance)
            {
                closestDistance = dist;
                closestConnector = connector;
            }
        }

        MatchConnectors(c, closestConnector);

    }

    void MatchConnectors(Connector x, Connector y)
    {
        if(x.isOccupied || y.isOccupied)
        {
            Debug.Log("One or both connectors are occupied");
        }

        x.isOccupied = true;
        connectorsToSpawn.Remove(x);

        y.isOccupied = true;
        connectorsToSpawn.Remove(y);

        x.connectedTo = y;
        y.connectedTo = x;
    }
}
