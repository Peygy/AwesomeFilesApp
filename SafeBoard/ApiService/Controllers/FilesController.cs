using ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Controllers
{
    /// <summary>
    /// Контроллер для обработки запросов, приходящих на данный api-сервис
    /// от пользователя.
    ///
    /// Имеются 4 эндпоинта для обработки запросов:
    /// 1.
    /// 2.
    /// 3.
    /// 4.
    /// </summary>
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly FileService fileService;
        private readonly ArchiveService archiveService;
        private readonly ILogger<FilesController> logger;

        /// <summary>
        /// Конструктор котроллера
        /// </summary>
        /// <param name="fileService">Сервис для работы с “потрясающими” файлами</param>
        /// <param name="archiveService">Сервис для работы с архивами “потрясающих” файлов</param>
        /// <param name="logger">Логгер для фиксации запросов пользователей в консоли</param>
        public FilesController(FileService fileService, ArchiveService archiveService, ILogger<FilesController> logger)
        {
            this.fileService = fileService;
            this.archiveService = archiveService;
            this.logger = logger;
        }

        [HttpGet(Name = "GetAllFiles")]
        public IEnumerable<string> GetFiles()
        {
            logger.LogInformation("Запрос от пользователя на получение файлов");
            return fileService.GetFilesNames();
        }

        [HttpPost("archive", Name = "InitilizationOfFilesArchive")]
        public IActionResult InitFilesArchive([FromBody] FileNamesDto fileNamesDto)
        {
            logger.LogInformation("Запрос от пользователя на создание архива выбранных файлов");

            var fileNames = fileNamesDto.FileNames;

            if (fileNames.Count == 0)
            {
                return BadRequest("Должен присутствовать хотя бы один файл!");
            }

            foreach (var file in fileNames)
            {
                if (!fileService.FileExistCheck(file))
                {
                    return BadRequest($"Файла {file} не существует!");
                }
            }

            var filesPaths = fileService.GetFilesPaths(fileNames);
            var processId = archiveService.CreateNewProcess(filesPaths);

            return Ok(processId);
        }

        [HttpGet("archive/{processId}", Name = "CheckProcessStatus")]
        public IActionResult CheckProcessStatus(int processId)
        {
            logger.LogInformation($"Запрос от пользователя на получение статуса процесса по номеру {processId}");
            var status = archiveService.GetStatusByProcessId(processId);

            if (status == null)
            {
                return BadRequest($"Процесса с номером {processId} не существует!");
            }

            return Ok(status);
        }

        [HttpGet("archive/{processId}/download", Name = "DownloadFilesArchive")]
        public IActionResult DownloadArchive(int processId)
        {
            logger.LogInformation($"Запрос от пользователя на скачивание архива по номеру {processId}");
            var stream = archiveService.GetArchiveByProcessId(processId);

            if (stream == null)
            {
                return BadRequest($"Архива, процесс которого имеет номер {processId}, не существует!");
            }

            return File(stream, "application/zip", $"{processId}.zip");
        }

        public class FileNamesDto
        {
            public List<string> FileNames { get; set; }
        }
    }
}
