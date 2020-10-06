using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public class SystemEditor : MainframeTableWindow<ProvenceSystem>{

        public static void Show<T>(ProvenceSystem system) where T : ProvenceSystem{
            SystemEditor window = GetWindow<SystemEditor>();
            window.titleContent = new GUIContent(Regex.Replace(typeof(T).Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0") + " Editor");
            window.eventManager.Raise<SelectKey<ProvenceSystem>>(new SelectKey<ProvenceSystem>(system));
        }

        protected override void SetEditorSettings(){
            this.titleContent = new GUIContent("System Editor");
            this.uiKey = "system-editor";
        }

        protected override void RegisterEventListeners(){
            eventManager.AddListener<SelectKey<ProvenceSystem>>(SelectKey);
            eventManager.AddListener<DrawColumnEventArgs<ProvenceSystem>>(DrawColumn);
            eventManager.AddListener<SceneSavedEvent>(SaveSystem);
        }

        protected override void InitializeWindow(){
            if(root == null) return;
        }

        protected override void SelectKey(SelectKey<ProvenceSystem> args){
            chosenKey = args.key;
            eventManager.Raise<DrawColumnEventArgs<ProvenceSystem>>(new DrawColumnEventArgs<ProvenceSystem>(0));
        }

        protected override void DrawColumn(DrawColumnEventArgs<ProvenceSystem> args){
            switch(args.column){
                case 0:
                    SetTitle();
                    if(chosenKey != null) Helpers.InvokeGenericMethod(this, "DrawSystemControl", chosenKey.GetType());
                    break;
            }
        }

        protected void SetTitle(){
            if(chosenKey != null){
                string systemName = Regex.Replace(chosenKey.GetType().Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
                root.Q<ListItemText>("system-title").text = systemName + " Editor";
            }
        }

        protected void DrawSystemControl<T>() where T : ProvenceSystem{
            ColumnScroller scroller = root.Q<ColumnScroller>("editor-scroller");
            T system = (T)chosenKey;
            if(scroller != null){
                scroller.Clear();
                StructureControl<T> control = new StructureControl<T>(ref system, system.GetType().Name, false, chosenKey.world);
                control.eventManager.AddListener<StructureControlUpdated<T>>(e =>{
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                });
                scroller.Add(control);
            }
        }

        protected void SaveSystem(SceneSavedEvent args){
            ProvenceManager.Instance.Save();
        }
    }

}