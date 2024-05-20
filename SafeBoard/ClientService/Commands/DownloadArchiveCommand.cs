using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    [Command(Name = "download", Description = "Команда для скачивания архива")]
    internal class DownloadArchiveCommand
    {
        private readonly ApiHandler apiClient;

        [Argument(0, "processId", "Идентификатор процесса архивации")]
        public int ProcessId { get; set; }

        [Argument(1, "outputPath", "Путь для сохранения архива")]
        public string OutputPath { get; set; } = null!;

        public DownloadArchiveCommand(ApiHandler apiClient)
        {
            this.apiClient = apiClient;
        }

        private async Task<int> OnExecuteAsync()
        {
            if (ProcessId == 0 || string.IsNullOrEmpty(OutputPath))
            {
                Console.WriteLine("Error: Null id of process or path");
                return 0;
            }

            try
            {
                if (await apiClient.DownloadArchiveAsync(ProcessId, OutputPath))
                {
                    Console.WriteLine($"Archive downloaded to {OutputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return 0;
        }
    }
}
