using System.Collections.Generic;

public class TileDatabase
{

    public Dictionary<uint, string[]> tileDictionary = new Dictionary<uint, string[]>()
    {
        {
            0, new string[] //Default grass
            {
                //"Tiles/FlatHex",
                "Tiles/FlatHex_mountain",
                "Tiles/FlatHex_beach"
            }
        },
        {
            1, new string[] //Mountain
            {
                "Tiles/FlatHex",
                //"Tiles/FlatHex_mountain"
            }
        },
        {
            2, new string[] //Beach
            {
                "Tiles/FlatHex_shallowwater"
            }
        },
        {
            3, new string[] //Shallow water
            {
                //"Tiles/FlatHex_shallowwater",
                "Tiles/FlatHex_beach",
                "Tiles/FlatHex_deepwater"
            }
        },
        {
            4, new string[] //Deep water
            {
                //"Tiles/FlatHex_deepwater",
                "Tiles/FlatHex_shallowwater"
            }
        },
        {
            10, new string[] //Square test tile
            {
                "Tiles/Square/PlaneTile"
            }
        },
        {
            20, new string[] // Cube tile
            {
                "Tiles/3dTile/Cube"
            }
        },
        {
            30, new string[]  //3d Connectors
            {
                "Tiles/3dTile/RectangleVertical",
                "Tiles/3dTile/QuadConnector"
            }
        },
        {
            31, new string[]
            {
                "Tiles/3dTile/RectangleHorizontal",
                "Tiles/3dTile/QuadConnector"
            }
        },
        {
            32, new string[]
            {
                "Tiles/3dTile/RectangleVertical",
                "Tiles/3dTile/TriConnector",
                "Tiles/3dTile/QuadConnector"
            }
        },
        {
            33, new string[]
            {
                "Tiles/3dTile/RectangleHorizontal",
                "Tiles/3dTile/TriConnector",
                "Tiles/3dTile/QuadConnector"
            }
        },
        {
            97, new string[]
            {
                "Tiles/Square/FloorTile"
            }
        },
        {
            98, new string[]
            {
                "Tiles/Square/FloorTileHole"
            }
        },
        {
            99, new string[]
            {
                "Tiles/Square/FloorTile",
                "Tiles/Square/LadderUp",
                "Tiles/Square/LadderDown"
            }
        },
        {
            100, new string[]
            {
                "TextureTileCellBox"
            }
        }
    };
}
