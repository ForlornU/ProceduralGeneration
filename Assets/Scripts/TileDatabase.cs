using System.Collections.Generic;
using UnityEngine;

public class TileDatabase
{
    
    public Dictionary<uint, GameObject> tileDictionary = new Dictionary<uint, GameObject>()
    {
        //id, path
        {0,  Resources.Load("Tiles/TilePrototype") as GameObject }

    };


}
