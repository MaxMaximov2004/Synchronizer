using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Synchronizer_Core;
using Win_Synch.ViewModels;
using Microsoft.Win32;
using Synchronizer_Core.Vault_Manager;
using System.Diagnostics;

namespace Win_Synch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
                
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindow_VM(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Win_Synch", "Data.txt"));

        }

        //Это и прочие обработчики кликов ниже нужно заменить на команды
        private void MenuItem_Click(object sender, RoutedEventArgs e)//создание еденицы синхронизации
        {
            var context = this.DataContext as MainWindow_VM;
            if (context != null) {
                var FolderDialogSource = new OpenFolderDialog();
                string? new_source = null;
                string? new_synch = null;
                
                if (FolderDialogSource.ShowDialog()??false)
                {
                    new_source = FolderDialogSource.FolderName;
                }

                var FolderDialogSynch = new OpenFolderDialog();
                if (FolderDialogSynch.ShowDialog() ?? false)
                {
                    new_synch = FolderDialogSynch.FolderName;
                }

                if ((new_source != null) && (new_synch != null))
                {

                    JSON_Manager manager = new JSON_Manager();
                    {
                        DateTime now = DateTime.Now;
                        string path = System.IO.Path.Combine(context.path_working_dir, $"{now.Year}_{now.Day}_{now.DayOfWeek}  {now.Hour}-{now.Minute}-{now.Second}.json");
                        File.Create(path).Close();
                        manager.Open(path);
                    }
                    LinkedList<Tuple<string,string>> tuples = new LinkedList<Tuple<string,string>>();
                    //tuples.AddLast(Tuple.Create(new_source,new_synch));

                    DirectoryInfo sorce_dir = new DirectoryInfo(new_source);
                    DirectoryInfo synch_dir = new DirectoryInfo(new_synch);

                    Debug.WriteLine($"{new_source} {new_synch}\nFiles:");


                    foreach (var source_file in sorce_dir.GetFiles()) {

                        tuples.AddLast(Tuple.Create(
                            System.IO.Path.Combine(new_synch,source_file.Name),
                            source_file.FullName
                                ));
                        Debug.WriteLine($"source:{source_file.FullName} dist:{System.IO.Path.Combine(new_synch, source_file.Name)}");
                    }

                    /*foreach(var source_file in sorce_dir.GetFiles())
                    {
                        foreach(var synch_files in synch_dir.GetFiles())
                        {

                            tuples.AddLast(Tuple.Create(synch_files.FullName, source_file.FullName));
                            Debug.WriteLine($"source:{source_file.FullName} dist:{synch_files.FullName}");
                        }
                    }*/


                    manager.Save(tuples);
                    context.Manage_Units.Add(manager);
                }
            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)//Отладка
        {
            var context = this.DataContext as MainWindow_VM;
            if (context != null)
            {
                Debug.WriteLine($"{context.path_working_dir}");
                Debug.WriteLine($"{context.Manage_Units.Count}");
                foreach(var json in context.Manage_Units)
                {
                    Debug.WriteLine(json);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)//Синхронизация
        {
            
            JSON_Manager? item = (sender as Button).CommandParameter as JSON_Manager;
            if(item != null)
            {
                Debug.WriteLine("Beging synch");

                Directory_Manager directory = new Directory_Manager();
                LinkedList<Tuple<string, string>> files = item.Get();

                
                foreach (var file in files) { 
                    directory.Add_Path(file.Item1, file.Item2);
                    Debug.WriteLine($"{file.Item1} <+> {file.Item2}");
                }

                directory.Synchronize();
                Debug.WriteLine("End synch");

                foreach (var error in directory.Error)
                {
                    Debug.WriteLine(error);
                }
            }
        }
    }
}