using System.Collections.Generic;
using UnityEngine;

public class SpatialHash : MonoBehaviour
{

    public Dictionary<Vector3, Cell> cells = new Dictionary<Vector3, Cell>();
    [HideInInspector] public float cellDiameter;

    public void Init(float cellRadius)
    {
        //this.cellRadius = cellRadius;
        cellDiameter = cellRadius * 2;
    }

    public Cell GetCellAtPos(Vector3 pos)
    {
        if(cells.ContainsKey(pos))
            return cells[pos];
        else
            return null;
    }

    public void AddTileToGrid(Vector3 pos, Tile tile)
    {
        //Place into an existing cell
        if (cells.TryGetValue(pos, out Cell foundCell))
        {
            if (foundCell.isOccupied)
            {
                Debug.Log("Adding to an occupied tile, continuing");
                return;
            }

            foundCell.PlaceTile(tile);
            CreateNeighbors(pos);
            return;
        }

        CreateFirstCell(pos, tile);
    }

    private void CreateFirstCell(Vector3 pos, Tile tile)
    {
        Cell newCell = new Cell(pos);
        cells.Add(pos, newCell);
        newCell.PlaceTile(tile);
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

    public List<Cell> GetNeighbours(Cell c)
    {
        List<Cell> neighbours = new List<Cell>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for(int z = -1; z <= 1; z++)
                {
                    Vector3 pos = c.worldPosition + new Vector3(x, y, z) * cellDiameter;

                    if (IsDiagonalOrCenter(new Vector3(x, y, z)) || !cells.ContainsKey(pos))
                        continue;

                    neighbours.Add(cells[pos]);
                }
            }
        }

        return neighbours;
    }

    public void ResetGrid()
    {
        cells.Clear();
    }

    public void RemoveAtPos(Vector3 pos)
    {
        if (!cells.ContainsKey(pos))
            return;

        cells.Remove(pos);
    }
}
