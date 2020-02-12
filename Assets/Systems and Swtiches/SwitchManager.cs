using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProvenceECS{

    public class SwitchManager : MonoBehaviour{

        public World world;
        public List<string> switches = new List<string>();
        public string fileName;
        private string path = @".\switches\";

        public void InitializeSwitches(){
            world.eventManager.AddListener<SetSwitchEvent>(SetSwitch);
            if(File.Exists(path + fileName)){
                using(StreamReader sr = File.OpenText(path + fileName)){
                    string currentSwitch;
                    while((currentSwitch = sr.ReadLine()) != null){
                        switches.Add(currentSwitch);
                    }
                    sr.Close();
                }

            } 
        }

        private void SetSwitch(SetSwitchEvent args){
            
            if(args.switchName.Equals("")){ 
                new EventHandle(world,args.OnCompleteCallback).Raise();
                return;
            }
            if(!switches.Contains(args.switchName)){ 
                switches.Add(args.switchName);
                world.eventManager.Raise<OnSwitchSetEvent>(OnSwitchSetEvent.CreateInstance(args.switchName));
                new EventHandle(world,args.OnCompleteCallback).Raise();
            }
        }

        public bool IsSet(string switchName){
            return(switches.Contains(switchName));
        }

        public bool ValidateSwitchConditons(List<StringConditional> list){
            foreach(StringConditional current in list){
                if(current.conditionName.Equals("")) continue;
                bool contains = switches.Contains(current.conditionName);
                if(contains != current.conditionState) return false;
            }
            return true;
        }

        private void SaveToFile(){
            Directory.CreateDirectory(path);
            if(!File.Exists(path + fileName)) File.Create(path + fileName);
            using(StreamWriter sw = new StreamWriter(path + fileName)){
                foreach(string currentSwitch in switches){
                    sw.WriteLine();
                }
                sw.Close();
            }
        }
        
    }

}