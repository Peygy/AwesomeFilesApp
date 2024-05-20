using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    [Command(Name = "list", Description = "Команда вывода файлов")]
    internal class ListFilesCommand
    {
        private readonly ApiHandler apiClient;

        public ListFilesCommand(ApiHandler apiClient)
        {
            this.apiClient = apiClient;
        }

        private async Task<int> OnExecuteAsync()
        {
            try
            {
                var files = await apiClient.GetFilesAsync();
                Console.WriteLine(string.Join(" ", files));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return 0;
        }
    }
}
