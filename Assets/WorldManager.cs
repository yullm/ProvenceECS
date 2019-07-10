using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDictionary : SerializableDicitonary<int,World>{}

public class WorldManager : MonoBehaviour
{
    //switch to array
    public WorldDictionary worlds = new WorldDictionary();
    public World activeWorld;
}
