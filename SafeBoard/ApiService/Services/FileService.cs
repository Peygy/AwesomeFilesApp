using System.Text;

namespace ApiService.Services
{
    /// <summary>
    /// Сервис FileService для управления файлами, использующиеся для создания архивов.
    /// </summary>
    public class FileService
    {
        private readonly string filesDirPath;
        private readonly ILogger<FileService> logger;

        /// <summary>
        /// Конструктор класса FileService.
        /// Инициализирует путь к директории файлов и логгер.
        /// Создает директорию, если она не существует, и инициализирует файлы.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения, содержащая пути к файлам.</param>
        /// <param name="logger">Логгер для ведения журнала операций.</param>
        public FileService(IConfiguration configuration, ILogger<FileService> logger)
        {
            // Получение путь к директории файлов из конфигурации
            filesDirPath = configuration.GetSection("Paths:Files").Value;

            // Инициализация логгера
            this.logger = logger;

            // Проверка, существует ли директория, и её создание, если не существует
            if (!Directory.Exists(filesDirPath))
            {
                // Создание директории по заданному пути
                Directory.CreateDirectory(filesDirPath);
                // Логирование информации о созданной директории
                logger.LogInformation($"Создана директория {filesDirPath}");

#if DEBUG
                // Инициализация файлов в директории
                InitAwesomeFiles();
#endif
            }
        }

        /// <summary>
        /// Инициализирует несколько файлов в директории filesDirPath.
        /// Создает 5 файлов с предопределенным текстом.
        /// </summary>
        private void InitAwesomeFiles()
        {
            // Создаем 5 файлов с предопределенным текстом
            for (int i = 1; i <= 5; i++)
            {
                // Создание файла с задданным путем
                using (var fileStream = File.Create(Path.Combine(filesDirPath, $"file{i}")))
                {
                    // Установка заготовленного текста для записи его в файл
                    var fileText = new UTF8Encoding(true).GetBytes($"File{i} text!");
                    // Запись текста в файл
                    fileStream.Write(fileText, 0, fileText.Length);
                }
            }

            // Логирование информации о созданных файлах
            logger.LogInformation("Созданы 5 файлов в директории files");
        }

        /// <summary>
        /// Возвращает список имен всех файлов в директории filesDirPath.
        /// </summary>
        /// <returns>Перечисление строк с именами файлов.</returns>
        public IEnumerable<string> GetFilesNames()
        {
            // Возврат списка всех файлов в директории
            return Directory.GetFiles(filesDirPath);
        }

        /// <summary>
        /// Возвращает список полных путей к файлам на основе их имен.
        /// </summary>
        /// <param name="fileNames">Список имен файлов.</param>
        /// <returns>Перечисление строк с полными путями к файлам.</returns>
        public IEnumerable<string> GetFilesPaths(List<string> fileNames)
        {
            // Обновление каждого элемента списка, добавляя к нему путь к директории
            for (int i = 0; i < fileNames.Count; i++)
            {
                fileNames[i] = Path.Combine(filesDirPath, fileNames[i]);
            }

            // Возврат списка полных путей к файлам
            return fileNames;
        }

        /// <summary>
        /// Проверяет, существует ли файл с заданным именем в директории filesDirPath.
        /// </summary>
        /// <param name="fileName">Имя файла для проверки.</param>
        /// <returns>true, если файл существует, иначе false.</returns>
        public bool FileExistCheck(string fileName)
        {
            // Проверка существования файла в директории
            return File.Exists(Path.Combine(filesDirPath, fileName));
        }
    }
}
