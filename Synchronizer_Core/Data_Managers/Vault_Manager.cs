using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Synchronizer_Core.Vault_Manager
{
    public record Managed_Data
    {
        public String Distination {  get; set; }
        public String Source { get; set; }

        public Managed_Data(String distination, String source)
        {
            Distination = distination;
            Source = source;
        }

        public void Deconstruct(out string Item1, out string Item2)
        {
            Item1 = Source;
            Item2 = Distination;
        }
    }



    //Базовый абстрактный класс инкапсулирующий работу с конкретным хранилищем: реестр, отдельные файлы и тд. 
    abstract public class Vault_Manager
    {
        abstract public void Save(LinkedList<Managed_Data> list);
        abstract public LinkedList<Managed_Data> Get();
        
    }

    /*TODO реализовать этот класс*/
    public class Registry_Manager : Vault_Manager
    {
        public override LinkedList<Managed_Data> Get()
        {
            throw new NotImplementedException();
        }

        public override void Save(LinkedList<Managed_Data> list)
        {
            throw new NotImplementedException();
        }
    }
}
