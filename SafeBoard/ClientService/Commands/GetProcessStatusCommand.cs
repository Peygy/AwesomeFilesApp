using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    [Command(Name = "status", Description = "Команда для получения статуса процесса создания архива")]
    internal class GetProcessStatusCommand
    {
        private readonly ApiHandler apiClient;

        [Argument(0, "processId", "Идентификатор процесса архивации")]
        public int ProcessId { get; set; }

        public GetProcessStatusCommand(ApiHandler apiClient)
        {
            this.apiClient = apiClient;
        }

        private async Task<int> OnExecuteAsync()
        {
            try
            {
                var status = await apiClient.GetArchiveStatusAsync(ProcessId);

                string message = status switch
                {
                    "Выполняется" => "Process in progress, please wait...",
                    "Успешно" => "Archive has been created.",
                    "Провален" => "Archive creation is failed.",
                    _ => string.Empty
                };

                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return 0;
        }
    }
}
