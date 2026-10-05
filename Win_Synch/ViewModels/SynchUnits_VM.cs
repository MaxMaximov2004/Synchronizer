using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Synchronizer_Core.Synch;
using Synchronizer_Core.Vault_Manager;
using Win_Synch.Commands;


namespace Win_Synch.ViewModels
{
    public class SynchUnits_VM
    {
        public ISynch? Synch_Manager { get; set; } = null;
        public Vault_Manager? Vault_Manager { get; set; } = null;

        public string Name { 
            get {

                if ((JSON_Manager)Vault_Manager == null)
                {
                    return "NULL :(";
                }
                else
                {
                    return ((JSON_Manager)Vault_Manager).Name;
                }
            } 
        }

        /*
         * Пока что так, обязательно сделать конструктор более гибким:
            передаём название файла отслеживаемый папок, потом в конструкторе понимаем какое множество синхронизаторов можно применить к этим папкам
         */
        public SynchUnits_VM(string path_json)
        {
            Vault_Manager = new JSON_Manager();
            
            try
            {
                ((JSON_Manager)Vault_Manager).Open(path_json);
            }
            catch (Exception ex) {
                Vault_Manager = null;
            }

            if (Vault_Manager != null) 
            { 
                Synch_Manager = new Directory_Synch(Vault_Manager.Get());
                Sync = new SimpleCommand(exe_Sync, can_exe_Sync);
            }
        }

        /*public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }*/

        /*Используемые команды*/
        public SimpleCommand Sync { get; set; }

        protected void exe_Sync(object? manager)
        {

            Synch_Manager?.Synchronize();
        }
        protected bool can_exe_Sync(object? manager)
        {
            return true;
        }
    }
}
