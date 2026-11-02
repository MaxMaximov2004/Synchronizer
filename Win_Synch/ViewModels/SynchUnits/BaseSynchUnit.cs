using Synchronizer_Core.Synch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Win_Synch.Commands;

namespace Win_Synch.ViewModels.SynchUnits
{
    public abstract class BaseSynchUnit:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public abstract string Name { get; set; }
        public abstract SimpleCommand Sync { get; protected set; }
        public abstract ISynch? Synch_Manager { get; protected set; }
    }
}
