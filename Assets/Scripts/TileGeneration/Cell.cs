using UnityEngine;

//public enum CellState { Empty, Occupied}
public class Cell
{
    //public CellState State;
    public GameObject DebugBox;
    public Tile occupyingTile { get; private set; }
    public bool isOccupied { get { return occupyingTile != null; } }

    public Vector3 worldPosition;

    public void PlaceTile(Tile tile)
    {
        occupyingTile = tile;
    }

    public Cell(Vector3 worldPosition)
    {
        this.worldPosition = worldPosition;
    }

}
