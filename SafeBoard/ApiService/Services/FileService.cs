using System.Text;

namespace ApiService.Services
{
    public class FileService
    {
        private readonly string filesDirPath;
        private readonly ILogger<FileService> logger;

        public FileService(IConfiguration configuration, ILogger<FileService> logger)
        {
            filesDirPath = configuration.GetSection("Paths:Files").Value;
            this.logger = logger;

            if (!Directory.Exists(filesDirPath))
            {
                Directory.CreateDirectory(filesDirPath);
                logger.LogInformation($"Создана директория {filesDirPath}");
                InitAwesomeFiles();
            }
        }

        private void InitAwesomeFiles()
        {
            for (int i = 1; i <= 5; i++)
            {
                using (var fileStream = File.Create(Path.Combine(filesDirPath, $"file{i}")))
                {
                    var fileText = new UTF8Encoding(true).GetBytes($"File{i} text!");
                    fileStream.Write(fileText, 0, fileText.Length);
                }
            }

            logger.LogInformation("Созданы 5 файлов в директории files");
        }

        public IEnumerable<string> GetFilesNames()
        {
            return Directory.GetFiles(filesDirPath);
        }

        public IEnumerable<string> GetFilesPaths(List<string> fileNames)
        {
            for (int i = 0; i < fileNames.Count; i++)
            {
                fileNames[i] = Path.Combine(filesDirPath, fileNames[i]);
            }

            return fileNames;
        }

        public bool FileExistCheck(string fileName)
        {
            return File.Exists(Path.Combine(filesDirPath, fileName));
        }
    }
}
