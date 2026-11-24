// file: Synchronizer_Core_Tests\Test1.cs
using Synchronizer_Core.Synch;
using System.IO;
using System;

namespace Synchronizer_Core_Tests
{
    [TestClass]
    public class DirectorySynchTests
    {
        private string _baseSourceDir;
        private string _baseDestDir;
        private string _actualSourceDir;
        private string _actualDestDir;
        private Directory_Synch _synch;

        [TestInitialize]
        public void Setup()
        {
            // Создаем уникальные временные папки для изоляции тестов
            string tempRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Tests", "SyncTests_" + Guid.NewGuid().ToString("N"));

            Directory.CreateDirectory(tempRoot);

            _baseSourceDir = Path.Combine(tempRoot, "Source");
            _baseDestDir = Path.Combine(tempRoot, "Destination");

            Directory.CreateDirectory(_baseSourceDir);
            Directory.CreateDirectory(_baseDestDir);


            _synch = new Directory_Synch();

            // Вызываем Add_Path. Он создаст подпапку с именем Source внутри Destination
            _synch.Add_Path(_baseSourceDir, _baseDestDir);

            // Фактические пути, где будут лежать файлы после Add_Path
            _actualSourceDir = _baseSourceDir;
            _actualDestDir = Path.Combine(_baseDestDir, new DirectoryInfo(_baseSourceDir).Name);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Очищаем временные папки после каждого теста
            if (Directory.Exists(Path.GetDirectoryName(_baseSourceDir)))
            {
                Directory.Delete(Path.GetDirectoryName(_baseSourceDir), true);
            }
        }

        // Вспомогательный метод для создания файлов с заданным временем изменения
        private void CreateFile(string directory, string fileName, string content, DateTime lastWriteTime)
        {
            string filePath = Path.Combine(directory, fileName);
            File.WriteAllText(filePath, content);
            File.SetLastWriteTime(filePath, lastWriteTime);
        }

        // Вспомогательный метод для создания иерархии папок с файлами
        private void CreateTestHierarchy(string rootDir, int maxDepth = 6, int filesPerDir = 10)
        {
            CreateDirectoryStructure(rootDir, "", maxDepth, filesPerDir);
        }

        // Рекурсивное создание структуры папок
        private void CreateDirectoryStructure(string rootDir, string currentPath, int depthRemaining, int filesPerDir)
        {
            if (depthRemaining <= 0) return;

            string fullPath = Path.Combine(rootDir, currentPath);
            Directory.CreateDirectory(fullPath);

            // Создаем файлы в текущей директории
            for (int i = 0; i < filesPerDir; i++)
            {
                string fileName = $"file_{i}.txt";
                string content = $"Content of file {i} in {currentPath}";
                string filePath = Path.Combine(fullPath, fileName);
                File.WriteAllText(filePath, content);

                // Устанавливаем разное время изменения для тестирования
                DateTime writeTime = DateTime.Now.AddHours(-i);
                File.SetLastWriteTime(filePath, writeTime);
            }

            // Создаем поддиректории
            for (int i = 0; i < 3; i++) // 3 поддиректории на каждом уровне
            {
                string subDirName = $"subdir_{i}";
                string newPath = string.IsNullOrEmpty(currentPath) ? subDirName : Path.Combine(currentPath, subDirName);
                CreateDirectoryStructure(rootDir, newPath, depthRemaining - 1, filesPerDir);
            }
        }

        #region 1. Тест начальной синхронизации (Full)

