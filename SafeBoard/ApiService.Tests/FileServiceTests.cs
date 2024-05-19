using ApiService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiService.Tests
{
    public class FileServiceTests
    {
        private readonly string testFilesDirPath;
        private readonly FileService fileService;

        public FileServiceTests()
        {
            var loggerMock = new Mock<ILogger<FileService>>();
            var configurationMock = new Mock<IConfiguration>();

            // Установка тестового пути для файлов
            testFilesDirPath = Path.Combine(Path.GetTempPath(), "files_test");
            configurationMock.Setup(config => config.GetSection("Paths:Files").Value).Returns(testFilesDirPath);

            // Удаляем тестовую директорию, если она уже существует
            if (Directory.Exists(testFilesDirPath))
            {
                try
                {
                    Directory.Delete(testFilesDirPath, true);
                }
                catch (IOException)
                {
                    // Retry logic or wait for a short period to ensure all handles are released
                    Thread.Sleep(100);
                    Directory.Delete(testFilesDirPath, true);
                }
            }

            fileService = new FileService(configurationMock.Object, loggerMock.Object);
        }

        [Fact]
        public void Constructor_CreatesDirectoryAndFiles()
        {
            // Assert
            Assert.True(Directory.Exists(testFilesDirPath));
            Assert.Equal(5, Directory.GetFiles(testFilesDirPath).Length);
        }

        [Fact]
        public void GetFilesNames_ReturnsAllFileNames()
        {
            // Act
            var fileNames = fileService.GetFilesNames();

            // Assert
            Assert.Equal(5, fileNames.Count());
        }

        [Fact]
        public void GetFilesPaths_ReturnsCorrectPaths()
        {
            // Arrange
            var fileNames = new List<string> { "file1", "file2", "file3" };

            // Act
            var filePaths = fileService.GetFilesPaths(fileNames);

            // Assert
            foreach (var filePath in filePaths)
            {
                Assert.True(File.Exists(filePath));
            }
        }

        [Fact]
        public void FileExistCheck_ReturnsTrueIfFileExists()
        {
            // Act
            var fileExists = fileService.FileExistCheck("file1");

            // Assert
            Assert.True(fileExists);
        }

        [Fact]
        public void FileExistCheck_ReturnsFalseIfFileDoesNotExist()
        {
            // Act
            var fileExists = fileService.FileExistCheck("nonexistentfile");

            // Assert
            Assert.False(fileExists);
        }
    }
}
