using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

namespace ProvenceECS{

    public class TickRateTimer{

        protected  float goal;
        protected float timer;
        protected System.Action action;

        public TickRateTimer(byte rate, System.Action action){
            this.goal = 1 / rate;
            this.timer = 0;
            this.action = action;
        }

        public void Tick(float deltaTime){
            this.timer += deltaTime;
            if(timer >= goal){
                action.Invoke();
                timer -= goal;
            }
        }
    }

    public enum BooleanEnum {FALSE,TRUE};

    public class Helpers{

        public static JsonSerializerSettings baseSerializerSettings{
            get{
                return new JsonSerializerSettings{
                    TypeNameHandling = TypeNameHandling.All,
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    ContractResolver = new ProvenceContractResolver()
                };
            }
        }

        public static JsonSerializerSettings netSerializerSettings{
            get{
                return new JsonSerializerSettings{
                    TypeNameHandling = TypeNameHandling.Objects,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                    ContractResolver = new ProvenceContractResolver()
                };
            }
        }

        public static JsonSerializerSettings lowCostSerializerSettings{
            get{
                return new JsonSerializerSettings{
                    TypeNameHandling = TypeNameHandling.None,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                    ContractResolver = new ProvenceContractResolver()
                };
            }
        }

        public static List<Type> GetAllTypesFromBaseType(Type baseType, params Type[] additonalAssemblies){

            List<Type> types = new List<Type>();
            List<Type> baseTypes = new List<Type>(baseType.Assembly.GetTypes());

            foreach(Type type in additonalAssemblies){
                Type[] curTypes = type.Assembly.GetTypes();
                foreach(Type curType in curTypes) if(!baseTypes.Contains(curType)) baseTypes.Add(curType);
            }

            foreach(Type type in baseTypes){
                if((type.IsSubclassOf(baseType) || baseType.IsAssignableFrom(type)) && !type.IsAbstract) types.Add(type);
            }

            return types;
        }

        public static List<Type> GetAllTypesFromBaseType(Type baseType, List<Type> existingTypes, params Type[] additonalAssemblies){
            List<Type> types = new List<Type>();
            foreach(Type type in GetAllTypesFromBaseType(baseType,additonalAssemblies)){
                if(!existingTypes.Contains(type)) types.Add(type);
            }
            return types;
        }

        public static object CreateGenericObject(Type unboundType, Type genericType, params object[] args){
            try{
                Type boundType = unboundType.MakeGenericType(genericType);
                return Activator.CreateInstance(boundType, args);
            }catch(Exception e){
                Debug.LogWarning(e);
                return null;
            }
        }

        public static object InvokeGenericMethod<T>(T invokingObject, string methodName, Type genericType, params object[] args){
            try{
                MethodInfo method = typeof(T).GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                MethodInfo reference = method.MakeGenericMethod(genericType);
                return reference.Invoke(invokingObject, args); 
            }catch(Exception e){
                throw e;
            }
        }

        public static object InvokeStaticGenericMethod<T>( string methodName, Type genericType, params object[] args){
            try{
                MethodInfo method = typeof(T).GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                MethodInfo reference = method.MakeGenericMethod(genericType);
                return reference.Invoke(null, args); 
            }catch(Exception e){
                throw e;
            }
        }

        public static void InvokeExtensionMethod<T>(T invokingObject, string methodName, Type extensionClass, Type genericType = null, params object[] args){
            try{
                MethodInfo method = extensionClass.GetMethod(methodName, new Type[]{invokingObject.GetType()});
                if(genericType != null) method = method.MakeGenericMethod(genericType);
                List<object> extensionArgs = new List<object>(){invokingObject};
                for(int i = 0; i < args.Length; i++){
                    extensionArgs.Add(args[i]);
                }
                method.Invoke(null, extensionArgs.ToArray());
            }catch(Exception e){
                throw e;
            }
        }

        public static void BackUpFile(string dir, string fileName, string extension, int depth = 1){ 
            string fullPath = dir + fileName + extension;
            if(!Directory.Exists(dir + "/backup/")) Directory.CreateDirectory(dir + "/backup/");
            for(int i = depth - 1 ; i > 0; i--){
                string prevPath = dir + "/backup/" + fileName + "-backup-" + (i-1) + extension;
                string curPath = dir + "/backup/" + fileName + "-backup-" + i + extension;
                if(File.Exists(prevPath)){
                    File.Copy(prevPath,curPath,true);
                }
            }
            if(File.Exists(fullPath)){
                File.Copy(fullPath, dir + "/backup/" + fileName + "-backup-0" + extension, true);
            }
        }

