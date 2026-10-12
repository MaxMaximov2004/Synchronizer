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

                Debug.WriteLine("Selected:");
                Debug.WriteLine(context.Selected_Unit.Name);
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)//Отладка
        {
            Debug.WriteLine("Не работает этоот метод!");

        }




        
    }
}