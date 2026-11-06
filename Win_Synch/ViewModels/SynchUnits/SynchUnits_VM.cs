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


namespace Win_Synch.ViewModels.SynchUnits
{

    public class SynchUnits_VM : BaseSynchUnit
    {
        public Vault_Manager? Vault_Manager { get; set; } = null;


        private string _name = "NULL :(";
        private SimpleCommand _sync;
        private ISynch? _synchManager;
        public Manage_Type Manage_Type { get; set; } = Manage_Type.Full;


        public override string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    base.OnPropertyChanged();
                }
            }
        }

        public override SimpleCommand Sync
        {
            get { return _sync; }
            protected set
            {
                if (_sync != value)
                {
                    _sync = value;
                    base.OnPropertyChanged();
                }
            }
        }

        public override ISynch? Synch_Manager
        {
            get { return _synchManager; }
            protected set
            {
                if (_synchManager != value)
                {
                    _synchManager = value;
                    base.OnPropertyChanged();
                }
            }
        }

        public LinkedList<Managed_Data> Data { get { return Synch_Manager?.Export_Data() ?? new LinkedList<Managed_Data>(); } }

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
                _synchManager = new Directory_Synch(Vault_Manager.Get());
                _sync = new SimpleCommand(exe_Sync, can_exe_Sync);
                _name = Path.GetFileName(path_json);
            } 
        }

        protected SynchUnits_VM() { _sync = new SimpleCommand(exe_Sync, can_exe_Sync); }
        protected void Init(string path_json, string source, string distination)
        {

            _synchManager = new Directory_Synch();
            _synchManager.Add_Path(source, distination);

            _name = Path.GetFileName(path_json);
            Vault_Manager = new JSON_Manager();
            File.Create(path_json).Close();
            ((JSON_Manager)Vault_Manager).Open(path_json);
            ((JSON_Manager)Vault_Manager).Save(_synchManager.Export_Data());

        }

        /*Используемые команды*/
        
        virtual protected void exe_Sync(object? manager)
        {
            //Debug.WriteLine($"Vault val: {Vault_Manager}, Synch val: {Synch_Manager}");
            Debug.WriteLine("Begin synch");
            if (Synch_Manager != null) 
            {
                Thread thread = new Thread(() => { Synch_Manager.Synchronize(Manage_Type); });
                thread.Start();
            }
            //Synch_Manager?.Synchronize();
        }
        virtual protected bool can_exe_Sync(object? manager)
        {
            //return (Synch_Manager == null); //Что-то не понятное, почему-то считает что Synch_Manager не существует
            return true;
        }
        

        
    }

      
    


    //Необходимые данные для создания новой еденицы синхронизации
    public class NewSynchUnits_VM:SynchUnits_VM
    {
        protected string dir;

        /*protected string name = "Укажите имя";
        public string? Name { get { return name; } set { name = value; OnPropertyChanged(); } }*/

        protected string? source_fold = null;
        public string? Source_Fold { get { return source_fold; } set { source_fold = value; OnPropertyChanged(); } }

        protected string? destination_fold = null;
        public string? Destination_Fold { get { return destination_fold; } set { destination_fold = value; OnPropertyChanged(); } }


        public NewSynchUnits_VM(string work_dir) {

            dir = work_dir;
           

            Save_data = new SimpleCommand(save, can_save);
            Select_Source = new SimpleCommand(select_source, (obj) => { return true; });
            Select_Destination = new SimpleCommand(select_distination, (obj) => { return true; });
        }

        /*Комманды*/
        public SimpleCommand Save_data { get; set; }
        protected void save(object? obj)
        {
            //some magic with close window
            Init(Path.Combine(dir, base.Name.EndsWith(".json") ? base.Name : base.Name + ".json" ), source_fold, destination_fold);
            Debug.WriteLine("base Init!");
        }
        protected bool can_save(object? obj)
        {
            return Name!=null&&Name.Count()>0&&!Name.Contains(string.Concat(Path.GetInvalidFileNameChars()))
                &&Source_Fold!=null&&Directory.Exists(Source_Fold)
                &&Destination_Fold!=null&&Directory.Exists(Destination_Fold);
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
