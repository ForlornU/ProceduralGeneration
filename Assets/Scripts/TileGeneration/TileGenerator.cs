using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneratorUI))]
public class TileGenerator : MonoBehaviour
{
    [SerializeField] Transform cursor;
    [SerializeField] bool realTimeModuleControl = false;

    [SerializeField] GenerationSettings settings;
    int passIndex = 0;

    //Dependencies
    GeneratorUI UI;
    TileDatabase tileDatabase;
    GeneratorAutomota automata;

    //Collections
    List<Connector> connectorsToSpawn = new List<Connector>();
    List<Tile> spawnedTiles = new List<Tile>();

    //Runtime
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
        UI.SetGenerationOptions(automata.GetAllModuleNames());
    }

    private void Update()
    {
        UI.WriteToUI(connectorsToSpawn.Count, generatedTiles);

        if(realTimeModuleControl)
            automata.ChangeModule(UI.GetCurrentModule); // allows for real time changing of modules manually, overrides breakpoints
    }

    public void Generate()
    {
        InitStart();

        for (int i = 0; i < settings.Passes.Length; i++)
        {
            PassSettings pass = settings.Passes[i];
            automata.ChangeModule(pass.modulename);
            generatedTiles = 0;

            if (pass.isInstant)
                GenerateInstantly();
            else
                StartCoroutine(GenerateOverTime());
            //This can be solved by having Generate be a coroutine and then:
            // yield return StartCoroutine(GenerateOverTime());
            //This would wait for the GenerateOverTime to finish
            //GenerateInstantly();
            passIndex = i;
            Debug.Log(passIndex);
        }

        passIndex = 0;
    }

    private void InitStart()
    {
        GameObject firstTile = Instantiate(settings.startTile);
        lastGeneratedTile = firstTile.GetComponent<Tile>();
        lastGeneratedTile.Init();
        connectorsToSpawn.AddRange(lastGeneratedTile.connectors);
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

            yield return new WaitForSeconds(settings.Passes[passIndex].creationspeed); //(UI.TimeSliderValue);
        }

        while (canSpawn);

        UI.StopSession();
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

        cursor.position = Vector3.zero;
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
        // Sort connectors based on their distance from c
        t.connectors.Sort((x, y) => Vector3.Distance(c.transform.position, x.transform.position).CompareTo(Vector3.Distance(c.transform.position, y.transform.position)));

        Connector closestConnector = t.connectors[0];

        MatchConnectors(c, closestConnector);
    }

    void MatchConnectors(Connector x, Connector y)
    {
        x.isOccupied = true;
        connectorsToSpawn.Remove(x);

        y.isOccupied = true;
        connectorsToSpawn.Remove(y);

        x.connectedTo = y;
        y.connectedTo = x;
    }
}
