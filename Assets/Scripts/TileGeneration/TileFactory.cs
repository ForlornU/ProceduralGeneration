using UnityEngine;

public class TileFactory : MonoBehaviour
{

    public void CreateTile(GameObject newTile, Connector currentConnector)
    {
        newTile = Instantiate(newTile, currentConnector.transform.position, Quaternion.identity);
        Tile tile = newTile.GetComponent<Tile>();
        tile.Init();

        //cursor.position = newTile.transform.position;

        //generatedTiles++;
        //spawnedTiles.Add(tile);

        if (tile.connectors.Count == 0)
            return;

        //connectorsToSpawn.AddRange(tile.connectors);

        //MoveTileToPosition(newTile);
        //ConnectClosestConnectorOnNewTile(tile, currentConnector);
        //ConnectToSurroundingTiles(tile);
    }

}
