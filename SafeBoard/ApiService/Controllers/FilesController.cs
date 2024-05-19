using ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Controllers
{
    /// <summary>
    /// Контроллер FilesController для обработки запросов, приходящих на данное api-приложение от пользователя.
    ///
    /// Имеются 4 конечных точек для обработки запросов:
    /// GET api/files - получение всех файлов;
    /// POST api/files/archive - запуск процесса созданиея архива из выбранных файлов (файлы находятся в теле запроса);
    /// GET api/files/archive/{processId} - получение статуса процесса создания архива по номеру его процесса;
    /// GET api/files/archive/{processId}/download - запрос на скачивание архива по номеру его процесса.
    /// </summary>
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly FileService fileService;
        private readonly ArchiveService archiveService;
        private readonly ILogger<FilesController> logger;

        /// <summary>
        /// Конструктор контроллера FilesController.
        /// Инициализирует логгер, сервисы FileService и ArchiveService.
        /// </summary>
        /// <param name="fileService">Сервис для работы с файлами пользователя.</param>
        /// <param name="archiveService">Сервис для работы с процессами создания архивов пользовательских файлов.</param>
        /// <param name="logger">Логгер для фиксации запросов пользователей в консоли.</param>
        public FilesController(FileService fileService, ArchiveService archiveService, ILogger<FilesController> logger)
        {
            this.fileService = fileService;
            this.archiveService = archiveService;
            this.logger = logger;
        }

        /// <summary>
        /// Возвращает результат обработки конечной точки GET api/files.
        /// </summary>
        /// <returns>Коллекцию всех пользовательских файлов.</returns>
        [HttpGet(Name = "GetAllFiles")]
        public IEnumerable<string> GetFiles()
        {
            // Логирование информации о запросе пользователя
            logger.LogInformation("Запрос от пользователя на получение файлов");

            // Вызов метода для получения имен всех файлов
            return fileService.GetFilesNames();
        }

        /// <summary>
        /// Возвращает результат обработки конечной точки POST api/files/archive.
        /// </summary>
        /// <param name="fileNamesDto">Список имен файлов, необходимых для формирования zip-архива.</param>
        /// <returns>Статусный код: 200 Ok - архив создан без ошибок, 400 - неверные/отсутствующие данные в запросе.</returns>
        [HttpPost("archive", Name = "InitilizationOfFilesArchive")]
        public IActionResult InitFilesArchive([FromBody] FileNamesDto fileNamesDto)
        {
            // Логирование информации о запросе пользователя
            logger.LogInformation("Запрос от пользователя на создание архива выбранных файлов");

            // Получение коллекции имен файлов из объекта передачи данных
            var fileNames = fileNamesDto.FileNames;
            // Проверка на наличие файлов в запросе пользователя
            if (fileNames.Count == 0)
            {
                // Если пользователь не задал наименования файлов, то выдается ошибка запроса
                return BadRequest("Должен присутствовать хотя бы один файл!");
            }

            // Выборка наименования каждого файла входящей коллекции
            foreach (var file in fileNames)
            {
                // Проверка каждого файла на существование
                if (!fileService.FileExistCheck(file))
                {
                    // Если файл не существует, то выдается ошибка запроса
                    return BadRequest($"Файла {file} не существует!");
                }
            }

            // Получение путей заданных пользователем файлов
            var filesPaths = fileService.GetFilesPaths(fileNames);
            // Получение идентификатора процесса создания архива
            var processId = archiveService.CreateNewProcess(filesPaths);

            // Отправка пользователю идентификатора процесса создания архива
            return Ok(processId);
        }

        /// <summary>
        /// Возвращает результат обработки конечной точки GET api/files/archive/{processId}.
        /// </summary>
        /// <param name="processId">Уникальный идентификатор нужного процесса создания архива.</param>
        /// <returns>
        /// Статусный код:
        ///     200 Ok - получен статус по заданному идентификатору процесса создания архива;
        ///     400 BadRequest - неверный идентификатор процесса создания архива.
        /// </returns>
        [HttpGet("archive/{processId}", Name = "CheckProcessStatus")]
        public IActionResult CheckProcessStatus(int processId)
        {
            // Логирование информации о запросе пользователя
            logger.LogInformation($"Запрос от пользователя на получение статуса процесса по номеру {processId}");

            // Получение статуса процесса создания архива
            var status = archiveService.GetStatusByProcessId(processId);

            // Проверка статуса процесса создания архива на существование
            if (status == null)
            {
                // Если статус процесса создания архива не найден, то выдается ошибка запроса
                return BadRequest($"Процесса с номером {processId} не существует!");
            }

            // Отправка статуса процесса создания архива
            return Ok(status);
        }

        /// <summary>
        /// Возвращает результат обработки конечной точки GET api/files/archive/{processId}/download.
        /// </summary>
        /// <param name="processId">Уникальный идентификатор нужного процесса создания архива.</param>
        /// <returns>
        /// Статусный код:
        ///     FileStreamResult - архив по заданному идентификатору процесса найден;
        ///     400 BadRequest - неверный идентификатор процесса создания архива.
        /// </returns>
        [HttpGet("archive/{processId}/download", Name = "DownloadFilesArchive")]
        public IActionResult DownloadArchive(int processId)
        {
            // Логирование информации о запросе пользователя
            logger.LogInformation($"Запрос от пользователя на скачивание архива по номеру {processId}");

            // Получение файлового потока для скачивания архива, найденного по идентификатору процесса создания архива
            var stream = archiveService.GetArchiveByProcessId(processId);

            // Проверка потока на его существование
            if (stream == null)
            {
                // Если поток не создан, то нужного процесса не существует
                return BadRequest($"Архива, процесс которого имеет номер {processId}, не существует!");
            }

            // Отправка потока с вписанным в него необходимым zip-архивом
            return File(stream, "application/zip", $"{processId}.zip");
        }

        /// <summary>
        /// Класс объекта передачи данных (DTO), необходимый для получения списка наименований файлов от пользователя.
        /// </summary>
        public class FileNamesDto
        {
            // Коллекция наименовваний файлов, указанных пользователем
            public List<string> FileNames { get; set; } = null!;
        }
    }
}