        public static void SerializeAndSaveToFile<T>(T obj, string dir, string filename, string extension){
            try{
                
                if(!Directory.Exists(dir)){
                    Directory.CreateDirectory(dir);
                }

                if(!ValidateSerialization<T>(obj)){
                    Debug.Log("Validation failed, save aborted");
                    return;
                }

                using(StreamWriter sw = File.CreateText(dir+filename+extension)){
                    sw.Write(JsonConvert.SerializeObject(obj, Formatting.Indented, baseSerializerSettings));
                    sw.Close();
                }
            }catch(System.Exception e){
                throw e;
            }
        }

        public static T LoadFromSerializedFile<T>(string path) where T : new(){
            try{
                if(File.Exists(path)){
                    string contents = "";
                    using(StreamReader sr = new StreamReader(path)){
                        contents = sr.ReadToEnd();
                        sr.Close();
                    }
                    if(contents.Equals("")) return default(T);
                    return JsonConvert.DeserializeObject<T>(contents, baseSerializerSettings);
                }
            }catch(System.Exception e){
                Debug.LogWarning(e);
            }
            return new T();
        }

        public static string GetFileContents(string path){
            try{
                if(File.Exists(path)){
                    string contents = "";
                    using(StreamReader sr = new StreamReader(path)){
                        contents = sr.ReadToEnd();
                        sr.Close();
                    }
                    return contents;
                }
            }catch(System.Exception e){
                Debug.LogWarning(e);
            }
            return null;
        }

        public static void DeleteFile(string path){
            if(File.Exists(path))
                File.Delete(path);
        }

        public static void DeleteFolderContents(string path){
            try{
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.EnumerateFiles()){
                    file.Delete(); 
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories()){
                    dir.Delete(true); 
                }
            }catch(Exception e){
                throw e;
            }
        }

        public static Boolean ValidateSerialization<T>(T obj){
            try{
                JsonConvert.SerializeObject(obj,baseSerializerSettings);
                return true;
            }catch(Exception e){
                Debug.Log(e);
                return false;
            }
        }

        public static async void Delay<T,J>(EventManager<T> eventManager, J args, int delay = 0)where J : T{
            await System.Threading.Tasks.Task.Delay(delay);
            eventManager.Raise<J>(args);
        }

        public static async void Delay(Action action, int delay = 0){
            await System.Threading.Tasks.Task.Delay(delay);
            action.Invoke();
        }

        public static byte RandomByte(bool includeZero = false){
            System.Random random = new System.Random();
            byte[] bytes = new byte[1];
            do{
                new System.Random().NextBytes(bytes);
            }while(includeZero == true || bytes[0] != 0);
            return bytes[0];
        }

        public static byte RandomByte(Byte[] exclusions, bool includeZero = false){
            System.Random random = new System.Random();
            byte[] bytes = new byte[1];
            do{
                new System.Random().NextBytes(bytes);
            }while(exclusions.Contains(bytes[0]) && (includeZero == true || bytes[0] != 0));
            return bytes[0];
        }

    }

    public class UnityHelpers{

        public static bool IsCurrentScene(string name){
            Scene currentScene = SceneManager.GetActiveScene();
            return currentScene.name == name;
        }

        public static bool SceneExists(string name){
            if(string.IsNullOrEmpty(name))
            return false;

            for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++){
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                int lastSlash = scenePath.LastIndexOf("/");
                string sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

                if(string.Compare(name, sceneName, true) == 0)
                    return true;
            }

            return false;
        }

        public static async void LoadSceneAsync(string sceneName, Action callback = null){
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while(!op.isDone){
                await Task.Delay(1);
            }    
            if(callback != null) callback.Invoke();        
        }

        public static async void UnloadSceneAsync(string sceneName, Action callback = null){
            AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
            while(!op.isDone){
                await Task.Delay(1);
            }    
            if(callback != null) callback.Invoke();   
        }

        public static string GetSceneName(){
            return SceneManager.GetActiveScene().name;
        }

    }

    public class ProvenceMath{

        public static Vector3 FloorVector3(Vector3 point){
            return new Vector3(Mathf.Floor(point.x),Mathf.Floor(point.y),Mathf.Floor(point.z));
        }

        public static float DiagonalDistance(Vector3 start, Vector3 end){
            float dx = end.x - start.x;
            float dy = end.y - start.y;
            float dz = end.z - start.z;
            return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy), Mathf.Abs(dz));
            
        }

    }
    
}