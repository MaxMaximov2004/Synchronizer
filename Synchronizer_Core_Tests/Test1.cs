using Synchronizer_Core.Synch;

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
                string tempRoot = Path.Combine(Path.GetTempPath(), "SyncTests_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempRoot);

                _baseSourceDir = Path.Combine(tempRoot, "Source");
                _baseDestDir = Path.Combine(tempRoot, "Destination");

                Directory.CreateDirectory(_baseSourceDir);
                // Destination оставляем пустой, как и требовалось в ТЗ

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

            #region 1. Тест начальной синхронизации (Full)

            [TestMethod]
            public void Synchronize_Full_InitialSetup_CopiesStructureAndFiles()
            {
                // Arrange: Создаем структуру с вложенными папками и файлами в Source
                string subDir = Path.Combine(_actualSourceDir, "SubFolder");
                Directory.CreateDirectory(subDir);

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
        }
    
}
