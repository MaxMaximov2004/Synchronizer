using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synchronizer_Core.Vault_Manager;

namespace Synchronizer_Core.Synch
{
    public enum Manage_Type : byte
    {
        Join, //Синхронизируем только файлы с одинкаовми именами
        Source_Join, //Синхронизируем только файлы с одинкаовми именами И переносим из DIST все не существующий файлы
        Dist_Join, //Синхронизируем только файлы с одинкаовми именами И переносим из SOURCE все не существующий файлы
        Full //Синхронизируем только файлы с одинкаовми именами И синхронизируем DIST, SOURCE что-бы во всех папках были одинаковые файлы

    }

    public interface ISynch
    {
        public abstract void Add_Path(String sync_path, String dist_path);
        public abstract void Synchronize(Manage_Type manage_type = Manage_Type.Full);

        public abstract LinkedList<Managed_Data> Export_Data();
    }
}
