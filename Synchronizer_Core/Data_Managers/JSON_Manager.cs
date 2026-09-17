using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;

namespace Synchronizer_Core.Vault_Manager
{

   
    public class JSON_Manager : Vault_Manager
    {
        protected bool JSON_Can_Open=false;
        protected String path = "";

        public String Name { get { return Path.GetFileName(path)??"Can`t open or not exist"; }  set { } }

        public JSON_Manager() {}
        public void Open(string path)
        {
            if (File.Exists(path)) { 
                var info = new FileInfo(path);
                if(info.Extension == ".json")
                {

                    this.path = path;   
                    JSON_Can_Open = true;
                }

            } else { throw new ArgumentException("File must exist!"); }
        }

        public override LinkedList<Managed_Data> Get()
        {
            if (!JSON_Can_Open) { throw new MemberAccessException("You must turn me available path to json file"); }
            using (var file = File.Open(path, FileMode.Open))
            {
                LinkedList<Managed_Data>? res = JsonSerializer.Deserialize<LinkedList<Managed_Data>>(file);

                if (res == null) { throw new ArgumentException("Something wrong with file! List is null"); }
                
                return res;
            }
        }

        public override void Save(LinkedList<Managed_Data> list)
        {
            if (!JSON_Can_Open) { throw new MemberAccessException("You must turn me available path to json file"); }

            //Debug.WriteLine("To Save:");
            //foreach (var item in list) {
            //    Debug.WriteLine($"{item.Source} > {item.Distination}");
            //}

            using (var file = File.Open(path,FileMode.Open))
            {
                JsonSerializer.Serialize<LinkedList<Managed_Data>>(file, list);
            }
        }

        public override string ToString()
        {
            string inf = $"\t{path}\n\tCan open:{JSON_Can_Open}";

            if (JSON_Can_Open) {
                using (var file = File.Open(path, FileMode.Open))
                {
                    LinkedList<Managed_Data>? res = JsonSerializer.Deserialize<LinkedList<Managed_Data>>(file);

                    if (res == null) { inf += $"\tCan`t desiralize"; }
                    else {
                        inf += $"\tsize: {res.Count}";
                    }
                    
                }
            }

            return inf;
        }
    }
}
