using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    List<GameObject> tilesToSpawn = new List<GameObject>();
    List<GameObject> tilesNotSpawned = new List<GameObject>();

    const int maxTiles = 5;
    int generatedTiles = 0;
    TileDatabase tileDatabase;

    private void Start()
    {
        tileDatabase = new TileDatabase();
        FindTilesInWorld();

        while (generatedTiles <= maxTiles)
        {
            GenerateTiles();

            if (tilesToSpawn.Count <= 0 && tilesNotSpawned.Count > 0)
            {
                tilesToSpawn.AddRange(tilesNotSpawned);
                tilesNotSpawned.Clear();
            }
        }
    }

    void FindTilesInWorld()
    {
        GameObject[] foundTiles = GameObject.FindGameObjectsWithTag("Tile");
        tilesToSpawn.AddRange(foundTiles);

        if (tilesToSpawn.Count > 0)
            Debug.Log("Found " + tilesToSpawn.Count + " tiles in the world");
        else
            Debug.Log("No tiles found in the world");
    }

    void GenerateTiles()
    {
        foreach (GameObject tileGO in tilesToSpawn)
        {
            Tile tile = tileGO.GetComponent<Tile>();

            foreach (Connector connector in tile.connectors)
            {
                if (connector.isOccupied)
                    continue;

                FindMatchingTile(tile, connector);
                connector.isOccupied = true;
            }
        }
        tilesToSpawn.Clear();
    }

    void CreateATile(Tile sourceTile, GameObject newTile, Vector3 connectorPosition)
    {
        newTile = Instantiate(newTile, connectorPosition, Quaternion.identity);
        tilesNotSpawned.Add(newTile);
        generatedTiles++;

        Vector3 dir = sourceTile.transform.position - newTile.transform.position;
        newTile.transform.position -= dir;
    }

    void FindMatchingTile(Tile t, Connector con)
    {
        if (tileDatabase.tileDictionary.TryGetValue(con.connectorID, out string[] options))
        {
            Debug.Log(options.Length + " options found for connector id : " + con.connectorID);
            int optionIndex = Random.Range(0, options.Length);
            Resources.Load(options[optionIndex]);
            GameObject result = Resources.Load(options[optionIndex]) as GameObject;

            CreateATile(t, result, con.transform.position);
        }
        else
        {
            Debug.Log("No tile with given id : " + con.connectorID);
        }
    }

}
