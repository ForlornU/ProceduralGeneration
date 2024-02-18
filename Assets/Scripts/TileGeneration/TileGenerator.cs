using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneratorUI))]
public class TileGenerator : MonoBehaviour
{
    [SerializeField] bool randomSimulation = false;
    [SerializeField] bool RandomWalk = true;

    [SerializeField] Transform cursor;
    GeneratorUI UI;
    TileDatabase tileDatabase;

    List<Connector> connectorsToSpawn = new List<Connector>();
    Connector currentConnector;

    List<Tile> spawnedTiles = new List<Tile>();
    Tile StartingTile;
    Tile lastGeneratedTile;
    int generatedTiles = 0;

    public bool canSpawn { get { return connectorsToSpawn.Count > 0 && generatedTiles < UI.maxSliderValue; } }

    private void Start()
    {
        UI = GetComponent<GeneratorUI>();
        tileDatabase = new TileDatabase();

        FindStartingConnectors(); //Only do this once, add rest manually
    }

    private void Update()
    {
        UI.WriteToUI(connectorsToSpawn.Count, generatedTiles);
    }

    public void Generate()
    {
        UI.StartSession();

        FindStartingConnectors();

        StartCoroutine(GenerateTiles());
    }

    IEnumerator GenerateTiles()
    {
        int connectorIndex = 0;

        do
        {
            yield return null;

            if (randomSimulation)
                connectorIndex = Random.Range(0, connectorsToSpawn.Count-1);
            else
                SortConnectors();

            if (RandomWalk)
            {
                connectorIndex = Random.Range(0, connectorsToSpawn[connectorIndex].parentTile.connectors.Count-1); // randomize between the 6 closest options
            }

            //Index to be 0 for sorted
            if (!canProcessConnector(connectorIndex))
            {
                continue;
            }

            if (hasMatchingTile(out GameObject matchingTile))
            {
                CreateTile(matchingTile);
            }

            yield return new WaitForSeconds(UI.TimeSliderValue);
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

            foreach (Connector connector in tile.connectors)
            {
                if (connector.isOccupied)
                    continue;

                connectorsToSpawn.Add(connector);
            }
        }
    }

    void SortConnectors()
    {
        //connectorsToSpawn.Sort((x, y) => x.transform.position.x.CompareTo(y.transform.position.x));
        //Possibly spawn the one closest to the player in the future...
        Vector3 rwalkpos = Vector3.zero;
        if(lastGeneratedTile == null)
        {
            rwalkpos = cursor.position;
        }
        else
        {
            rwalkpos = lastGeneratedTile.transform.position;
        }

        if(RandomWalk)
            connectorsToSpawn.Sort((x, y) => Vector3.Distance(x.transform.position, rwalkpos).CompareTo(Vector3.Distance(y.transform.position, rwalkpos)));
        else
            connectorsToSpawn.Sort((x, y) => tileDatabase.tileDictionary[x.connectorID].Length.CompareTo(tileDatabase.tileDictionary[y.connectorID].Length));
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
