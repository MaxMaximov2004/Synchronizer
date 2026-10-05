using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
            //Debug.WriteLine($"Vault val: {Vault_Manager}, Synch val: {Synch_Manager}");
            Synch_Manager?.Synchronize();
        }
        protected bool can_exe_Sync(object? manager)
        {
            //return (Synch_Manager == null); //Что-то не понятное, почему-то считает что Synch_Manager не существует
            return true;
        }
    }


    //Необходимые данные для создания новой еденицы синхронизации
    public class NewSynchUnits_VM 
    {
        public String? Name { get; set; } = "Укажите имя";
        public String? Source_Fold { get; set; } = null;
        public String? Destination_Fold { get; set; } = null;

        protected ObservableCollection<SynchUnits_VM> Manage_Units;

        public NewSynchUnits_VM(ObservableCollection<SynchUnits_VM> manage_units) { 

            Manage_Units = manage_units; 
            
        }

        /*Комманды*/
        public SimpleCommand Save_data { get; set; }
        protected void save(object? obj)
        {
            //some magic with close window
        }
        protected bool can_save(object? obj)
        {
            return (Name!=null)&&(Name.Count()>0)&&(Name.Contains(String.Concat(Path.GetInvalidFileNameChars())))
                &&(Source_Fold!=null)&&(Directory.Exists(Source_Fold))
                &&(Destination_Fold!=null)&&(Directory.Exists(Destination_Fold));
        }


        public SimpleCommand Select_Source { get; set; }
        protected void select_source(object? obj)
        {

        }


        public SimpleCommand Select_Destination { get; set; }
        protected void select_distination(object? obj)
        {

        }
    }
}
