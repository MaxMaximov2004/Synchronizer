using System.Collections.Generic;
using System.Text.Json;

namespace Synchronizer_Core.Vault_Manager
{

   
    public class JSON_Manager : Vault_Manager
    {
        protected bool JSON_Can_Open=false;
        protected String path = "";


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

        public override LinkedList<Tuple<string, string>> Get()
        {
            if (!JSON_Can_Open) { throw new MemberAccessException("You must turn me available path to json file"); }
            using (var file = File.Open(path, FileMode.Open))
            {
                LinkedList<Tuple<string, string>>? res = JsonSerializer.Deserialize<LinkedList<Tuple<string, string>>>(file);

                if (res == null) { throw new ArgumentException("Something wrong with file! List is null"); }
                //JSON_File.Flush();
                return res;
            }
        }

        public override void Save(LinkedList<Tuple<string, string>> list)
        {
            if (!JSON_Can_Open) { throw new MemberAccessException("You must turn me available path to json file"); }
            using (var file = File.Open(path,FileMode.Open))
            {
                JsonSerializer.Serialize<LinkedList<Tuple<string, string>>>(file, list);
                //JSON_File.Flush();
            }
        }
    }
}
