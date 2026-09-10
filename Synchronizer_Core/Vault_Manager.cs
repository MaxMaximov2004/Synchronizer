using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Synchronizer_Core.Vault_Manager
{
    //public class Managed_Data
    //{
    //    public Managed_Data() {
    //        Data = new LinkedList<Tuple<string, string>>();
    //        Data_Name = "";
    //    }

    //    public Managed_Data(LinkedList<Tuple<string, string>> data, string data_name)
    //    {
    //        Data = data;
    //        Data_Name = data_name;
    //    }

    //    public LinkedList<Tuple<string,string>> Data { get; set; }
    //    public string Data_Name { get; set; }
    //}

    //Базовый абстрактный класс инкапсулирующий работу с конкретным хранилищем: реестр, отдельные файлы и тд. 
    
    abstract public class Vault_Manager
    {
        abstract public void Save(LinkedList<Tuple<string, string>> list);
        abstract public LinkedList<Tuple<string, string>> Get();
        
    }

    /*TODO реализовать этот класс*/
    public class Registry_Manager : Vault_Manager
    {
        public override LinkedList<Tuple<string, string>> Get()
        {
            throw new NotImplementedException();
        }

        public override void Save(LinkedList<Tuple<string, string>> list)
        {
            throw new NotImplementedException();
        }
    }
}
