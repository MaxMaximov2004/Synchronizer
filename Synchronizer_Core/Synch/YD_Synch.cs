using Synchronizer_Core.Vault_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Synchronizer_Core.Synch
{

    class YD_Data
    {
        public string path = "disk:";
        public int limit = 20;
        public int offset = 0;
        public string fields = "";

    }

    public class YD_Synch : ISynch
    {
        /*Поля класса*/
        protected LinkedList<Managed_Data> dirs = new LinkedList<Managed_Data>();
        public LinkedList<Exception> Errors { get; set;} = new LinkedList<Exception>();


        /*необходимые методы*/
        public LinkedList<Managed_Data> Export_Data()
        {
            return dirs;
        }

        public void Synchronize(Manage_Type manage_type = Manage_Type.Full)
        {
            throw new NotImplementedException();
        }

        public void Add_Path(string sync_path, string dist_path)
        {

            if (string.IsNullOrWhiteSpace(sync_path))
            { throw new ArgumentException("Путь синхронизации не может быть пустым", nameof(sync_path)); }

            if (string.IsNullOrWhiteSpace(dist_path))
            { throw new ArgumentException("URL дистанционного хранилища не может быть пустым", nameof(dist_path)); }

            if (!Directory.Exists(sync_path))
            {
                throw new DirectoryNotFoundException($"Синхронизируемая директория не найдена: {sync_path}");
            }
            if (!IsYandexDiskUrl(dist_path))
            {
                throw new ArgumentException(
                    $"Неверный URL Yandex Disk. Ожидается формат: https://cloud-api.yandex.net/v1/disk/..., получено: {dist_path}",
                    nameof(dist_path));
            }


            LinkedList<DirectoryInfo> _source_dirs = new LinkedList<DirectoryInfo>(); //Список всех доступных локальных папок
            _source_dirs.AddFirst(new DirectoryInfo(sync_path));
            {
                DirectoryInfo _sync_info = new DirectoryInfo(sync_path);
                HashSet<DirectoryInfo> _not_checked_dirs = new HashSet<DirectoryInfo>();
                foreach (var _not_checked_dir in _sync_info.GetDirectories()) { _not_checked_dirs.Add(_not_checked_dir); }

                while (_not_checked_dirs.Count > 0)
                {
                    HashSet<DirectoryInfo> child_dirs = new HashSet<DirectoryInfo>();
                    foreach (DirectoryInfo dir in _not_checked_dirs)
                    {
                        foreach (DirectoryInfo child_dir in dir.GetDirectories())
                        {
                            child_dirs.Add(child_dir);

                        }

                        _source_dirs.AddLast(dir);
                    }
                    _not_checked_dirs = child_dirs;
                }
            }

            
            Uri _dist_dirs = new Uri(dist_path);
            {
                DirectoryInfo root = (new DirectoryInfo(sync_path)).Parent ?? (new DirectoryInfo(sync_path)).Root;

            }
        }

        /*Дополниительные мтеоды*/
        private bool IsYandexDiskUrl(string url)
        {
            

            // Быстрая проверка формата без создания Uri
            if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!url.Contains("yandex.net"))
                return false;

            if (!url.Contains("/v1/disk/"))
                return false;

            // Полнота проверки через Uri для дополнительной безопасности
            try
            {
                var uri = new Uri(url);
                return uri.Scheme == Uri.UriSchemeHttps &&
                       uri.Host.EndsWith("yandex.net") &&
                       uri.AbsolutePath.StartsWith("/v1/disk/");
            }
            catch (UriFormatException)
            {
                return false;
            }
        }
    }
}
