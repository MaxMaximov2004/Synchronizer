using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Synchronizer_Core
{
    public enum Manage_Type:byte 
    {
        Standart, //Закрепляем файл с самым последней датой изменения, если где-то файла нет устанавливаем имеющейся
        Ignore, //Закрепляем файл с самым последней датой изменения, если где-то файла игнорируем её
        Delete //Закрепляем файл с самым последней датой изменения, если файла нет в sync, то удаляем файл (если есть) из dist

    }


    public class Directory_Manager
    {
        protected LinkedList<Tuple<string, string>> files;


        public Directory_Manager() {    files = new LinkedList<Tuple<string, string>>();    }
        public LinkedList<String> Error { get; set; } = new LinkedList<string> { };

        public void Add_Path(String dist_path, String sync_path)
        {

            if (File.Exists(sync_path))
            {
                files.AddLast( Tuple.Create(dist_path, sync_path));
            } else
            {
                throw new ArgumentException("Sync path is invalid! It`s must be exist");
            }
        }

        public void Synchronize(Manage_Type manage_type = Manage_Type.Standart)
        {
        
            /*Придумать эффективный механизм разрешения типа управления, по умолчанию - Standart*/
            foreach(Tuple<string, string> file in files) 
            { 
                
                /*Если где-то нет папок - создаём их*/
                if( !Path.Exists(file.Item2)) 
                { 
                    Directory.CreateDirectory(
                        file.Item2.Trim(
                            Path.GetFileName(
                                file.Item2).ToString().ToCharArray())); 
                }

                if (!Path.Exists(file.Item1))
                {
                    Directory.CreateDirectory(
                        file.Item1.Trim(
                            Path.GetFileName(
                                file.Item1).ToString().ToCharArray()));
                }

                /*first-dist  second-sync*/
                if (File.Exists(file.Item1)&&File.Exists(file.Item2))
                {
                    DateTime date_dist = new FileInfo(file.Item1).LastWriteTime;
                    DateTime date_sync = new FileInfo(file.Item2).LastWriteTime;

                    if (date_sync > date_dist)
                    {
                        File.Copy(file.Item2, file.Item1, true);
                    } else
                    {
                        File.Copy(file.Item1, file.Item2, true);
                    }

                } else
                {
                    Error.AddLast($"{DateTime.Now} files not exist:{ (File.Exists(file.Item1) ? file.Item1: "") } { (File.Exists(file.Item2)? file.Item2:"") } {(File.Exists(file.Item2) && File.Exists(file.Item1)?"Can`t create":"Create")}");
                    
                    if (!File.Exists(file.Item1))
                    {
                       File.Copy(file.Item2,file.Item1, true);
                    } else {

                        if (!File.Exists(file.Item2)) {
                            File.Copy(file.Item1,file.Item2, true);
                        }
                    }

                }


            }
            

        }
        

    }
}
