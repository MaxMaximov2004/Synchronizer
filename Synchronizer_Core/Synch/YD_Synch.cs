using Synchronizer_Core.Vault_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace Synchronizer_Core.Synch
{

    /// <summary>
    /// Перечисление доступных полей для запросов к Yandex Disk API
    /// </summary>
    public enum YD_Field
    {
        /// <summary>
        /// Путь к ресурсу
        /// </summary>
        path,

        /// <summary>
        /// Тип ресурса (file или folder)
        /// </summary>
        type,

        /// <summary>
        /// Имя ресурса
        /// </summary>
        name,

        /// <summary>
        /// Размер ресурса в байтах
        /// </summary>
        size,

        /// <summary>
        /// Дата последнего изменения
        /// </summary>
        modified,

        /// <summary>
        /// Дата создания ресурса
        /// </summary>
        created,

        /// <summary>
        /// Ссылка на ресурс
        /// </summary>
        resource_id,

        /// <summary>
        /// MIME-тип файла
        /// </summary>
        mime_type,

        /// <summary>
        /// Ссылка на файл
        /// </summary>
        file,

        /// <summary>
        /// Список вложенных ресурсов
        /// </summary>
        items,

        /// <summary>
        /// Количество вложенных ресурсов
        /// </summary>
        _embedded,

        /// <summary>
        /// Ссылка на родительскую папку
        /// </summary>
        parent,

        /// <summary>
        /// Статус ресурса
        /// </summary>
        status,

        /// <summary>
        /// Ссылка на публичный ресурс
        /// </summary>
        public_url,

        /// <summary>
        /// Статус публикации
        /// </summary>
        publish_status,

        /// <summary>
        /// Статус доступа
        /// </summary>
        access,

        /// <summary>
        /// Статус владельца
        /// </summary>
        owner,

        /// <summary>
        /// Статус обновления
        /// </summary>
        updated,

        /// <summary>
        /// Статус удаления
        /// </summary>
        deleted,

        /// <summary>
        /// Статус восстановления
        /// </summary>
        restored,

        /// <summary>
        /// Статус перемещения
        /// </summary>
        moved,

        /// <summary>
        /// Статус копирования
        /// </summary>
        copied,

        /// <summary>
        /// Статус синхронизации
        /// </summary>
        synced,

        /// <summary>
        /// Статус загрузки
        /// </summary>
        uploaded,

        /// <summary>
        /// Статус скачивания
        /// </summary>
        downloaded,

        /// <summary>
        /// Статус обработки
        /// </summary>
        processed,

        /// <summary>
        /// Статус ошибки
        /// </summary>
        error,

        /// <summary>
        /// Статус успеха
        /// </summary>
        success,

        /// <summary>
        /// Статус завершения
        /// </summary>
        completed,

        /// <summary>
        /// Статус начала
        /// </summary>
        started,

        /// <summary>
        /// Статус ожидания
        /// </summary>
        waiting,

        /// <summary>
        /// Статус активности
        /// </summary>
        active,

        /// <summary>
        /// Статус неактивности
        /// </summary>
        inactive,

        /// <summary>
        /// Статус блокировки
        /// </summary>
        locked,

        /// <summary>
        /// Статус разблокировки
        /// </summary>
        unlocked,

        /// <summary>
        /// Статус архивации
        /// </summary>
        archived,

        /// <summary>
        /// Статус разархивации
        /// </summary>
        unarchived,

        /// <summary>
        /// Статус резервного копирования
        /// </summary>
        backup,

        /// <summary>
        /// Статус восстановления
        /// </summary>
        restore,

        /// <summary>
        /// Статус очистки
        /// </summary>
        clean,

        /// <summary>
        /// Статус проверки
        /// </summary>
        check,

        /// <summary>
        /// Статус анализа
        /// </summary>
        analyze,

        /// <summary>
        /// Статус оптимизации
        /// </summary>
        optimize,

        /// <summary>
        /// Статус компрессии
        /// </summary>
        compress,

        /// <summary>
        /// Статус декомпрессии
        /// </summary>
        decompress,

        /// <summary>
        /// Статус шифрования
        /// </summary>
        encrypt,

        /// <summary>
        /// Статус дешифрования
        /// </summary>
        decrypt,

        /// <summary>
        /// Статус подписи
        /// </summary>
        sign,

        /// <summary>
        /// Статус проверки подписи
        /// </summary>
        verify,

        /// <summary>
        /// Статус аутентификации
        /// </summary>
        authenticate,

        /// <summary>
        /// Статус авторизации
        /// </summary>
        authorize
    }


    public record YD_ApiData
    {
        public string path { get; set; } = "disk:";
        public int limit { get; set; } = 20;
        public int offset { get; set; } = 0;
        public string fields { get; set; } = "path,name,size"; // Сериализованные поля через запятую

        // Конструктор для удобства
        public YD_ApiData() { }

        public YD_ApiData(string path, int limit, int offset, string fields)
        {
            this.path = path;
            this.limit = limit;
            this.offset = offset;
            this.fields = fields;
        }

        //TODO Вырезать этот метод и найти что-нибудь что генерирует список строку запроса из переданной записи (тип как сериализатор Json)
        public string querry_string()
        {
            return new string($"path={path}&limit={limit}&offset={offset}&fields={fields}");
        }
    }
  
    public class YD_DataBuilder
    {
        private string _path = "disk:";
        private int _limit = 20;
        private int _offset = 0;
        private YD_Field[] _fields = { YD_Field.path, YD_Field.name, YD_Field.size };

        // Конструктор с валидацией
        public YD_DataBuilder(int limit = 20, int offset = 0)
        {
            Limit = limit;
            Offset = offset;
        }

        // Свойства с валидацией
        public int Limit
        {
            get => _limit;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Limit), "Limit cannot be less than zero");
                _limit = value;
            }
        }

        public int Offset
        {
            get => _offset;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Offset), "Offset cannot be less than zero");
                _offset = value;
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                if (!IsValidPath(value))
                    throw new ArgumentException("Path must be in format 'disk:' or 'disk:/folder1/folder2'", nameof(Path));
                _path = value;
            }
        }

        public YD_Field[] Fields
        {
            get => _fields;
            set => _fields = value ?? throw new ArgumentNullException(nameof(Fields));
        }
        private bool IsValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (path == "disk:")
                return true;

            if (path.StartsWith("disk:/"))
            {
                // Проверяем, что путь содержит только допустимые символы после disk:/
                var pathPart = path.Substring(5); // Убираем "disk:"
                return !pathPart.Contains("..") && !pathPart.Contains("//");
            }

            return false;
        }

        // Метод добавления папки в путь
        public void AddFolderToPath(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("Folder name cannot be null or empty", nameof(folderName));

            //Если получаем не наш разделитель, то меням его на наш
            if(folderName.Contains("\\")) { folderName.Replace("\\", "/"); }
            
            // Проверяем, что имя папки не содержит запрещенных символов - добавить если есть
            if ( folderName.Contains(":"))
                throw new ArgumentException("Folder name contains invalid characters", nameof(folderName));

            if (_path == "disk:")
            {
                _path = $"disk:/{folderName}";
            }
            else
            {
                // Убедимся, что путь заканчивается на /
                if (!_path.EndsWith("/"))
                    _path += "/";
                _path += folderName;
            }
        }

        // Метод очистки пути (оставляет только "disk:")
        public void ClearPath()
        {
            _path = "disk:";
        }

        // Метод для получения сериализуемого объекта
        public YD_ApiData ToApiData()
        {
            return new YD_ApiData
            {
                path = _path,
                limit = _limit,
                offset = _offset,
                fields = string.Join(",", _fields.Select(f => f.ToString()))
            };
        }

        // Метод для создания нового объекта с заданными полями
        public YD_DataBuilder WithFields(params YD_Field[] fields)
        {
            Fields = fields;
            return this;
        }

        // Метод для создания нового объекта с заданным путем
        public YD_DataBuilder WithPath(string path)
        {
            Path = path;
            return this;
        }

        // Метод для создания нового объекта с заданным лимитом
        public YD_DataBuilder WithLimit(int limit)
        {
            Limit = limit;
            return this;
        }

        // Метод для создания нового объекта с заданным офсетом
        public YD_DataBuilder WithOffset(int offset)
        {
            Offset = offset;
            return this;
        }

        // Метод для создания объекта с заданными параметрами
        public YD_DataBuilder WithAll(int limit = 20, int offset = 0, string path = "disk:", params YD_Field[] fields)
        {
            Limit = limit;
            Offset = offset;
            Path = path;
            Fields = fields;
            return this;
        }
    }

    public class YD_Synch : ISynch
    {
        /*Поля класса*/
        protected LinkedList<Managed_Data> dirs = new LinkedList<Managed_Data>();
        public LinkedList<Exception> Errors { get; set;} = new LinkedList<Exception>();
        private string token = string.Empty;
        private string adress = "https://cloud-api.yandex.net/v1/disk/resources";
        /**/
        public YD_Synch(string token)
        {
            this.token = token;
        }
        private YD_Synch() { }


        /*необходимые методы*/
        public LinkedList<Managed_Data> Export_Data()
        {
            return dirs;
        }

        public void Synchronize(Manage_Type manage_type = Manage_Type.Full)
        {
            throw new NotImplementedException();
        }

        public void Add_Path(string sync_path, string dist_folder_name)
        {

            if (string.IsNullOrWhiteSpace(sync_path))
            { throw new ArgumentException("Путь синхронизации не может быть пустым", nameof(sync_path)); }

            if (!Directory.Exists(sync_path))
            {
                throw new DirectoryNotFoundException($"Синхронизируемая директория не найдена: {sync_path}");
            }
            

            /*Обрабатываем все локальные папки (на ПК)*/
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

            /*Папки в удалённой директории*/
            
            LinkedList<string> _remotes = new LinkedList<string>();
            using (var http_client = new HttpClient())
            {
               {
                 string _source_dir_name = (new DirectoryInfo(sync_path)).Name;
                 YD_DataBuilder builder = new YD_DataBuilder();
                 builder.AddFolderToPath(dist_folder_name);
                 yandex_disc_create_folder(builder.ToApiData(), http_client, true);// это не правиьно нужно заставить создовать папки рекурсивно
                 builder.AddFolderToPath(_source_dir_name);

                 YD_ApiData data = builder.ToApiData();
                 yandex_disc_create_folder(data, http_client, true);
                 _remotes.AddLast(data.path);
               }

                var _local = new DirectoryInfo(sync_path);
               foreach (DirectoryInfo dir in _source_dirs)
               {
                    string reletive_path = Path.GetRelativePath(_local.FullName, dir.FullName);

                    if (reletive_path != ".")
                    {
                        YD_DataBuilder builder = new YD_DataBuilder();
                        builder.AddFolderToPath(dist_folder_name);
                        builder.AddFolderToPath(reletive_path); 

                        YD_ApiData data = builder.ToApiData();
                        yandex_disc_create_folder(data, http_client, true);
                        _remotes.AddLast(data.path);
                        
                    }

                }
            }

            dirs = new(_source_dirs.Zip(_remotes, (src, rmt) => new Managed_Data(rmt, src.FullName)));

        }

        /*Метод создания папки в удалённой директории*/
        /*Не может создавать сразу систему папок рекурсивно про это нужно помнить*/
        protected void yandex_disc_create_folder(YD_ApiData folder_data, HttpClient client, bool check)
        {

            UriBuilder addr_with_data = new UriBuilder(adress);
            addr_with_data.Query = folder_data.querry_string();
            
            var _req = new HttpRequestMessage( HttpMethod.Put, addr_with_data.Uri);
            HttpResponseMessage? _resp = null;

            try
            {
                _req.Headers.Add("Authorization", token);
                
                //_req.Content = JsonContent.Create(folder_data);
                _resp = client.SendAsync(_req).Result;
            }
            catch (Exception e) { Errors.AddLast(e); }

            if (_resp != null) {

                if (_resp.StatusCode == System.Net.HttpStatusCode.OK) {

                    Console.WriteLine(_resp.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Errors.AddLast(
                        new ArgumentException
                            ($"Получили статус: {_resp.StatusCode}\nЗапрос: {_req.ToString()}\t\t{_req.Content?.ReadAsStringAsync().Result}\nОтвет: {_resp.ToString()}\t\t{_resp.Content?.ReadAsStringAsync().Result}")
                        );
                }

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
