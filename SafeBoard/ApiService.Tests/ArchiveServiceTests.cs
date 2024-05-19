using ApiService.Models;
using ApiService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiService.Tests
{
    public class ArchiveServiceTests
    {
        private readonly string testArchiveDirPath;
        private readonly ArchiveService archiveService;

        public ArchiveServiceTests()
        {
            var loggerMock = new Mock<ILogger<ArchiveService>>();
            var configurationMock = new Mock<IConfiguration>();

            testArchiveDirPath = Path.Combine(Path.GetTempPath(), "archives_test");
            configurationMock.Setup(config => config.GetSection("Paths:Archives").Value).Returns(testArchiveDirPath);

            if (Directory.Exists(testArchiveDirPath))
            {
                Directory.Delete(testArchiveDirPath, true);
            }

            archiveService = new ArchiveService(configurationMock.Object, loggerMock.Object);
        }

        [Fact]
        public void Constructor_CreatesDirectoryIfNotExists()
        {
            // Assert
            Assert.True(Directory.Exists(testArchiveDirPath));
        }

        [Fact]
        public void Constructor_ClearsDirectoryIfExists()
        {
            // Arrange
            File.Create(Path.Combine(testArchiveDirPath, "test.zip")).Dispose();

            // Act
            var loggerMock = new Mock<ILogger<ArchiveService>>();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config.GetSection("Paths:Archives").Value).Returns(testArchiveDirPath);
            _ = new ArchiveService(configurationMock.Object, loggerMock.Object);

            // Assert
            Assert.True(Directory.Exists(testArchiveDirPath));
            Assert.Empty(Directory.GetFiles(testArchiveDirPath));
        }

        [Fact]
        public void CreateNewProcess_AddsProcessToStorage()
        {
            // Arrange
            var filePath1 = Path.Combine(Path.GetTempPath(), "files_test", "file1");
            var filePath2 = Path.Combine(Path.GetTempPath(), "files_test", "file2");
            var filesPaths = new List<string> { filePath1, filePath2 };

            // Act
            var processId = archiveService.CreateNewProcess(filesPaths);

            // Assert
            Assert.True(archiveService.GetStatusByProcessId(processId) == "Выполняется");
        }

        [Fact]
        public void GetStatusByProcessId_ReturnsCorrectStatus()
        {
            // Arrange
            var filePath1 = Path.Combine(Path.GetTempPath(), "files_test", "file1");
            var filePath2 = Path.Combine(Path.GetTempPath(), "files_test", "file2");
            var filesPaths = new List<string> { filePath1, filePath2 };

            var processId = archiveService.CreateNewProcess(filesPaths);

            // Act
            var status = archiveService.GetStatusByProcessId(processId);

            // Assert
            Assert.Equal("Выполняется", status);
        }

        [Fact]
        public async Task CreateNewProcess_CreatesArchiveFile()
        {
            // Arrange
            var filePath1 = Path.Combine(Path.GetTempPath(), "files_test", "file1");
            var filePath2 = Path.Combine(Path.GetTempPath(), "files_test", "file2");
            var filesPaths = new List<string> { filePath1, filePath2 };

            var processId = archiveService.CreateNewProcess(filesPaths);

            // Act
            await Task.Delay(1000);
            var archivePath = Path.Combine(testArchiveDirPath, $"{processId}.zip");

            // Assert
            Assert.True(File.Exists(archivePath));
        }

        [Fact]
        public async Task GetArchiveByProcessId_ReturnsFileStreamWhenProcessIsSuccessful()
        {
            // Arrange
            var filePath1 = Path.Combine(Path.GetTempPath(), "files_test", "file1");
            var filePath2 = Path.Combine(Path.GetTempPath(), "files_test", "file2");
            var filesPaths = new List<string> { filePath1, filePath2 };

            var processId = archiveService.CreateNewProcess(filesPaths);
            await Task.Delay(1000);

            // Act
            var stream = archiveService.GetArchiveByProcessId(processId);

            // Assert
            Assert.NotNull(stream);
            stream.Dispose();
        }

        [Fact]
        public void GetArchiveByProcessId_ReturnsNullWhenProcessIsNotSuccessful()
        {
            // Arrange
            var process = new ArchiveCreationProcess
            {
                Id = 123456,
                Status = "Провален",
                FilePaths = new List<string> { "file1" }
            };

            archiveService.GetType()
                   .GetField("processStatusStorage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                   ?.GetValue(archiveService)
                   .GetType()
                   .GetMethod("TryAdd")
                   ?.Invoke(archiveService.GetType()
                                   .GetField("processStatusStorage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                   ?.GetValue(archiveService), [process.Id, process]);

            // Act
            var stream = archiveService.GetArchiveByProcessId(process.Id);

            // Assert
            Assert.Null(stream);
        }
    }
}