        [TestMethod]
        public void Synchronize_Full_InitialSetup_CopiesStructureAndFiles()
        {
            // Arrange: Создаем структуру с вложенными папками и файлами в Source
            string subDir = Path.Combine(_actualSourceDir, "SubFolder");
            Directory.CreateDirectory(subDir);

            _synch.Add_Path(_baseSourceDir, _baseDestDir);

            CreateFile(_actualSourceDir, "root_file.txt", "Root content", DateTime.Now.AddDays(-2));
            CreateFile(subDir, "nested_file.txt", "Nested content", DateTime.Now.AddDays(-1));

            // Act
            _synch.Synchronize(Manage_Type.Full);

            // Assert: Проверяем, что структура и файлы скопировались в Destination
            Assert.IsTrue(File.Exists(Path.Combine(_actualDestDir, "root_file.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(_actualDestDir, "SubFolder", "nested_file.txt")));

            Assert.AreEqual("Root content", File.ReadAllText(Path.Combine(_actualDestDir, "root_file.txt")));
            Assert.AreEqual("Nested content", File.ReadAllText(Path.Combine(_actualDestDir, "SubFolder", "nested_file.txt")));
        }

        #endregion

        #region 2. Тесты разрешения конфликтов (Join) - побеждает более новый файл

        [TestMethod]
        public void Synchronize_Join_SourceIsNewer_OverwritesDestination()
        {
            // Arrange
            CreateFile(_actualSourceDir, "conflict.txt", "Newer Source Data", DateTime.Now);
            CreateFile(_actualDestDir, "conflict.txt", "Older Dest Data", DateTime.Now.AddDays(-1));

            // Act
            _synch.Synchronize(Manage_Type.Join);

            // Assert: Файл в Destination должен перезаписаться данными из Source
            string destContent = File.ReadAllText(Path.Combine(_actualDestDir, "conflict.txt"));
            Assert.AreEqual("Newer Source Data", destContent);
        }

        [TestMethod]
        public void Synchronize_Join_DestinationIsNewer_OverwritesSource()
        {
            // Arrange
            CreateFile(_actualSourceDir, "conflict.txt", "Older Source Data", DateTime.Now.AddDays(-1));
            CreateFile(_actualDestDir, "conflict.txt", "Newer Dest Data", DateTime.Now);

            // Act
            _synch.Synchronize(Manage_Type.Join);

            // Assert: Файл в Source должен перезаписаться данными из Destination
            string sourceContent = File.ReadAllText(Path.Combine(_actualSourceDir, "conflict.txt"));
            Assert.AreEqual("Newer Dest Data", sourceContent);
        }

        [TestMethod]
        public void Synchronize_Join_UniqueFiles_AreIgnored()
        {
            // Arrange: Уникальные файлы в каждой папке
            CreateFile(_actualSourceDir, "unique_source.txt", "Source only", DateTime.Now);
            CreateFile(_actualDestDir, "unique_dest.txt", "Dest only", DateTime.Now);

            // Act
            _synch.Synchronize(Manage_Type.Join);

            // Assert: Join синхронизирует ТОЛЬКО файлы с одинаковыми именами. Уникальные не трогаются.
            Assert.IsFalse(File.Exists(Path.Combine(_actualDestDir, "unique_source.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(_actualSourceDir, "unique_dest.txt")));
        }

        #endregion

        #region 3. Тесты режимов Source_Join и Dist_Join (Перенос уникальных файлов)

        [TestMethod]
        public void Synchronize_SourceJoin_CopiesUniqueFromDestToSource()
        {
            // Arrange
            CreateFile(_actualSourceDir, "common.txt", "Source", DateTime.Now.AddDays(-1));
            CreateFile(_actualDestDir, "common.txt", "Dest", DateTime.Now); // Dest новее

            CreateFile(_actualDestDir, "unique_in_dest.txt", "Unique Dest", DateTime.Now);
            CreateFile(_actualSourceDir, "unique_in_source.txt", "Unique Source", DateTime.Now);

            // Act
            _synch.Synchronize(Manage_Type.Source_Join);

            // Assert: 
            // 1. Уникальный файл из Dest перенесся в Source
            Assert.IsTrue(File.Exists(Path.Combine(_actualSourceDir, "unique_in_dest.txt")));
            // 2. Общий файл обновился (так как Dest новее)
            Assert.AreEqual("Dest", File.ReadAllText(Path.Combine(_actualSourceDir, "common.txt")));
            // 3. Уникальный файл из Source НЕ должен был попасть в Dest (логика Source_Join)
            Assert.IsFalse(File.Exists(Path.Combine(_actualDestDir, "unique_in_source.txt")));
        }

        [TestMethod]
        public void Synchronize_DistJoin_CopiesUniqueFromSourceToDest()
        {
            // Arrange
            CreateFile(_actualSourceDir, "unique_in_source.txt", "Unique Source", DateTime.Now);
            CreateFile(_actualDestDir, "unique_in_dest.txt", "Unique Dest", DateTime.Now);

            // Act
            _synch.Synchronize(Manage_Type.Dist_Join);

            // Assert: Уникальный файл из Source перенесся в Dest
            Assert.IsTrue(File.Exists(Path.Combine(_actualDestDir, "unique_in_source.txt")));
            // Уникальный файл из Dest НЕ должен попасть в Source
            Assert.IsFalse(File.Exists(Path.Combine(_actualSourceDir, "unique_in_dest.txt")));
        }

        #endregion

        #region 4. Тест режима Full (Полная двусторонняя синхронизация)

        [TestMethod]
        public void Synchronize_Full_SyncsBothUniqueAndConflictingFiles()
        {
            // Arrange
            // Конфликтный файл (Source новее)
            CreateFile(_actualSourceDir, "conflict.txt", "New Source", DateTime.Now);
            CreateFile(_actualDestDir, "conflict.txt", "Old Dest", DateTime.Now.AddDays(-1));

            // Уникальные файлы
            CreateFile(_actualSourceDir, "only_source.txt", "Src", DateTime.Now);
            CreateFile(_actualDestDir, "only_dest.txt", "Dst", DateTime.Now);

            // Act
            _synch.Synchronize(Manage_Type.Full);

            // Assert: Обе папки должны стать полностью идентичными
            // 1. Конфликт разрешен в пользу Source
            Assert.AreEqual("New Source", File.ReadAllText(Path.Combine(_actualDestDir, "conflict.txt")));
            Assert.AreEqual("New Source", File.ReadAllText(Path.Combine(_actualSourceDir, "conflict.txt")));

            // 2. Уникальные файлы скопированы в обе стороны
            Assert.IsTrue(File.Exists(Path.Combine(_actualDestDir, "only_source.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(_actualSourceDir, "only_dest.txt")));
        }

        #endregion

        #region 5. Тест эффективности работы с иерархией (Эффективность)

        [TestMethod]
        public void Synchronize_Efficiency_HierarchicalStructure_Test()
        {
            // Arrange: Создаем иерархию папок с глубиной 6 и 10 файлов в каждой
            CreateTestHierarchy(_actualSourceDir, 6, 10);

            // Act: Синхронизируем
            var startTime = DateTime.Now;
            _synch.Synchronize(Manage_Type.Full);
            var endTime = DateTime.Now;

            // Calculate duration
            var duration = endTime - startTime;

            // Assert: Проверяем, что синхронизация завершена успешно
            Assert.IsTrue(duration.TotalSeconds < 30, $"Синхронизация заняла слишком много времени: {duration.TotalSeconds:F2} секунд");

            // Проверяем, что структура скопировалась
            var destDirInfo = new DirectoryInfo(_actualDestDir);
            var sourceDirInfo = new DirectoryInfo(_actualSourceDir);

            // Проверяем, что количество файлов совпадает
            int sourceFileCount = sourceDirInfo.GetFiles("*", SearchOption.AllDirectories).Length;
            int destFileCount = destDirInfo.GetFiles("*", SearchOption.AllDirectories).Length;

            Assert.AreEqual(sourceFileCount, destFileCount, "Количество файлов в исходной и целевой папках должно совпадать");

            // Проверяем, что структура папок скопировалась
            Assert.IsTrue(destDirInfo.Exists, "Целевая директория должна существовать");
        }

        [TestMethod]
        public void Synchronize_Efficiency_Performance_Metrics()
        {
            // Arrange: Создаем большую иерархию для тестирования производительности
            CreateTestHierarchy(_actualSourceDir, 4, 15); // Меньше уровней для скорости

            // Act: Измеряем производительность
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _synch.Synchronize(Manage_Type.Full);
            stopwatch.Stop();

            // Assert: Проверяем, что операция завершена быстро
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000,
                $"Синхронизация завершена за {stopwatch.ElapsedMilliseconds} мс, что больше допустимого 5000 мс");

            // Проверяем успешное завершение
            Assert.IsTrue(_synch.Errors.Count == 0, "Синхронизация не должна содержать ошибок");
        }

        [TestMethod]
        public void Synchronize_Efficiency_FileSizes_Check()
        {
            // Arrange: Создаем файлы с размером до 1 Кбайта
            string testDir = Path.Combine(_actualSourceDir, "test_files");
            Directory.CreateDirectory(testDir);

            for (int i = 0; i < 20; i++)
            {
                string fileName = $"small_file_{i}.txt";
                string content = new string('A', 512); // 512 байт содержимого
                CreateFile(testDir, fileName, content, DateTime.Now);
            }

            // Act
            _synch.Synchronize(Manage_Type.Full);

            // Assert: Проверяем размеры файлов
            var destDir = new DirectoryInfo(_actualDestDir);
            var files = destDir.GetFiles("*.txt", SearchOption.AllDirectories);

            Assert.IsTrue(files.Length > 0, "Должны быть файлы в целевой директории");

            foreach (var file in files)
            {
                // Проверяем, что размер файла не превышает 1 Кбайт
                Assert.IsTrue(file.Length <= 1024, $"Файл {file.Name} превышает 1 Кбайт: {file.Length} байт");
            }
        }

        #endregion

        #region 6. Тесты с разными режимами синхронизации

        [TestMethod]
        public void Synchronize_Efficiency_Mode_Comparison()
        {
            // Arrange: Создаем тестовую структуру
            CreateTestHierarchy(_actualSourceDir, 3, 5);

            // Act: Тестируем разные режимы
            var modes = new[] { Manage_Type.Full, Manage_Type.Join, Manage_Type.Source_Join, Manage_Type.Dist_Join };
            var results = new Dictionary<Manage_Type, TimeSpan>();

            foreach (var mode in modes)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _synch.Synchronize(mode);
                stopwatch.Stop();
                results[mode] = stopwatch.Elapsed;

                // Очищаем для следующего теста
                if (Directory.Exists(_actualDestDir))
                {
                    Directory.Delete(_actualDestDir, true);
                    Directory.CreateDirectory(_actualDestDir);
                    _synch.Add_Path(_baseSourceDir, _baseDestDir);
                }
            }

            // Assert: Все режимы должны завершиться за разумное время
            foreach (var kvp in results)
            {
                Assert.IsTrue(kvp.Value.TotalSeconds < 10,
                    $"Режим {kvp.Key} завершился за {kvp.Value.TotalSeconds:F2} секунд");
            }
        }

        #endregion
    }
}
