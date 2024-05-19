using ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Controllers
{
    /// <summary>
    /// ���������� FilesController ��� ��������� ��������, ���������� �� ������ api-���������� �� ������������.
    ///
    /// ������� 4 �������� ����� ��� ��������� ��������:
    /// GET api/files - ��������� ���� ������;
    /// POST api/files/archive - ������ �������� ��������� ������ �� ��������� ������ (����� ��������� � ���� �������);
    /// GET api/files/archive/{processId} - ��������� ������� �������� �������� ������ �� ������ ��� ��������;
    /// GET api/files/archive/{processId}/download - ������ �� ���������� ������ �� ������ ��� ��������.
    /// </summary>
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly FileService fileService;
        private readonly ArchiveService archiveService;
        private readonly ILogger<FilesController> logger;

        /// <summary>
        /// ����������� ����������� FilesController.
        /// �������������� ������, ������� FileService � ArchiveService.
        /// </summary>
        /// <param name="fileService">������ ��� ������ � ������� ������������.</param>
        /// <param name="archiveService">������ ��� ������ � ���������� �������� ������� ���������������� ������.</param>
        /// <param name="logger">������ ��� �������� �������� ������������� � �������.</param>
        public FilesController(FileService fileService, ArchiveService archiveService, ILogger<FilesController> logger)
        {
            this.fileService = fileService;
            this.archiveService = archiveService;
            this.logger = logger;
        }

        /// <summary>
        /// ���������� ��������� ��������� �������� ����� GET api/files.
        /// </summary>
        /// <returns>��������� ���� ���������������� ������.</returns>
        [HttpGet(Name = "GetAllFiles")]
        public IEnumerable<string> GetFiles()
        {
            // ����������� ���������� � ������� ������������
            logger.LogInformation("������ �� ������������ �� ��������� ������");

            // ����� ������ ��� ��������� ���� ���� ������
            return fileService.GetFilesNames();
        }

        /// <summary>
        /// ���������� ��������� ��������� �������� ����� POST api/files/archive.
        /// </summary>
        /// <param name="fileNamesDto">������ ���� ������, ����������� ��� ������������ zip-������.</param>
        /// <returns>��������� ���: 200 Ok - ����� ������ ��� ������, 400 - ��������/������������� ������ � �������.</returns>
        [HttpPost("archive", Name = "InitilizationOfFilesArchive")]
        public IActionResult InitFilesArchive([FromBody] FileNamesDto fileNamesDto)
        {
            // ����������� ���������� � ������� ������������
            logger.LogInformation("������ �� ������������ �� �������� ������ ��������� ������");

            // ��������� ��������� ���� ������ �� ������� �������� ������
            var fileNames = fileNamesDto.FileNames;
            // �������� �� ������� ������ � ������� ������������
            if (fileNames.Count == 0)
            {
                // ���� ������������ �� ����� ������������ ������, �� �������� ������ �������
                return BadRequest("������ �������������� ���� �� ���� ����!");
            }

            // ������� ������������ ������� ����� �������� ���������
            foreach (var file in fileNames)
            {
                // �������� ������� ����� �� �������������
                if (!fileService.FileExistCheck(file))
                {
                    // ���� ���� �� ����������, �� �������� ������ �������
                    return BadRequest($"����� {file} �� ����������!");
                }
            }

            // ��������� ����� �������� ������������� ������
            var filesPaths = fileService.GetFilesPaths(fileNames);
            // ��������� �������������� �������� �������� ������
            var processId = archiveService.CreateNewProcess(filesPaths);

            // �������� ������������ �������������� �������� �������� ������
            return Ok(processId);
        }

        /// <summary>
        /// ���������� ��������� ��������� �������� ����� GET api/files/archive/{processId}.
        /// </summary>
        /// <param name="processId">���������� ������������� ������� �������� �������� ������.</param>
        /// <returns>
        /// ��������� ���:
        ///     200 Ok - ������� ������ �� ��������� �������������� �������� �������� ������;
        ///     400 BadRequest - �������� ������������� �������� �������� ������.
        /// </returns>
        [HttpGet("archive/{processId}", Name = "CheckProcessStatus")]
        public IActionResult CheckProcessStatus(int processId)
        {
            // ����������� ���������� � ������� ������������
            logger.LogInformation($"������ �� ������������ �� ��������� ������� �������� �� ������ {processId}");

            // ��������� ������� �������� �������� ������
            var status = archiveService.GetStatusByProcessId(processId);

            // �������� ������� �������� �������� ������ �� �������������
            if (status == null)
            {
                // ���� ������ �������� �������� ������ �� ������, �� �������� ������ �������
                return BadRequest($"�������� � ������� {processId} �� ����������!");
            }

            // �������� ������� �������� �������� ������
            return Ok(status);
        }

        /// <summary>
        /// ���������� ��������� ��������� �������� ����� GET api/files/archive/{processId}/download.
        /// </summary>
        /// <param name="processId">���������� ������������� ������� �������� �������� ������.</param>
        /// <returns>
        /// ��������� ���:
        ///     FileStreamResult - ����� �� ��������� �������������� �������� ������;
        ///     400 BadRequest - �������� ������������� �������� �������� ������.
        /// </returns>
        [HttpGet("archive/{processId}/download", Name = "DownloadFilesArchive")]
        public IActionResult DownloadArchive(int processId)
        {
            // ����������� ���������� � ������� ������������
            logger.LogInformation($"������ �� ������������ �� ���������� ������ �� ������ {processId}");

            // ��������� ��������� ������ ��� ���������� ������, ���������� �� �������������� �������� �������� ������
            var stream = archiveService.GetArchiveByProcessId(processId);

            // �������� ������ �� ��� �������������
            if (stream == null)
            {
                // ���� ����� �� ������, �� ������� �������� �� ����������
                return BadRequest($"������, ������� �������� ����� ����� {processId}, �� ����������!");
            }

            // �������� ������ � ��������� � ���� ����������� zip-�������
            return File(stream, "application/zip", $"{processId}.zip");
        }

        /// <summary>
        /// ����� ������� �������� ������ (DTO), ����������� ��� ��������� ������ ������������ ������ �� ������������.
        /// </summary>
        public class FileNamesDto
        {
            // ��������� ������������� ������, ��������� �������������
            public List<string> FileNames { get; set; } = null!;
        }
    }
}
