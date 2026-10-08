using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Synchronizer_Core.Vault_Manager;
using System.Diagnostics;

namespace Synchronizer_Core.Synch
{
    public enum Manage_Type:byte 
    {
        Join, //Синхронизируем только файлы с одинкаовми именами
        Source_Join, //Синхронизируем только файлы с одинкаовми именами И переносим из DIST все не существующий файлы
        Dist_Join, //Синхронизируем только файлы с одинкаовми именами И переносим из SOURCE все не существующий файлы
        Full //Синхронизируем только файлы с одинкаовми именами И синхронизируем DIST, SOURCE что-бы во всех папках были одинаковые файлы

    }


    //Управляем только директориями, при синхронизации ищем файлы в директории и такие же в папки синхронизации
    public class Directory_Synch:ISynch
    {
        public LinkedList<String> Errors  { get; set; } = new LinkedList<string> { };
        protected LinkedList<Tuple<string, string>> dirs;
        //Item 1 = Source
        //Item 2 = Distination

        public Directory_Synch() { dirs = new LinkedList<Tuple<string, string>>();    }
        public Directory_Synch(LinkedList<Managed_Data> trusted_data) {

            dirs = new LinkedList<Tuple<string, string>>();
            foreach (Managed_Data path in trusted_data) { dirs.AddLast(Tuple.Create(path.Source,path.Distination)); }
        }



        // sync_path - папка которую необходимо синхронизировать
        // dist_path - папка в которой будет хранится синхронизируемая папка (вместе с версиями)
        // Получение 2-ух директорий, проверка то что они существуют и настройка директории синхронизации
        public void Add_Path(String sync_path, String dist_path)
        {

            if (Directory.Exists(sync_path) && Directory.Exists(dist_path))
            {

                DirectoryInfo sync_info = new DirectoryInfo(sync_path);
                DirectoryInfo dist_info = new DirectoryInfo(dist_path);

                if (!dist_info.EnumerateDirectories().Any(inf => (inf.Name == sync_info.Name)))
                {
                    dist_info.CreateSubdirectory(sync_info.Name);
                    dist_info = new DirectoryInfo(Path.Combine(dist_path, sync_info.Name));
                } else
                {
                    dist_info = new DirectoryInfo(Path.Combine(dist_path, sync_info.Name));
                }

                dirs.AddLast(
                    Tuple.Create(sync_info.FullName, dist_info.FullName)
                    );

                {
                    HashSet<DirectoryInfo> not_checked_dirs = new HashSet<DirectoryInfo>() { sync_info };

                    while (not_checked_dirs.Count > 0)
                    {

                        HashSet<DirectoryInfo> child_dirs = new HashSet<DirectoryInfo>();
                        foreach (DirectoryInfo dir in not_checked_dirs)
                        {
                            foreach(DirectoryInfo child_dir in dir.GetDirectories())
                            {
                                child_dirs.Add(child_dir);
                                
                                if (!Directory.Exists(
                                        Path.Combine(
                                            dist_info.FullName, $"{child_dir.FullName.Replace(sync_path + Path.DirectorySeparatorChar, "")}"
                                            )
                                    ))
                                {
                                    dist_info.CreateSubdirectory($"{child_dir.FullName.Replace(sync_path + Path.DirectorySeparatorChar, "")}");

                                }

                                dirs.AddLast(
                                    Tuple.Create(
                                        child_dir.FullName, 
                                        Path.Combine(
                                            dist_info.FullName, 
                                            $"{child_dir.FullName.Replace(sync_path + Path.DirectorySeparatorChar, "")}"
                                            ))
                                    );

                            }

                            
                        }
                        not_checked_dirs = child_dirs;


                    }

                }

                

            }
            else {

                throw new ArgumentException($"Directory: {sync_path} and {dist_path} must exist ");
            }
        }

        public void Synchronize(Manage_Type manage_type = Manage_Type.Join)
        {

            foreach (Tuple<String, String> dir in dirs)
            {
                DirectoryInfo source = new DirectoryInfo(dir.Item1);
                DirectoryInfo dist = new DirectoryInfo(dir.Item2);



                //FileInfo[] dist_files = dist.GetFiles();
                //foreach (FileInfo source_file in source.GetFiles())
                //{
                //    FileInfo? dist_file = null;

                //    try
                //    {
                //        dist_file = dist_files.First(d_inf => (d_inf.Name == source_file.Name));
                //    } catch (Exception) 
                //    { }

                    
                //    if (dist_file!=null)
                //    {
                        
                //        if (dist_file.LastWriteTime > source_file.LastWriteTime)
                //        {

                //            try {    dist_file.CopyTo(source_file.FullName, true);
                //            } catch (Exception ex) {
                //                Errors.AddLast($"{dist_file.FullName} >> {source_file.FullName}\nCatched: {ex.Message}");  }

                //        } else
                //        {

                //            try {    source_file.CopyTo(dist_file.FullName, true);
                //            } catch (Exception ex) {
                //                Errors.AddLast($"{source_file.FullName } >> {dist_file.FullName}\nCatched: {ex.Message}");  }
                //        }

                //    }
                //    else
                //    {
                        
                //        try {

                //            source_file.CopyTo(
                //                Path.Combine(dist.FullName, source_file.Name), true);
                //        } catch (Exception ex)
                //        {
                //            Errors.AddLast($"{source_file.FullName} >> create\n{ex.Message}");
                //        }
                //    }



                //}
            }
            
        }
        
        public LinkedList<Managed_Data> Export_Data()
        {
            LinkedList < Managed_Data > export = new LinkedList<Managed_Data>();

            foreach (Tuple<string,string> dir in dirs)
            {
                export.AddLast(
                    new Managed_Data(dir.Item2,dir.Item1)
                    );
            }

            return export;
        }
    }
}
