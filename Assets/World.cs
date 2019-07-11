using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int id;
    public string worldName;
    public WorldManager manager;
    public EntityManager entityManager;

    public void RemoveEntity(Entity entity){
        entityManager.RemoveEntity(entity);
    }

}
