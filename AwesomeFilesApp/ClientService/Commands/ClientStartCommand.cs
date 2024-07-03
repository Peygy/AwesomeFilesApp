using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    /// <summary>
    /// Класс команды для запуска клиента для взаимодействия с API.
    /// </summary>
    [Command(Name = "client", Description = "Команда запуска клиента для взаимодействия с Api")]
    [Subcommand(typeof(ListFilesCommand))]
    [Subcommand(typeof(CreateArchiveCommand))]
    [Subcommand(typeof(GetProcessStatusCommand))]
    [Subcommand(typeof(DownloadArchiveCommand))]
    [Subcommand(typeof(GetCompleteArchiveCommand))]
    public class ClientStartCommand
    {
        /// <summary>
        /// Асинхронный метод, выполняемый при запуске команды клиента.
        /// </summary>
        /// <param name="app">CommandLineApplication, представляющий текущее приложение.</param>
        /// <param name="console">IConsole для взаимодействия с консолью.</param>
        /// <returns>Задача, представляющая результат выполнения команды.</returns>
        private async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            // Сообщение о запуске клиента и инструкции для выхода
            console.WriteLine("Client was started.\nPress <Enter> to exit...");

            while (true)
            {
                console.Write("> ");
                var input = Console.ReadLine();

                // Если введенная строка пуста, выйти из цикла
                if (string.IsNullOrEmpty(input))
                {
                    break;
                }

                // Разделение введенной строки на команду и аргументы
                var inputArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var commandName = inputArgs[0];
                var commandArgs = inputArgs.Skip(1).ToArray();

                try
                {
                    // Поиск команды по имени
                    var command = app.Commands.SingleOrDefault(c => c.Name == commandName);
                    // Если команда не найдена, вывести сообщение об ошибке
                    if (command == null)
                    {
                        console.WriteLine($"Unknown command: {commandName}");
                        continue;
                    }

                    // Выполнение найденной команды с переданными аргументами
                    await command.ExecuteAsync(commandArgs);
                }
                catch (Exception ex)
                {
                    // Обработка исключений и вывод сообщения об ошибке
                    console.WriteLine($"Error: {ex.Message}");
                }
            }

            // Возвращение кода завершения 0, что означает успешное выполнение
            return 0;
        }
    }
}
