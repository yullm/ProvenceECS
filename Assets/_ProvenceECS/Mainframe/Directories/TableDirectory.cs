using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public enum TableDirectoryKey {DIRECTORY,FILE,EXTENSION};

    public class TableDirectory : MonoBehaviour{

        public static readonly string ROOT = @"./_ProvenceECSData/Mainframe";

        private static readonly Dictionary<string, Dictionary<TableDirectoryKey,string>> corePaths = new Dictionary<string, Dictionary<TableDirectoryKey,string>>(){
            {
                "worlds", new Dictionary<TableDirectoryKey, string>{
                    {TableDirectoryKey.DIRECTORY,   ROOT+@"/Worlds/"},
                    {TableDirectoryKey.FILE,        @""},
                    {TableDirectoryKey.EXTENSION,   @".meglo"}
                }
            },{
                "temp", new Dictionary<TableDirectoryKey, string>{
                    {TableDirectoryKey.DIRECTORY,   ROOT+@"/temp/"},
                    {TableDirectoryKey.FILE,        @""},
                    {TableDirectoryKey.EXTENSION,   @".meglo"}
                }
            },{
                "temp-editors", new Dictionary<TableDirectoryKey, string>{
                    {TableDirectoryKey.DIRECTORY,   ROOT+@"/temp/editors/"},
                    {TableDirectoryKey.FILE,        @""},
                    {TableDirectoryKey.EXTENSION,   @".meglo"}
                }
            },{
                "actor-manual", new Dictionary<TableDirectoryKey, string>{
                    {TableDirectoryKey.DIRECTORY,   ROOT+@"/ActorManual/"},
                    {TableDirectoryKey.FILE,        @"ActorManual"},
                    {TableDirectoryKey.EXTENSION,   @".meglo"}
                }
            },{
                "actor-manual-keys", new Dictionary<TableDirectoryKey, string>{
                    {TableDirectoryKey.DIRECTORY,   @"./Assets/_ProvenceECS/Mainframe/ActorManual/"},
                    {TableDirectoryKey.FILE,        @"ActorManualKeys"},
                    {TableDirectoryKey.EXTENSION,   @".cs"}
                }
            },{
                "asset-manager", new Dictionary<TableDirectoryKey, string>{
                    {TableDirectoryKey.DIRECTORY,   ROOT+@"/AssetManager/"},
                    {TableDirectoryKey.FILE,        @"AssetManager"},
                    {TableDirectoryKey.EXTENSION,   @".meglo"}
                }
            }
        };

        private static readonly Dictionary<string, Dictionary<TableDirectoryKey,string>> additionalPaths = new Dictionary<string, Dictionary<TableDirectoryKey,string>>(){
            
        };

        public static string GetPath(string key){
            if(corePaths.ContainsKey(key)){
                return corePaths[key][TableDirectoryKey.DIRECTORY] + corePaths[key][TableDirectoryKey.FILE] + corePaths[key][TableDirectoryKey.EXTENSION];
            }
            if(additionalPaths.ContainsKey(key)){
                return additionalPaths[key][TableDirectoryKey.DIRECTORY] + additionalPaths[key][TableDirectoryKey.FILE] + additionalPaths[key][TableDirectoryKey.EXTENSION];
            }
            return "";
        }

        public static string GetSubKey(string key, TableDirectoryKey subKey){
            if(corePaths.ContainsKey(key)){
                if(corePaths[key].ContainsKey(subKey)) return corePaths[key][subKey];
            }
            if(additionalPaths.ContainsKey(key)){
                if(additionalPaths[key].ContainsKey(subKey)) return additionalPaths[key][subKey];
            }
            return "";
        }
        
    }

}
