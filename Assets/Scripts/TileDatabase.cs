using System.Collections.Generic;

public class TileDatabase
{
    public Dictionary<uint, string[]> tileDictionary = new Dictionary<uint, string[]>()
    {
        {
            0, new string[] // Dark
            {
                "Tiles/FlatHex",
                "Tiles/FlatHex_empty"
            }
        },
        {
            1, new string[] // Grey can spawn any color
            {
                "Tiles/FlatHex",
                "Tiles/FlatHex_yellow",
                "Tiles/FlatHex_empty"
            }
        },
        {
            2, new string[] // Red can only spawn yellow
            {
                "Tiles/FlatHex_red",
            }
        },
        {
            3, new string[] // Yellow can only spawn red and yellow
            {
                "Tiles/FlatHex_red",
                "Tiles/FlatHex_yellow",
            }
        }
    };
}
