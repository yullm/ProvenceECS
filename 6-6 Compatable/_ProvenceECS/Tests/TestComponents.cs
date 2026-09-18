using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using Newtonsoft.Json;

public class TestComponents : ProvenceComponent{
    
    public string words;
    public bool boolean = false;
    [JsonConverter(typeof(Vector3Converter))]
    public Vector3 vector = Vector3.one;
    [JsonConverter(typeof(Vector2Converter))]
    public Vector2 vector2 = Vector2.one;
    public int x;
    public float y = 0.9f;
    public GameObject go;
    public Entity entity;
    //public List<Entity> entities;

    public TestComponents(){
      this.words = "words n stuff";
      this.x = 10;
    }

}
