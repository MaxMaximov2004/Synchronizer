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


using Win_Synch.ViewModels;
using Microsoft.Win32;

using System.Diagnostics;

using Synchronizer_Core;
using Synchronizer_Core.Vault_Manager;
using Synchronizer_Core.Synch;

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
        //private void MenuItem_Click(object sender, RoutedEventArgs e)//создание еденицы синхронизации
        //{
        //    var context = this.DataContext as MainWindow_VM;
        //    if (context != null) {
        //        var FolderDialogSource = new OpenFolderDialog();
        //        string? new_source = null;
        //        string? new_dist = null;
                
        //        if (FolderDialogSource.ShowDialog()??false)
        //        {
        //            new_source = FolderDialogSource.FolderName;
        //        }

        //        var FolderDialogSynch = new OpenFolderDialog();
        //        if (FolderDialogSynch.ShowDialog() ?? false)
        //        {
        //            new_dist = FolderDialogSynch.FolderName;
        //        }

        //        if ((new_source != null) && (new_dist != null))
        //        {

        //            JSON_Manager manager = new JSON_Manager();
                    
        //                DateTime now = DateTime.Now;
        //                string path = System.IO.Path.Combine(context.path_working_dir, $"{now.Year}_{now.Day}_{now.DayOfWeek}  {now.Hour}-{now.Minute}-{now.Second}.json");
        //                File.Create(path).Close();
        //                manager.Open(path);
                    

        //            Directory_Synch directory_Manager = new Directory_Synch();
        //            directory_Manager.Add_Path(new_source, new_dist);
        //            //directory_Manager.Synchronize();

        //            manager.Save(directory_Manager.Export_Data());
        //            context.Manage_Units.Add(new SynchUnits_VM(path));

        //        }
        //    }

        //}

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

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)//Отладка
        {
            var context = this.DataContext as MainWindow_VM;
            if (context != null)
            { 
               
                foreach(String err in context.Manager.Errors)
                {
                    Debug.WriteLine(err);
                }
            
            }
        }



        //private void Button_Click(object sender, RoutedEventArgs e)//Синхронизация
        //{
            
        //    JSON_Manager? item = (sender as Button).CommandParameter as JSON_Manager;
        //    var context = this.DataContext as MainWindow_VM;

        //    if ((item != null)&&(context!=null))
        //    {
        //        LinkedList<Managed_Data>? dir_list = null;

        //        try
        //        {
        //            dir_list = item.Get();
        //        }
        //        catch (Exception ex) { }
                
        //        context.Manager = new Directory_Synch(dir_list);
        //        context.Manager.Synchronize();


        //    }
        //}

        
    }
}