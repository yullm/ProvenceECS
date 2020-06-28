using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProvenceECS.Mainframe{

    public class SetSelectorParameters<T> : MainframeUIArgs{
        public T paramaters;

        public SetSelectorParameters(T paramaters){
            this.paramaters = paramaters;
        }
    }

    //public delegate void MainframeKeySelection<T> (T index);

    public class SelectorParameters{}

    public class MainframeKeySelection<T> : MainframeUIArgs{
        public T value;

        public MainframeKeySelection(T value){
            this.value = value;
        }
    };

    public abstract class MainframeSelectorWindow<T> : MainframeTableWindow<T>{
        
        protected ProvenceDelegate<MainframeKeySelection<T>> callback;

        public static M Open<M>(string title, ProvenceDelegate<MainframeKeySelection<T>> callback) where M : MainframeSelectorWindow<T>{
            M window = GetWindow<M>();
            window.titleContent = new GUIContent(title);
            window.eventManager.AddListener<MainframeKeySelection<T>>(callback);
            return window;
        }

        protected virtual void ReturnSelection(){
            if(chosenKey != null) eventManager.Raise<MainframeKeySelection<T>>(new MainframeKeySelection<T>(chosenKey));
            this.Close();
        }

    }

}