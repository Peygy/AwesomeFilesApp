using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    [Command(Name = "create-archive", Description = "Команда для создания архива")]
    internal class CreateArchiveCommand
    {
        private readonly ApiHandler apiClient;

        [Argument(0, "fileNames", "Список имен файлов для создания архива")]
        public List<string> FileNames { get; set; } = new List<string>();

        public CreateArchiveCommand(ApiHandler apiClient)
        {
            this.apiClient = apiClient;
        }

        private async Task<int> OnExecuteAsync()
        {
            try
            {
                var processId = await apiClient.CreateArchiveAsync(FileNames);
                if (processId != 0)
                {
                    Console.WriteLine(processId);
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
