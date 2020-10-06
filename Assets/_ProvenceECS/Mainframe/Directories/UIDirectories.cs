using System.Collections;
using System.Collections.Generic;

namespace ProvenceECS.Mainframe{

    public class UIDirectories{

        private static readonly string root = "Assets/_ProvenceECS/Mainframe/Editor";

        public static readonly Dictionary<string,Dictionary<string,string>> coreDirectory = new Dictionary<string, Dictionary<string, string>>(){
            {
                "base", new Dictionary<string, string>(){
                    {"uss",          "/Core/_Base/MainframeTable.uss"},
                    {"uxml",         "/Core/_Base/MainframeControls.uxml"},
                    {"inspector-uss","/Core/_Base/Unity Inspector/MainframeUnityInspector.uss"}
                }
            },{
                "collection-editor", new Dictionary<string, string>(){
                    {"uss", "/Core/_Base/ProvenceCollectionEditor/ProvenceCollectionEditorBase.uss"}
                }
            },{
                "provence-manager", new Dictionary<string, string>(){
                    {"cs",  "/Core/_ProvenceManager/ProvenceManagerEditor.cs"},
                    {"uxml","/Core/_ProvenceManager/ProvenceManagerEditor.uxml"},
                    {"uss", "/Core/_ProvenceManager/ProvenceManagerEditor.uss"}
                }
            },{
                "entity-selector", new Dictionary<string, string>(){
                    {"cs",  "/Core/EntitySelector/EntitySelector.cs"},
                    {"uxml","/Core/EntitySelector/EntitySelector.uxml"},
                    {"uss", "/Core/EntitySelector/EntitySelector.uss"}
                }
            },{
                "type-selector", new Dictionary<string, string>(){
                    {"cs",  "/Core/TypeSelector/TypeSelector.cs"},
                    {"uxml","/Core/TypeSelector/TypeSelector.uxml"},
                    {"uss", "/Core/TypeSelector/TypeSelector.uss"}
                }
            },{
                "hierarchy-selector", new Dictionary<string, string>(){
                    {"cs",  "/Core/HierarchySelector/HierarchySelector.cs"},
                    {"uxml","/Core/HierarchySelector/HierarchySelector.uxml"},
                    {"uss", "/Core/HierarchySelector/HierarchySelector.uss"}
                }
            },{
                "entity-editor", new Dictionary<string, string>(){
                    {"cs",  "/Core/EntityEditor/EntityEditor.cs"},
                    {"uxml","/Core/EntityEditor/EntityEditor.uxml"},
                    {"uss", "/Core/EntityEditor/EntityEditor.uss"}
                }
            },{
                "system-editor", new Dictionary<string, string>(){
                    {"cs",  "/Core/SystemEditor/SystemEditor.cs"},
                    {"uxml","/Core/SystemEditor/SystemEditor.uxml"},
                    {"uss", "/Core/SystemEditor/SystemEditor.uss"}
                }
            },{
                "entity-unity-inspector", new Dictionary<string, string>(){
                    {"cs",  "/Core/Entity Unity Inspector/EntityUnityInspector.cs"},
                    {"uxml","/Core/Entity Unity Inspector/EntityUnityInspector.uxml"},
                    {"uss", "/Core/Entity Unity Inspector/EntityUnityInspector.uss"}
                }
            },{
                "world-manager-unity-inspector", new Dictionary<string, string>(){
                    {"cs",  "/Core/_ProvenceManager/Scene Hook Unity Inspector/SceneHookInspector.cs"},
                    {"uxml","/Core/_ProvenceManager/Scene Hook Unity Inspector/SceneHookInspector.uxml"},
                    {"uss", "/Core/_ProvenceManager/Scene Hook Unity Inspector/SceneHookInspector.uss"}
                }
            },{
                "index-selector", new Dictionary<string, string>(){
                    {"cs",  "/Core/IndexSelector/IndexSelector.cs"},
                    {"uxml","/Core/IndexSelector/IndexSelector.uxml"},
                    {"uss", "/Core/IndexSelector/IndexSelector.uss"}
                }
            },{
                "actor-manual", new Dictionary<string, string>(){
                    {"cs",  "/Core/ActorManual/ActorManualEditor.cs"},
                    {"uxml","/Core/ActorManual/ActorManualEditor.uxml"},
                    {"uss", "/Core/ActorManual/ActorManualEditor.uss"}
                }
            },{
                "asset-manager", new Dictionary<string, string>(){
                    {"cs",  "/Core/AssetManager/AssetManagerEditor.cs"},
                    {"uxml","/Core/AssetManager/AssetManagerEditor.uxml"},
                    {"uss", "/Core/AssetManager/AssetManagerEditor.uss"}
                }
            },{
                "key-selector", new Dictionary<string, string>(){
                    {"cs",  "/Core/KeySelector/KeySelector.cs"},
                    {"uxml","/Core/KeySelector/KeySelector.uxml"},
                    {"uss", "/Core/KeySelector/KeySelector.uss"}
                }
            },{
                "system-package-manager", new Dictionary<string, string>(){
                    {"cs",  "/Core/SystemPackageManager/SystemPackageManagerEditor.cs"},
                    {"uxml","/Core/SystemPackageManager/SystemPackageManagerEditor.uxml"},
                    {"uss", "/Core/SystemPackageManager/SystemPackageManagerEditor.uss"}
                }
            }
        };

        public static readonly Dictionary<string,Dictionary<string,string>> additionsDirectory = new Dictionary<string, Dictionary<string, string>>(){
            
        };

        

        public static string GetPath(string pageName, string fileType){
            if(coreDirectory.ContainsKey(pageName)) return root + coreDirectory[pageName][fileType];
            if(additionsDirectory.ContainsKey(pageName)) return root + additionsDirectory[pageName][fileType];
            return "";
        }

        
    }
    
}