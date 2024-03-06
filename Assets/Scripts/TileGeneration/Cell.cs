using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Tile occupyingTile { get; private set; }
    public bool isOccupied { get { return occupyingTile != null; } }
    public bool markedAsVoid = false;

    public Vector3 worldPosition;

    List<Cell> neighbors = new List<Cell>();

    public void PlaceTile(Tile tile)
    {
        occupyingTile = tile;
        tile.parentCell = this;
    }

    public Cell(Vector3 worldPosition)
    {
        this.worldPosition = worldPosition;
    }

    public void AddNeighbor(Cell cell)
    {
        if(neighbors.Contains(cell)) 
            return;

        neighbors.Add(cell);
    }

}
