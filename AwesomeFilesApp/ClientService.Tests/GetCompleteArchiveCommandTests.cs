using ClientService.Commands;
using Moq;

namespace ClientService.Tests
{
    public class GetCompleteArchiveCommandTests
    {
        [Fact]
        public async Task OnExecuteAsync_CreateArchive_AndDownloadSuccessfully()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.CreateArchiveAsync(It.IsAny<List<string>>()))
                          .ReturnsAsync(123); // Процесс успешно создан
            mockApiHandler.Setup(api => api.GetArchiveStatusAsync(123))
                          .ReturnsAsync("Успешно"); // Процесс завершен успешно
            mockApiHandler.Setup(api => api.DownloadArchiveAsync(123, It.IsAny<string>()))
                          .ReturnsAsync(true); // Архив успешно скачан

            var command = new GetCompleteArchiveCommand(mockApiHandler.Object)
            {
                OutputPath = "path/archive0.zip",
                FileNames = new List<string> { "file1", "file2" }
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(0, result);
                Assert.Contains("Create archive task is started, id: 123", consoleOutput.GetOutput());
                Assert.Contains("Process status: Успешно", consoleOutput.GetOutput());
                Assert.Contains("Archive downloaded to path/archive0.zip", consoleOutput.GetOutput());
            }
        }

        [Fact]
        public async Task OnExecuteAsync_HandleError_WhenArchiveCreationFails()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.CreateArchiveAsync(It.IsAny<List<string>>()))
                          .ReturnsAsync(0); // Процесс не создан

            var command = new GetCompleteArchiveCommand(mockApiHandler.Object)
            {
                OutputPath = "path/to/archive.zip",
                FileNames = new List<string> { "file1", "file2" }
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(1, result);
                Assert.Contains("Ошибка создания архива", consoleOutput.GetOutput());
            }
        }

        /// <summary>
        /// Класс виртуальной консоли
        /// </summary>
        private class ConsoleOutput : IDisposable
        {
            private readonly StringWriter stringWriter;
            private readonly TextWriter originalOutput;

            public ConsoleOutput()
            {
                stringWriter = new StringWriter();
                originalOutput = Console.Out;
                Console.SetOut(stringWriter);
            }

            public string GetOutput()
            {
                return stringWriter.ToString();
            }

            public void Dispose()
            {
                Console.SetOut(originalOutput);
                stringWriter.Dispose();
            }
        }
    }
}
