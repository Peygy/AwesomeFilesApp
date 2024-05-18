using ApiService.Models;
using System.Collections.Concurrent;
using System.IO.Compression;

namespace ApiService.Services
{
    public class ArchiveService
    {
        private readonly string archivesDirPath;
        private readonly ConcurrentDictionary<int, ArchiveCreationProcess> processStatusStorage;
        private readonly ILogger<ArchiveService> logger;

        public ArchiveService(IConfiguration configuration, ILogger<ArchiveService> logger)
        {
            archivesDirPath = configuration.GetSection("Paths:Archives").Value;
            this.logger = logger;

            if (!Directory.Exists(archivesDirPath))
            {
                Directory.CreateDirectory(archivesDirPath);
                logger.LogInformation($"Создана директория {archivesDirPath}");
            }
            else
            {
                var directoryInfo = new DirectoryInfo(archivesDirPath);

                foreach (FileInfo zipArchive in directoryInfo.GetFiles())
                {
                    zipArchive.Delete();
                }

                logger.LogInformation($"Директория {archivesDirPath} пересоздана");
            }

            processStatusStorage = new ConcurrentDictionary<int, ArchiveCreationProcess>();
        }

        public int CreateNewProcess(IEnumerable<string> filesPaths)
        {
            var newProcess = new ArchiveCreationProcess
            {
                Id = new Random().Next(100000, 1000000),
                Status = "Выполняется",
                FilePaths = filesPaths
            };
            processStatusStorage.TryAdd(newProcess.Id, newProcess);
            logger.LogInformation($"Процесс {newProcess.Id} выполняется");

            // Параллельно запускаем задачу
            Task.Run(() => CreateArchiveAsync(newProcess));

            return newProcess.Id;
        }

        private async Task CreateArchiveAsync(ArchiveCreationProcess newProcess)
        {
            try
            {
                var newArchivePath = Path.Combine(archivesDirPath, $"{newProcess.Id}.zip");

                using (var zipStream = new FileStream(newArchivePath, FileMode.Create))
                {
                    using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                    {
                        foreach (var filePath in newProcess.FilePaths)
                        {
                            zipArchive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                        }
                    }
                }

                newProcess.Status = "Успешно";
                processStatusStorage[newProcess.Id] = newProcess;
                logger.LogInformation($"Процесс {newProcess.Id} заввершен успешно");
            }
            catch (Exception ex)
            {
                logger.LogError($"Ошибка при созднии архива: {ex.Message}");
                newProcess.Status = "Провален";
            }
        }

        public string? GetStatusByProcessId(int processId)
        {
            try
            {
                processStatusStorage.TryGetValue(processId, out var process);
                return process.Status;
            }
            catch
            {
                return null;
            }
        }

        public Stream? GetArchiveByProcessId(int processId)
        {
            processStatusStorage.TryGetValue(processId, out var process);

            if (process != null &&  process.Status == "Успешно")
            {
                var archivePath = Path.Combine(archivesDirPath, $"{process.Id}.zip");
                return new FileStream(archivePath, FileMode.Open, FileAccess.Read);
            }

            return null;
        }
    }
}
