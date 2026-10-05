using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synchronizer_Core.Synch
{
    public interface ISynch
    {
        public void Add_Path(String sync_path, String dist_path);
    }
}
