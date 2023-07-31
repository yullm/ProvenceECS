using ProvenceECS;
using UnityEditor;

namespace ProvenceECS.Mainframe{

    public class ProvenceManagerShortcuts{

        [MenuItem("ProvenceECS/Duplicate Entities &d")]
        public static void DuplicateSelection(){
            ProvenceManagerEditor window = ProvenceManagerEditor.GetWindow<ProvenceManagerEditor>();
            window.DuplicateSelectedEntities();
        }

        [MenuItem("ProvenceECS/Delete Entities &x")]
        public static void RemoveSelection(){
            ProvenceManagerEditor window = ProvenceManagerEditor.GetWindow<ProvenceManagerEditor>();
            window.DeleteSelectedEntities();
        }

    }

}