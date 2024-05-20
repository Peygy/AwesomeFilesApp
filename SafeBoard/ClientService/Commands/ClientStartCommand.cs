using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    [Command(Name = "client", Description = "Команда запуска клиента для взаимодействия с Api")]
    [Subcommand(typeof(ListFilesCommand))]
    [Subcommand(typeof(CreateArchiveCommand))]
    [Subcommand(typeof(GetProcessStatusCommand))]
    [Subcommand(typeof(DownloadArchiveCommand))]
    [Subcommand(typeof(GetJustArchiveCommand))]
    internal class ClientStartCommand
    {
        private async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("Client was started.\nPress <Enter> to exit...");
            while (true)
            {
                console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    break;
                }

                var inputArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var commandName = inputArgs[0];
                var commandArgs = inputArgs.Skip(1).ToArray();

                try
                {
                    var command = app.Commands.SingleOrDefault(c => c.Name == commandName);
                    if (command == null)
                    {
                        console.WriteLine($"Unknown command: {commandName}");
                        continue;
                    }

                    await command.ExecuteAsync(commandArgs);
                }
                catch (Exception ex)
                {
                    console.WriteLine($"Error: {ex.Message}");
                }
            }
            return 0;
        }
    }
}
