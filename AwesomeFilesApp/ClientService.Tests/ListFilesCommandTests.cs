using ClientService.Commands;
using Moq;

namespace ClientService.Tests
{
    public class ListFilesCommandTests
    {
        [Fact]
        public async Task OnExecuteAsync_PrintFiles_WhenApiReturnsFiles()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.GetFilesAsync())
                          .ReturnsAsync(new List<string> { "file1", "file2" });

            var command = new ListFilesCommand(mockApiHandler.Object);

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(0, result);
                Assert.Contains("file1 file2", consoleOutput.GetOutput());
            }
        }

        [Fact]
        public async Task OnExecuteAsync_PrintErrorMessage_WhenApiThrowsException()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.GetFilesAsync())
                          .ThrowsAsync(new Exception("API error"));

            var command = new ListFilesCommand(mockApiHandler.Object);

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(1, result);
                Assert.Contains("Error: API error", consoleOutput.GetOutput());
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
