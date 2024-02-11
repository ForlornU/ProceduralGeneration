using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    List<Connector> connectorsToSpawn = new List<Connector>();
    [SerializeField] int maxTiles = 28;
    int generatedTiles = 0;
    TileDatabase tileDatabase;
    public bool canSpawn { get { return connectorsToSpawn.Count > 0 && generatedTiles < maxTiles; } }

    Connector currentConnector;

    private void Start()
    {
        tileDatabase = new TileDatabase();
        FindStartingConnectors(); //Only do this once, add rest manually
        do
        {
            //SortConnectors();
            if (!canProcessConnector())
                continue;

            if (hasMatchingTile(out GameObject match))
            {
                CreateTile(match);
            }

        }

        while (canSpawn);


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
            tile.Init();

            foreach (Connector connector in tile.connectors)
            {
                if (connector.isOccupied)
                    continue;

                connectorsToSpawn.Add(connector);
                Debug.Log("Connector found");
            }
        }
    }

    //void SortConnectors()
    //{
    //    connectorsToSpawn.Sort((x, y) => x.transform.position.x.CompareTo(y.transform.position.x));
    //    Possibly spawn the one closest to the player in the future...

    //    Disabled for now
    //    connectorsToSpawn.Sort((x, y) => tileDatabase.tileDictionary[x.connectorID].Length.CompareTo(tileDatabase.tileDictionary[y.connectorID].Length));
    //}

    bool canProcessConnector()
    {
        Connector c = connectorsToSpawn[0];
        connectorsToSpawn.RemoveAt(0);

        if (c == null || c.isOccupied)
        {
            Debug.Log("Connector is null or occupied");
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
        generatedTiles++;

        newTile = Instantiate(newTile, currentConnector.transform.position, Quaternion.identity);
        Tile tile = newTile.GetComponent<Tile>();
        tile.Init();

        if (tile.connectors.Count == 0)
            return;

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
                Debug.Log("Hit something");
                //connector.isOccupied = true; If we set occupied here, it wont connect to the thing we hit
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
            connectorsToSpawn.Add(connector);

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
            return;
        }

        x.isOccupied = true;
        connectorsToSpawn.Remove(x);

        y.isOccupied = true;
        connectorsToSpawn.Remove(y);

        x.connectedTo = y;
        y.connectedTo = x;

        Debug.Log("Connected two connectors");
    }


}
