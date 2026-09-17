using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Synchronizer_Core.Vault_Manager;
using System.Diagnostics;

namespace Synchronizer_Core
{
    public enum Manage_Type:byte 
    {
        Standart, //Закрепляем файл с самым последней датой изменения, если где-то файла нет устанавливаем имеющейся
        Ignore, //Закрепляем файл с самым последней датой изменения, если где-то файла игнорируем её
        Delete //Закрепляем файл с самым последней датой изменения, если файла нет в sync, то удаляем файл (если есть) из dist

    }


    //Управляем только директориями, при синхронизации ищем файлы в директории и такие же в папки синхронизации
    public class Directory_Manager
    {
        public LinkedList<String> Error { get; set; } = new LinkedList<string> { };
        protected LinkedList<Tuple<string, string>> dirs;
        //Item 1 = Source
        //Item 2 = Distination

        public Directory_Manager() { dirs = new LinkedList<Tuple<string, string>>();    }
        public Directory_Manager(LinkedList<Managed_Data> data) {

            dirs = new LinkedList<Tuple<string, string>>();
            foreach (Managed_Data path in data) { dirs.AddLast(Tuple.Create(path.Source,path.Distination)); }
        }

        // sync_path - папка которую необходимо синхронизировать
        // dist_path - папка в которой будет хранится синхронизируемая папка (вместе с версиями)
        // Получение 2-ух директорий, проверка то что они существуют и настроука директории синхронизации
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

        public void Synchronize(Manage_Type manage_type = Manage_Type.Standart)
        {

            foreach (Tuple<String, String> dir in dirs)
            {
                DirectoryInfo source = new DirectoryInfo(dir.Item1);
                DirectoryInfo dist = new DirectoryInfo(dir.Item2);

                FileInfo[] dist_files = dist.GetFiles();
                foreach (FileInfo source_file in source.GetFiles())
                {
                    FileInfo? dist_file = null;

                    try
                    {
                        dist_file = dist_files.First(d_inf => (d_inf.Name == source_file.Name));
                    } catch (Exception) 
                    { }

                    Console.WriteLine($"\t\tdist>>{(dist_file==null?"NULL":dist_file.FullName)}");

                    if (dist_file!=null)
                    {
                        Console.WriteLine($"{source_file.FullName} <src|dst> {dist_file.FullName}");


                        if (dist_file.LastWriteTime > source_file.LastWriteTime)
                        {

                            dist_file.CopyTo(source_file.FullName,true);
                        } else
                        {

                            source_file.CopyTo(dist_file.FullName,true);
                        }

                    }
                    else
                    {
                        Console.WriteLine($"{source_file.FullName} <src|dst> create new");

                        
                        source_file.CopyTo(
                            Path.Combine(dist.FullName,source_file.Name),true);


                    }



                }
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
