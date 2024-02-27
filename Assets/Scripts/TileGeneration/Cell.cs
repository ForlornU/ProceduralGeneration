using UnityEngine;

public class Cell
{
    public Tile occupyingTile { get; private set; }
    public bool isOccupied { get { return occupyingTile != null; }}

    public int gridX;
    public int gridY;
    public Vector3 worldPosition;

    public void PlaceTile(Tile tile)
    {
        occupyingTile = tile;
    }

    public Cell(Vector3 point, int gridX, int gridY)
    {
        this.worldPosition = point;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}
