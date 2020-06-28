using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace ProvenceECS{
    
    [System.Serializable]
    [JsonConverter(typeof(EntityConverter))]
    public class Entity : System.IComparable{
        public string id;

        public Entity(){
            this.id = Guid.NewGuid().ToString();
        }

        public Entity(string id){
            this.id = id;
        }

        public static bool operator == (Entity obj1, Entity obj2){
            if(((object)obj1) == null)
                return ((object)obj2) == null;
            return obj1.Equals(obj2);
        }

        public static bool operator != (Entity obj1, Entity obj2){
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj){
            if (obj == null)
                return false;

            if(obj.GetType() != typeof(Entity))
                if(obj.GetType() != typeof(string)) return false;

            if(obj.GetType() == typeof(Entity)) return (id == ((Entity)obj).id);
            else return (id == (string)obj);
        }

        public override int GetHashCode(){
            return id.GetHashCode();
        }

        public override string ToString(){
            return id;
        }

        public int CompareTo(object obj){
            if(obj.GetType() != typeof(Entity))
                if(obj.GetType() != typeof(string)) return 1;

           if(obj.GetType() == typeof(Entity)) return id.CompareTo(((Entity)obj).id);
            else return id.CompareTo((string)obj);
        }

        public static explicit operator Entity(string value){
            Entity entity = new Entity(value);
            return entity;
        }

        public static explicit operator string(Entity entity){
            return entity.id;
        }
    }

    public class EntityHandle{
        public Entity entity;
        public World world;

        public EntityHandle(World world){
            this.world = world;
            this.entity = null;
        }

        public EntityHandle(Entity entity, World world){
            this.world = world;
            this.entity = entity;
        }

        public ComponentHandle<T> AddComponent<T>() where T : ProvenceComponent, new(){
            return world.AddComponent<T>(this);
        }

        public ComponentHandle<T> AddComponent<T>(T component) where T : ProvenceComponent{
            return world.AddComponent<T>(this, component);
        }

        public ComponentHandle<T> GetOrCreateComponent<T>() where T : ProvenceComponent, new(){
            return world.GetOrCreateComponent<T>(this);
        }

        public ComponentHandle<T> GetComponent<T>() where T : ProvenceComponent{
            return world.GetComponent<T>(this);
        }

        public List<ComponentHandle<ProvenceComponent>> GetAllComponents(){
            return world.GetAllComponents(this);
        }

        public void RemoveComponent<T>() where T : ProvenceComponent{
            world.RemoveComponent<T>(this);
        }

        public void RemoveComponent<T>(T component) where T : ProvenceComponent{
            world.RemoveComponent<T>(this);
        }

        public GameObject AddGameObject(){
            return world.AddGameObject(this);
        }

        public GameObject GetGameObject(){
            return world.GetGameObject(this);
        }

        public void RemoveGameObject(){
            world.RemoveGameObject(this);
        }

        public void Destroy(){
            world.RemoveEntity(this);
        }
    }
}