using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int id;
    public string worldName;
    public WorldManager manager;
    public EntityManager entityManager;

    public Entity CreateEntity(){
        return entityManager.CreateEntity();
    }

    public void RemoveEntity(Entity entity){
        entityManager.RemoveEntity(entity);
    }

}
