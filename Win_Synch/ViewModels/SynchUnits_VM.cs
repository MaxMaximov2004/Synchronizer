using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
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

        protected SynchUnits_VM() { Sync = new SimpleCommand(exe_Sync, can_exe_Sync); }
        protected void Init(string path_json, string source, string distination)
        {

            Synch_Manager = new Directory_Synch();
            Synch_Manager.Add_Path(source, distination);

            Vault_Manager = new JSON_Manager();
            File.Create(path_json).Close();
            ((JSON_Manager)Vault_Manager).Open(path_json);
            ((JSON_Manager)Vault_Manager).Save(Synch_Manager.Export_Data());

        }

        /*public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }*/

        /*Используемые команды*/
        public SimpleCommand Sync { get; set; }

        virtual protected void exe_Sync(object? manager)
        {
            //Debug.WriteLine($"Vault val: {Vault_Manager}, Synch val: {Synch_Manager}");
            Synch_Manager?.Synchronize();
        }
        virtual protected bool can_exe_Sync(object? manager)
        {
            //return (Synch_Manager == null); //Что-то не понятное, почему-то считает что Synch_Manager не существует
            return true;
        }
    }


    //Необходимые данные для создания новой еденицы синхронизации
    public class NewSynchUnits_VM:SynchUnits_VM, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        protected string dir;

        protected string name = "Укажите имя";
        public String? Name { get { return name; } set { name = value; OnPropertyChanged(); } }

        protected string? source_fold = null;
        public String? Source_Fold { get { return source_fold; } set { source_fold = value; OnPropertyChanged(); } }

        protected string? destination_fold = null;
        public String? Destination_Fold { get { return destination_fold; } set { destination_fold = value; OnPropertyChanged(); } }

        /*
         Для варианта если это не класс - наследник SynchUnits_VM - 
            те при создании мы вызваем его View в конструкторе которого вызывается и этот конструктор ViewModel, 
            при указании всех путей и имён метод save должен был создать объект JSON_Manager и SynchUnits_VM, и последний записать в ObservableCollection<SynchUnits_VM> Manage_Units, который мы получили из главной VM 
         */
        //protected ObservableCollection<SynchUnits_VM> Manage_Units;
        //public NewSynchUnits_VM(ObservableCollection<SynchUnits_VM> manage_units) { 
        //    Manage_Units = manage_units; 
        //}

        public NewSynchUnits_VM(string work_dir) {

            dir = work_dir;
            

            Save_data = new SimpleCommand(save, can_save);
            Select_Source = new SimpleCommand(select_source, (object? obj) => { return true; });
            Select_Destination = new SimpleCommand(select_distination, (object? obj) => { return true; });
        }

        /*Комманды*/
        public SimpleCommand Save_data { get; set; }
        protected void save(object? obj)
        {
            //some magic with close window

            //Debug.WriteLine(Name);
            //Debug.WriteLine(Source_Fold);
            //Debug.WriteLine(Destination_Fold);
            base.Init(Path.Combine(dir, (name.EndsWith(".json") ? name : name+".json") ), source_fold, destination_fold);
            Debug.WriteLine("base Init!");
        }
        protected bool can_save(object? obj)
        {
            return (Name!=null)&&(Name.Count()>0)&&(!Name.Contains(String.Concat(Path.GetInvalidFileNameChars())))
                &&(Source_Fold!=null)&&(Directory.Exists(Source_Fold))
                &&(Destination_Fold!=null)&&(Directory.Exists(Destination_Fold));
        }


        public SimpleCommand Select_Source { get; set; }
        protected void select_source(object? obj)
        {
            var FolderDialog = new OpenFolderDialog();
            if (FolderDialog.ShowDialog() ?? false)
            {
                    Source_Fold = FolderDialog.FolderName;
            }
        }


        public SimpleCommand Select_Destination { get; set; }
        protected void select_distination(object? obj)
        {
            var FolderDialog = new OpenFolderDialog();
            if (FolderDialog.ShowDialog() ?? false)
            {
                Destination_Fold = FolderDialog.FolderName;
            }
        }
    }
}
