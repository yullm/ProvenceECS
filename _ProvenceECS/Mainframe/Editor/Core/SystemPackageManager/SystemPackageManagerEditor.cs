using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public class SystemPackageManagerUIDirectory : UIDirectory{
        public SystemPackageManagerUIDirectory(){
            this.uxmlPath = provenceEditorRoot + @"/Core/SystemPackageManager/SystemPackageManagerEditor.uxml";
            this.ussPaths = new string[]{
                provenceEditorRoot + @"/Core/SystemPackageManager/SystemPackageManagerEditor.uss"
            };
        }
    }

    public class SystemPackageManagerEditor : ProvenceCollectionEditor<SystemPackage>{
        
        protected Texture delIcon;

        [MenuItem("ProvenceECS/System Package Manager")]
        public static void ShowWindow(){
            SystemPackageManagerEditor window = GetWindow<SystemPackageManagerEditor>();
        }

        protected override void SetEditorSettings(){
            this.uiDirectory = new SystemPackageManagerUIDirectory();
            this.titleContent = new GUIContent("System Package Manager");
            this.delIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/times.png");
        }

        protected override void RegisterEntryEditorEvents(){
            root.Q<Div>("entry-editor-menu-add-system-button").eventManager.AddListener<MouseClickEvent>(AddSystemButtonPressed);
        }

        protected override void DrawEntryEditor(){        
            entryEditorScroller.Clear();    
            DrawEntryTitle();
            if(chosenKey != null){
                DrawSystems();
            }
        }

        protected void DrawSystems(){

            bool alternate = false;
            foreach(System.Type systemType in collection[chosenKey].systems){
                ListItem item = new ListItem(alternate);
                item.userData = systemType.Name;
                item.AddToClassList("search-list-item");
                item.AddTitle(Regex.Replace(systemType.Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0"));

                ListItemImage delButton = item.AddImage(delIcon);
                delButton.AddToClassList("icon","selectable","hoverable");
                delButton.eventManager.AddListener<MouseClickEvent>(e =>{
                    if(e.button != 0) return;
                    collection[chosenKey].systems.Remove(systemType);
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
                });                

                entryEditorScroller.Add(item);
                alternate = !alternate;
            }
        }

        protected void AddSystemButtonPressed(MouseClickEvent e){
            if(e.button != 0) return;
            List<System.Type> existingTypes = collection[chosenKey].systems.ToList();
            TypeSelector.TypeSelectorParameters searchParameters = new TypeSelector.TypeSelectorParameters(typeof(ProvenceSystem), false, existingTypes, typeof(Entity));
            TypeSelector.Open(searchParameters, args =>{
                collection[chosenKey].systems.Add(args.value);
                collection[chosenKey].systems = new HashSet<System.Type>(collection[chosenKey].systems.OrderBy(system => system.Name));
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
            });
        }

        protected override void LoadCollection(){
            collection = ProvenceManager.Collections<SystemPackage>();
        }
    }

}