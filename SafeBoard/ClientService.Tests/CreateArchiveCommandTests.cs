using ClientService.Commands;
using Moq;

namespace ClientService.Tests
{
    public class CreateArchiveCommandTests
    {
        [Fact]
        public async Task OnExecuteAsync_CreateArchiveSuccessfully()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.CreateArchiveAsync(It.IsAny<List<string>>()))
                          .ReturnsAsync(123); // Процесс успешно создан

            var command = new CreateArchiveCommand(mockApiHandler.Object)
            {
                FileNames = new List<string> { "file1", "file2" }
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(0, result);
                Assert.Contains("123", consoleOutput.GetOutput());
            }
        }

        [Fact]
        public async Task OnExecuteAsync_HandleError_WhenArchiveCreationFails()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.CreateArchiveAsync(It.IsAny<List<string>>()))
                          .ReturnsAsync(0); // Процесс не создан

            var command = new CreateArchiveCommand(mockApiHandler.Object)
            {
                FileNames = new List<string> { "file1", "file2" }
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(1, result);
                Assert.Contains("Error: Failed to create archive", consoleOutput.GetOutput());
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
