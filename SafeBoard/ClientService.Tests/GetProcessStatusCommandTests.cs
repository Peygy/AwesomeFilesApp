using Moq;
using ClientService.Commands;

namespace ClientService.Tests
{
    public class GetProcessStatusCommandTests
    {
        [Theory]
        [InlineData("Выполняется", "Process in progress, please wait...")]
        [InlineData("Успешно", "Archive has been created.")]
        [InlineData("Провален", "Archive creation is failed.")]
        [InlineData("", "Unknown status.")]
        public async Task OnExecuteAsync_PrintCorrectMessage_ForGivenStatus(string apiStatus, string expectedMessage)
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.GetArchiveStatusAsync(It.IsAny<int>()))
                          .ReturnsAsync(apiStatus);

            var command = new GetProcessStatusCommand(mockApiHandler.Object)
            {
                ProcessId = 1
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Act
                var result = await command.OnExecuteAsync();

                // Assert
                Assert.Equal(0, result);
                Assert.Contains(expectedMessage, consoleOutput.GetOutput());
            }
        }

        [Fact]
        public async Task OnExecuteAsync_PrintErrorMessage_WhenApiThrowsException()
        {
            // Arrange
            var mockApiHandler = new Mock<IApiHandler>();
            mockApiHandler.Setup(api => api.GetArchiveStatusAsync(It.IsAny<int>()))
                          .ThrowsAsync(new Exception("API error"));

            var command = new GetProcessStatusCommand(mockApiHandler.Object)
            {
                ProcessId = 1
            };

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