using ApiService.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.IO.Compression;

namespace ApiService.Services
{
    /// <summary>
    /// Сервис ArchiveService для управления процессами создания архивов и самими архивами
    /// </summary>
    public class ArchiveService
    {
        private readonly string archivesDirPath;
        private readonly ConcurrentDictionary<int, ArchiveCreationProcess> processStatusStorage;
        private readonly ILogger<ArchiveService> logger;
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Конструктор класса ArchiveService.
        /// Инициализирует путь к директории архивов, хранилище статусов процессов и логгер.
        /// Создает директорию, если она не существует, либо очищает её, если существует.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения, содержащая пути к архивам.</param>
        /// <param name="logger">Логгер для ведения журнала операций.</param>
        /// <param name="memoryCache">Интерфейс для кэширования.</param>
        public ArchiveService(IConfiguration configuration, ILogger<ArchiveService> logger, IMemoryCache memoryCache)
        {
            // Получение пути, к директории архивов, из конфигурации
            archivesDirPath = configuration.GetSection("Paths:Archives").Value;

            // Инициализация логгера
            this.logger = logger;
            // Инициализация кэширования в памяти
            this.memoryCache = memoryCache;

            // Проверка существования директории, и её создание, если не существует
            if (!Directory.Exists(archivesDirPath))
            {
                // Создание директории по заданному пути
                Directory.CreateDirectory(archivesDirPath);
                // Логирование информации о созданной директории
                logger.LogInformation($"Создана директория {archivesDirPath}");
            }
            else
            {
                // Получение информации о директории
                var directoryInfo = new DirectoryInfo(archivesDirPath);

                // Очистка директории, если она существует
                foreach (var zipArchive in directoryInfo.GetFiles())
                {
                    // Удаление zip-архива
                    zipArchive.Delete();
                }

                // Логирование информации о пересоздании директории
                logger.LogInformation($"Директория {archivesDirPath} пересоздана");
            }

            // Инициализация хранилища статусов процессов
            processStatusStorage = new ConcurrentDictionary<int, ArchiveCreationProcess>();
        }

        /// <summary>
        /// Создает новый процесс архивации файлов.
        /// Возвращает идентификатор процесса.
        /// </summary>
        /// <param name="filesPaths">Перечисление путей к файлам, которые нужно заархивировать.</param>
        /// <returns>Идентификатор созданного процесса архивации.</returns>
        public int CreateNewProcess(IEnumerable<string> filesPaths)
        {
            // Создание нового процесса создания архива
            var newProcess = new ArchiveCreationProcess
            {
                Id = new Random().Next(100000, 1000000),
                Status = "Выполняется",
                FilePaths = filesPaths
            };
            // Указание пути расположения zip-архива
            newProcess.ZipFilePath = Path.Combine(archivesDirPath, $"{newProcess.Id}.zip");

            // Добавление процесса в хранилище
            processStatusStorage.TryAdd(newProcess.Id, newProcess);
            // Логирование информации о добавлении процесса
            logger.LogInformation($"Процесс {newProcess.Id} выполняется");

            // Запуск асинхронной задачи по созданию архива
            Task.Run(() => CreateArchive(newProcess));

            // Возврат идентификатора процесса
            return newProcess.Id;
        }

        /// <summary>
        /// Асинхронно создает архив из файлов, указанных в процессе.
        /// Обновляет статус процесса после завершения.
        /// </summary>
        /// <param name="newProcess">Процесс архивации с информацией о файлах.</param>
        private void CreateArchive(ArchiveCreationProcess newProcess)
        {
            try
            {
                // Создание файла для последующего создания zip-архива
                using (var zipStream = new FileStream(newProcess.ZipFilePath, FileMode.Create))
                {
                    // Создание нового zip-архива
                    using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                    {
                        // Проход по всем выбранным файлам
                        foreach (var filePath in newProcess.FilePaths)
                        {
                            // Добавление файла в архив
                            zipArchive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                        }
                    }
                }

                // Обновление статуса процесса на "Успешно"
                newProcess.Status = "Успешно";
                // Обноввление состояния процесса в хранилище
                processStatusStorage[newProcess.Id] = newProcess;
                // Кэширование архива путем добавление пары путь: связанный процесс
                memoryCache.Set(newProcess.Id, newProcess, TimeSpan.FromMinutes(10));
                // Логирование информации о завершении формирования архива
                logger.LogInformation($"Процесс {newProcess.Id} завершен успешно");
            }
            catch (Exception ex)
            {
                // Логируем ошибку и обновляем статус процесса на "Провален"
                logger.LogError($"Ошибка при создании архива: {ex.Message}");
                // Обновление статуса процесса на "Провален"
                newProcess.Status = "Провален";
            }
        }

        /// <summary>
        /// Возвращает статус процесса архивации по его идентификатору.
        /// </summary>
        /// <param name="processId">Идентификатор процесса архивации.</param>
        /// <returns>Строка со статусом процесса или null, если процесс не найден.</returns>
        public string? GetStatusByProcessId(int processId)
        {
            try
            {
                // Возвращение статуса процесса
                return GetArchiveCreationProcess(processId)?.Status;
            }
            catch
            {
                // Возвращение null в случае ошибки
                return null;
            }
        }

        /// <summary>
        /// Возвращает поток с архивом по идентификатору процесса.
        /// </summary>
        /// <param name="processId">Идентификатор процесса архивации.</param>
        /// <returns>Поток с архивом или null, если архив не найден или процесс не завершен успешно.</returns>
        public Stream? GetArchiveByProcessId(int processId)
        {
            var process = GetArchiveCreationProcess(processId);

            // Проверка успешного завершения процесса
            if (process != null && process.Status == "Успешно")
            {
                // Возвращение потока для скачивания архива
                return new FileStream(process.ZipFilePath, FileMode.Open, FileAccess.Read);
            }

            // Возвращение null, если архив не найден или процесс не завершен успешно
            return null;
        }

        private ArchiveCreationProcess? GetArchiveCreationProcess(int processId)
        {
            // Получение статуса процесса по его идентификатору из кэша
            if (!memoryCache.TryGetValue(processId, out ArchiveCreationProcess? process))
            {
                // Получение статуса процесса по его идентификатору из хранилища
                processStatusStorage.TryGetValue(processId, out process);
            }

            return process;
        }
    }
}
