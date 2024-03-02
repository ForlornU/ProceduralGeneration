using System.Collections.Generic;
using UnityEngine;

public class DynamicGrid : MonoBehaviour
{

    Dictionary<Vector3, Cell> cells = new Dictionary<Vector3, Cell>();
    //Vector3 startPosition = Vector3.zero; // Use to offset grid correctly?
    //float cellRadius;
    float cellDiameter;

    public void Init(float cellRadius)
    {
        //this.cellRadius = cellRadius;
        cellDiameter = cellRadius * 2;
    }

    public void AddTileToGrid(Vector3 pos, Tile tile)
    {
        //Place into an existing cell
        if(cells.TryGetValue(pos, out Cell foundCell))
        { 
            if(foundCell.isOccupied)
            {
                Debug.Log("Adding to an occupied tile, continuing");
                return;
            }

            Destroy(foundCell.DebugBox);
            foundCell.PlaceTile(tile);
            CreateNeighbors(pos);
            return;
        }

        //Create a new cell for this tile, meaning its the first tile
        //Add to list, occupy
        Cell newCell = new Cell(pos);
        cells.Add(pos, newCell);
        newCell.PlaceTile(tile);

        //Create neighbors
        CreateNeighbors(pos);
    }

    void AddACellToGrid(Vector3 pos)
    {
        if (cells.ContainsKey(pos))
        {
            Debug.Log("Already a cell here, skip");
            return;
        }

        Cell newCell = new Cell(pos);
        cells.Add(pos, newCell);
        GameObject DebugBox = Instantiate(Resources.Load("TextureTileCellBox") as GameObject, pos, Quaternion.identity);
        newCell.DebugBox = DebugBox;
    }

    void CreateNeighbors(Vector3 centre)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector3 pos = centre + new Vector3(x, y, z) * cellDiameter;

                    if (IsDiagonalOrCenter(new Vector3(x, y, z)) || cells.ContainsKey(pos))
                        continue;

                    AddACellToGrid(pos);
                }

            }
        }
    }

    bool IsDiagonalOrCenter(Vector3 pos)
    {
        return pos.x * pos.z != 0 || pos.y * pos.z != 0 || pos.x * pos.y != 0 || (pos.x == 0 && pos.y == 0 && pos.z == 0);
    }


    //public List<Cell> GetNeighbours(Cell c)
    //{
    //    List<Cell> neighbours = new List<Cell>();

    //    for (int x = -1; x <= 1; x++)
    //    {
    //        for (int y = -1; y <= 1; y++)
    //        {
    //            if (x == 0 && y == 0)
    //                continue;

    //            Vector2Int check = new Vector2Int(c.gridX + x, c.gridY + y);

    //            if (check.x >= 0 && check.x < size.x && check.y >= 0 && check.y < size.y)
    //            {
    //                neighbours.Add(cells[check.x, check.y]);
    //            }
    //        }
    //    }

    //    return neighbours;
    //}

    //public Cell CellFromWorldPoint(Vector3 worldPosition)
    //{
    //    Vector2 percent = new Vector2(
    //        (worldPosition.x + worldSize.x / 2f) / worldSize.x,
    //        (worldPosition.z + worldSize.y / 2f) / worldSize.y
    //    );

    //    percent.x = Mathf.Clamp01(percent.x);
    //    percent.y = Mathf.Clamp01(percent.y);

    //    int x = Mathf.RoundToInt((size.x - 1) * percent.x);
    //    int y = Mathf.RoundToInt((size.y - 1) * percent.y);

    //    return cells[x, y];
    //}

}
