using System.Collections.Generic;
using UnityEngine;

public class TileDatabase
{
    public Dictionary<uint, GameObject[]> tileDictionary = new Dictionary<uint, GameObject[]>()
{
    {
        0, new GameObject[] //Grey can spawn any color
        {
            Resources.Load("Tiles/hextest") as GameObject,
            Resources.Load("Tiles/hextest_yellow") as GameObject,
            Resources.Load("Tiles/hextest_red") as GameObject
        }
    },
    {
        1, new GameObject[] //Red can only spawn yellow
        {
            Resources.Load("Tiles/hextest_yellow") as GameObject
        }
    },
        {
            2, new GameObject[] //Yellow can only spawn red and yellow
            {
                Resources.Load("Tiles/hextest_red") as GameObject,
                Resources.Load("Tiles/hextest_yellow") as GameObject
            }
        }
};

}



