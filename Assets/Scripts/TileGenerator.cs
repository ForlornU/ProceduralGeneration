using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileGenerator : MonoBehaviour
{
    bool simulating = false;
    [SerializeField] Transform cursor;
    [SerializeField] Slider timeSlider;
    [SerializeField] Slider maxTilesSlider;
    [SerializeField] Button generateButton;

    [SerializeField] TMPro.TextMeshProUGUI maxTilesText;
    [SerializeField] TMPro.TextMeshProUGUI dataText;
    [SerializeField] TMPro.TextMeshProUGUI timeText;

    float timeSpent = 0f;

    List<Connector> connectorsToSpawn = new List<Connector>();
    List<Tile> spawnedTiles = new List<Tile>();
    Tile StartingTile;

    [HideInInspector] public int maxTiles = 28;
    int generatedTiles = 0;
    TileDatabase tileDatabase;
    public bool canSpawn { get { return connectorsToSpawn.Count > 0 && generatedTiles < maxTiles; } }

    Connector currentConnector;

    private void Start()
    {
        tileDatabase = new TileDatabase();
        FindStartingConnectors(); //Only do this once, add rest manually
    }

    private void Update()
    {
        maxTiles = (int)maxTilesSlider.value;
        maxTilesText.text = "Max Tiles: " + maxTiles;
        timeText.text = "Update Time: " + timeSlider.value;

        if (!simulating)
            return;

        timeSpent += Time.deltaTime;

        dataText.text = "Open connectors in world: \n" + connectorsToSpawn.Count + "\n" +
            "Tiles spawned: \n" + generatedTiles + "\n" +
            "Time spent in simulation: " + timeSpent.ToString("F1");
    }

    public void Generate()
    {
        simulating = true;
        timeSpent = 0f;

        generateButton.interactable = false;
        FindStartingConnectors();

        StartCoroutine(GenerateTiles());
    }

    IEnumerator GenerateTiles()
    {
        do
        {
            yield return new WaitForSeconds(timeSlider.value);
            SortConnectors();
            if (!canProcessConnector())
                continue;

            if (hasMatchingTile(out GameObject match))
            {
                CreateTile(match);
            }

        }

        while (canSpawn);

        simulating = false;
    }

    public void ClearOldTiles()
    {
        generateButton.interactable = true;
        dataText.text = "Open connectors in world: \n" + 0 + "\n" +
    "Tiles spawned: \n" + 0 + "\n" +
    "Time spent in simulation: " + 0;

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
    /// Finds all the current open connectors in the world
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

        //Disabled for now, tiles start to overlap
        connectorsToSpawn.Sort((x, y) => tileDatabase.tileDictionary[x.connectorID].Length.CompareTo(tileDatabase.tileDictionary[y.connectorID].Length));
    }

    bool canProcessConnector()
    {
        Connector c = connectorsToSpawn[0];
        connectorsToSpawn.RemoveAt(0);

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

            //CreateTile(result);
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

        cursor.position = newTile.transform.position;

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
            pos.y += 1f;

            //Debug.DrawRay(connector.transform.position, Vector3.down * 10f, Color.yellow, 3f);
            //Debug.DrawLine(connector.transform.position, pos, Color.blue, 3f);
            //Debug.DrawRay(pos, Vector3.down * 10f, Color.green, 3f);

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 10f)) //SphereCast(pos, 1f, Vector3.down, out RaycastHit hit))
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
        //if (t == currentConnector.parentTile)
        //{
        //    Debug.Log("We hit ourselves, ignore");
        //    return;
        //}

        float closestDistance = Mathf.Infinity;
        Connector closestConnector = null;

        foreach (Connector connector in t.connectors)
        {
            //if (connector.isOccupied)  //If we ignore an occupied connector, we might end up with a connector further away that is the wrong one
            //    continue;

            //Take this oppurtunity to add these connectors to the list
            //connectorsToSpawn.Add(connector); //If we already created this tile before, we don't want to add the connectors again

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
            //return; temp, occupy anyway
        }

        x.isOccupied = true;
        connectorsToSpawn.Remove(x);

        y.isOccupied = true;
        connectorsToSpawn.Remove(y);

        x.connectedTo = y;
        y.connectedTo = x;
    }


}
