using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public class SystemPackageManagerEditor : ProvenceCollectionEditorBase<SystemPackageManager, SystemPackage>{
        
        protected Texture delIcon;

        protected override void SetEditorSettings(){
            this.uiKey = "system-package-manager";
            this.titleContent = new GUIContent("System Package Manager");
            this.delIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/times.png");
        }

        protected override void RegisterEntryEditorEvents(){
            root.Q<Div>("entry-editor-menu-add-system-button").eventManager.AddListener<MouseClickEvent>(AddSystemButtonPressed);
        }

        protected override bool DrawEntryEditor(){
            if(base.DrawEntryEditor()){
                DrawSystems();
            }
            return true;
        }

        protected void DrawSystems(){
            Div container;
            ListItem titleItem = DrawShelf("Systems", out container);

            ListItemSearchBar searchBar = new ListItemSearchBar(container);
            container.Add(searchBar);

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

                container.Add(item);
                alternate = !alternate;
            }
            entryEditorScroller.Add(titleItem,container);
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
            collection = ProvenceManager.SystemPackageManager;
        }
    }

}