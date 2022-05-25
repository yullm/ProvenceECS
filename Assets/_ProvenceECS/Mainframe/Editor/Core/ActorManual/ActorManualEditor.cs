using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using ProvenceECS.Mainframe;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public class ActorManualEditorUIDirectory : UIDirectory{
        public ActorManualEditorUIDirectory(){
            this.uxmlPath = provenceEditorRoot + @"/Core/ActorManual/ActorManualEditor.uxml";
            this.ussPaths = new string[]{
                provenceEditorRoot + @"/Core/ActorManual/ActorManualEditor.uss"
            };
        }
    }

    public partial class ActorManualEditor : ProvenceCollectionEditor<ActorManualEntry>{
            
        #region ActorManual windowMethods
 
        protected override void SetEditorSettings(){
            this.titleContent = new GUIContent("Actor Manual");
            this.uiDirectory = new ActorManualEditorUIDirectory();
        }

        protected override void LoadCollection(){
            collection = ProvenceManager.Collections<ActorManualEntry>();
        }

        #endregion

    }

}