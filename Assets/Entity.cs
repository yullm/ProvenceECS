﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    [System.Serializable]
    public class Entity
    {

        public int id;

        public static implicit operator Entity (int value){
            return new Entity(){ id = value };
        }

        public static implicit operator int (Entity value){
            return value.id;
        }

        public static bool operator == (Entity e1, Entity e2){
            return e1.id == e2.id;
        }

        public static bool operator != (Entity e1, Entity e2){
            return e1.id != e2.id;
        }

        public override bool Equals(object o){
            return (o is Entity && ((Entity)o).id == this.id);
        }

        public override int GetHashCode(){
            return id.GetHashCode();
        }

        /*public int Compare(object o1, object o2){
            Entity a = (Entity)o1;
            Entity b = (Entity)o2;
            if(a < b) return -1;
            if(a == b) return 0;
            return 1;
        }*/

    }

    public class EntityHandle{
        public Entity entity;
        public World world;
        public EntityManager manager;
        public GameObject gameObject;

        public ComponentHandle<T> AddComponent<T>() where T : ProvenceComponent{
            return world.AddComponent<T>(this);
        }

        public void RemoveComponent<T>() where T : ProvenceComponent{
            world.RemoveComponent<T>(this);
        }

        public void Destroy(){
            world.RemoveEntity(this);
        }
    }
}