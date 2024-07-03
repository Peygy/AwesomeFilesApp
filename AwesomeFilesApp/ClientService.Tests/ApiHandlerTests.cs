namespace ClientService.Tests
{
    /// <summary>
    /// Тест API. Выполнять при рабочем API сервисе!!!
    /// </summary>
    public class ApiHandlerTests
    {
        private readonly string ApiAddress;
        private readonly ApiHandler apiHandler;

        public ApiHandlerTests()
        {
            ApiAddress = "http://localhost:8080";
            apiHandler = new ApiHandler(ApiAddress);
        }

        [Fact]
        public async Task GetFilesAsync_Success()
        {
            // Arrange
            var expectedFiles = new List<string> { "file1", "file2" };

            // Act
            var actualFiles = await apiHandler.GetFilesAsync();

            // Assert
            Assert.NotNull(actualFiles);
            Assert.True(actualFiles.Any(x => expectedFiles.Any(y => y == x)));
        }

        [Fact]
        public async Task CreateArchiveAsync_Success()
        {
            // Arrange
            var fileNames = new List<string> { "file1", "file2" };

            // Act
            var processId = await apiHandler.CreateArchiveAsync(fileNames);

            // Assert
            Assert.NotEqual(0, processId);
        }

        [Fact]
        public async Task GetArchiveStatusAsync_Success()
        {
            // Arrange
            var fileNames = new List<string> { "file1", "file2" };
            var processId = await apiHandler.CreateArchiveAsync(fileNames);

            // Act
            var status = await apiHandler.GetArchiveStatusAsync(processId);

            // Assert
            Assert.NotNull(status);
        }

        [Fact]
        public async Task DownloadArchiveAsync_Success()
        {
            // Arrange
            var fileNames = new List<string> { "file1", "file2" };
            var processId = await apiHandler.CreateArchiveAsync(fileNames);
            string outputPath = Path.GetTempPath();

            // Act
            var result = await apiHandler.DownloadArchiveAsync(processId, outputPath);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(Path.Combine(outputPath, "archive0.zip")));
        }
    }
}
