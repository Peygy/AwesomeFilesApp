using ClientService.Commands;
using Moq;

namespace ClientService.Tests
{
    public class DownloadArchiveCommandTests
    {
        [Fact]
        public async Task OnExecuteAsync_DownloadArchiveSuccessfully()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.DownloadArchiveAsync(123, "path/archive.zip"))
                          .ReturnsAsync(true); // Архив успешно скачан

            var command = new DownloadArchiveCommand(mockApiHandler.Object)
            {
                ProcessId = 123,
                OutputPath = "path/archive.zip"
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(0, result);
                Assert.Contains("Archive downloaded to path/archive.zip", consoleOutput.GetOutput());
            }
        }

        [Fact]
        public async Task OnExecuteAsync_HandleError_WhenDownloadFails()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.DownloadArchiveAsync(123, "path/archive.zip"))
                          .ReturnsAsync(false); // Скачивание архива не удалось

            var command = new DownloadArchiveCommand(mockApiHandler.Object)
            {
                ProcessId = 123,
                OutputPath = "path/archive.zip"
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(1, result);
                Assert.Contains("Error: Failed downloaded to", consoleOutput.GetOutput());
            }
        }

        [Fact]
        public async Task OnExecuteAsync_HandleNullIdAndPathError()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();

            var command = new DownloadArchiveCommand(mockApiHandler.Object)
            {
                // Некорректный id процесса
                ProcessId = 0,
                // Некорректный путь для сохранения архива
                OutputPath = null
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(1, result);
                Assert.Contains("Error: Null id of process or path", consoleOutput.GetOutput());
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
