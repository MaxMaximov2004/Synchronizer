using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synchronizer_Core.Vault_Manager;

namespace Synchronizer_Core.Synch
{
    public interface ISynch
    {
        public void Add_Path(String sync_path, String dist_path);
        public void Synchronize(Manage_Type manage_type = Manage_Type.Standart);

        public LinkedList<Managed_Data> Export_Data();
    }
}
