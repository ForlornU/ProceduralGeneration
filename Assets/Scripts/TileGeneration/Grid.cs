using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    Cell[,] cells;
    Vector2Int size;
    public Vector2 worldSize = new Vector2(200,200);
    LayerMask unwalkableMask;
    public float nodeRadius;
    float nodeDiameter;

    public void InitGrid(float radius)
    {
        nodeRadius = radius;
        nodeDiameter = radius * 2;
        size.x = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        size.y = Mathf.RoundToInt(worldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        cells = new Cell[size.x, size.y];
        Vector3 worldBottomLeft = Vector3.zero - Vector3.right * worldSize.x / 2 - Vector3.forward * worldSize.y / 2;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                cells[x, y] = new Cell(worldPoint, x, y);
                Debug.DrawRay(cells[x, y].worldPosition, Vector3.up, Color.red, 10f);
            }
        }

        Debug.Log(cells.Length);
    }

    public List<Cell> GetNeighbours(Cell c)
    {
        List<Cell> neighbours = new List<Cell>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                Vector2Int check = new Vector2Int(c.gridX + x, c.gridY + y);

                if (check.x >= 0 && check.x < size.x && check.y >= 0 && check.y < size.y)
                {
                    neighbours.Add(cells[check.x, check.y]);
                }
            }
        }

        return neighbours;
    }
}
