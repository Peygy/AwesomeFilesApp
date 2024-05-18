using ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Controllers
{
    /// <summary>
    /// ���������� ��� ��������� ��������, ���������� �� ������ api-������
    /// �� ������������.
    ///
    /// ������� 4 ��������� ��� ��������� ��������:
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
        /// ����������� ����������
        /// </summary>
        /// <param name="fileService">������ ��� ������ � ������������� �������</param>
        /// <param name="archiveService">������ ��� ������ � �������� ������������� ������</param>
        /// <param name="logger">������ ��� �������� �������� ������������� � �������</param>
        public FilesController(FileService fileService, ArchiveService archiveService, ILogger<FilesController> logger)
        {
            this.fileService = fileService;
            this.archiveService = archiveService;
            this.logger = logger;
        }

        [HttpGet(Name = "GetAllFiles")]
        public IEnumerable<string> GetFiles()
        {
            logger.LogInformation("������ �� ������������ �� ��������� ������");
            return fileService.GetFilesNames();
        }

        [HttpPost("archive", Name = "InitilizationOfFilesArchive")]
        public IActionResult InitFilesArchive([FromBody] FileNamesDto fileNamesDto)
        {
            logger.LogInformation("������ �� ������������ �� �������� ������ ��������� ������");

            var fileNames = fileNamesDto.FileNames;

            if (fileNames.Count == 0)
            {
                return BadRequest("������ �������������� ���� �� ���� ����!");
            }

            foreach (var file in fileNames)
            {
                if (!fileService.FileExistCheck(file))
                {
                    return BadRequest($"����� {file} �� ����������!");
                }
            }

            var filesPaths = fileService.GetFilesPaths(fileNames);
            var processId = archiveService.CreateNewProcess(filesPaths);

            return Ok(processId);
        }

        [HttpGet("archive/{processId}", Name = "CheckProcessStatus")]
        public IActionResult CheckProcessStatus(int processId)
        {
            logger.LogInformation($"������ �� ������������ �� ��������� ������� �������� �� ������ {processId}");
            var status = archiveService.GetStatusByProcessId(processId);

            if (status == null)
            {
                return BadRequest($"�������� � ������� {processId} �� ����������!");
            }

            return Ok(status);
        }

        [HttpGet("archive/{processId}/download", Name = "DownloadFilesArchive")]
        public IActionResult DownloadArchive(int processId)
        {
            logger.LogInformation($"������ �� ������������ �� ���������� ������ �� ������ {processId}");
            var stream = archiveService.GetArchiveByProcessId(processId);

            if (stream == null)
            {
                return BadRequest($"������, ������� �������� ����� ����� {processId}, �� ����������!");
            }

            return File(stream, "application/zip", $"{processId}.zip");
        }

        public class FileNamesDto
        {
            public List<string> FileNames { get; set; }
        }
    }
}
