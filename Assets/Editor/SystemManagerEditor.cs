using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProvenceECS{

    [CustomEditor(typeof(SystemManager))]
    public class SystemManagerEditor : Editor
    {
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
        }
    }
}