using System.Linq;
using ProvenceECS;
using ProvenceECS.Network;
using UnityEditor;

namespace ProvenceECS.Mainframe{

    public static class ProvenceManagerShortcuts{

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

        [MenuItem("ProvenceECS/Create Asset Library")]
        public static void CreateAssetLibrary(){
            AssetManager.CreateAssetLibrary();
        }

        [MenuItem("ProvenceECS/Generate Packets")]
        public static void GeneratePacketDictionary(){
            ProvenceNetwork.GeneratePacketDictionary();
            Helpers.SerializeAndSaveToFile(ProvenceNetwork.PacketDict.OrderBy(kvp => kvp.Key), ProvenceCollection<AssetData>.dataPath + "/Packets/", "provence-packets", ".meglo");
        }

    }

}