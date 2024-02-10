using System.Collections.Generic;
using UnityEngine;

public class TileDatabase
{
    public Dictionary<uint, string[]> tileDictionary = new Dictionary<uint, string[]>()
{
    {
        0, new string[] // Grey can spawn any color
        {
            "Tiles/hextest",
            "Tiles/hextest_yellow",
            "Tiles/hextest_red"
        }
    },
    {
        1, new string[] // Red can only spawn yellow
        {
            "Tiles/hextest_yellow"
        }
    },
    {
        2, new string[] // Yellow can only spawn red and yellow
        {
            "Tiles/hextest_red",
            "Tiles/hextest_yellow"
        }
    }
};


}



