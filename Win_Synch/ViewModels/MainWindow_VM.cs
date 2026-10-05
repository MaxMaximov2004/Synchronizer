using Synchronizer_Core;
using Synchronizer_Core.Vault_Manager;
using Synchronizer_Core.Synch;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using Win_Synch.Commands;

namespace Win_Synch.ViewModels
{
    public class MainWindow_VM
    { 
        protected string path_data = ""; //файл данных, что-бы был
        
        //P.S. убрать модификатор public после искоренения всех обработчиков!!! НЕ ЗАБЫТЬ ПРО ЭТО!!!
        public string path_working_dir = ""; //рабочая директория для создания Json-файлов



        public ObservableCollection<SynchUnits_VM> Manage_Units { get; set; }
        public Directory_Synch Manager { get; set; }


        //path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Win_Synch", "Data.txt")
        public MainWindow_VM(string path)
        {
            path_data = path;
            path_working_dir = Path.GetDirectoryName(path) ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Win_Synch");
            Manage_Units = new ObservableCollection<SynchUnits_VM>();
            Manager = new Directory_Synch();

            

            if (!File.Exists(path))
            {
                
                Directory.CreateDirectory(path_working_dir);
                File.Create(path_data).Close();

            } else
            {
                

                /*сканер рабочей папки на наличие json файлов едениц синхронизации*/
                foreach (var file in new DirectoryInfo(path_working_dir).EnumerateFiles()) {

                    if (file.Extension == ".json")
                    {
                        JSON_Manager manager = new JSON_Manager();
                        manager.Open(file.FullName);

                        try
                        {
                            manager.Get();

                            Manage_Units.Add(new SynchUnits_VM(file.FullName));

                        }
                        catch (Exception ex)
                        {
                            file.Delete();
                        }

                    }
                }

            }

            
        }


        

        public SimpleCommand Create_Fold { get; set; }
    
    }
}
