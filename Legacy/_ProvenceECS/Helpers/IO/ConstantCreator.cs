using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS.Mainframe.IO{

    public class ConstantCreator{
        
        public string namespacePath;
        public string className;
        public HashSet<string> constants;

        public ConstantCreator(string className, params string[] constants){
            this.className = className;
            this.namespacePath = "";
            this.constants = new HashSet<string>(constants);
        }

        public ConstantCreator(string className, string namespacePath, params string[] constants) : this(className,constants){
            this.namespacePath = namespacePath;
        }

        public void Save(string dir, string fileName){
            string path = dir + fileName;
            if(!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if(File.Exists(path)) File.Delete(path);
            FileStream fs = new FileStream(path,FileMode.CreateNew);
            if(!className.Equals("")){
                using(StreamWriter sr = new StreamWriter(fs)){
                    bool hasNamespace = !namespacePath.Equals("");
                    sr.WriteLine("using System.Collections.Generic;");
                    if(hasNamespace) sr.WriteLine("namespace " + namespacePath +"{");

                    sr.WriteLine( (hasNamespace ? "\t" : "") + "public class " + className + "{");

                    HashSet<string> validNames = new();
                    foreach(string c in constants){
                        string validName = ValidateName(c);
                        validNames.Add(validName);
                        sr.WriteLine((hasNamespace ? "\t" : "") + "\tpublic static readonly string " + validName +" = " + "\"" + c +"\";");
                    }
                    
                    sr.WriteLine((hasNamespace ? "\t" : "") + "\tpublic static HashSet<string> all = new(){");
                    foreach(string validName in validNames){
                        sr.WriteLine((hasNamespace ? "\t" : "") + $"\t\t{validName},");
                    }
                    sr.WriteLine((hasNamespace ? "\t" : "") + "\t};");

                    sr.WriteLine((hasNamespace ? "\t" : "") + "}");

                    if(hasNamespace) sr.WriteLine("}");
                    sr.Flush();
                    sr.Close();
                }
            }
        }

        protected string ValidateName(string name){
            name = name.Replace(" ","");
            name = name.Replace("-","");
            return name;
        }
        
    }
}