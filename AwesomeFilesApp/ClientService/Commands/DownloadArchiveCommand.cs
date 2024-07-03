using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    /// <summary>
    /// Класс команды для скачивания архива.
    /// </summary>
    [Command(Name = "download", Description = "Команда для скачивания архива")]
    public class DownloadArchiveCommand
    {
        private readonly IApiHandler apiHandler;

        /// <summary>
        /// Идентификатор процесса архивации, передаваемый в командной строке.
        /// </summary>
        [Argument(0, "processId", "Идентификатор процесса архивации")]
        public int ProcessId { get; set; }

        /// <summary>
        /// Путь для сохранения архива, передаваемый в командной строке.
        /// </summary>
        [Argument(1, "outputPath", "Путь для сохранения архива")]
        public string OutputPath { get; set; } = null!;

        /// <summary>
        /// Конструктор класса команды, принимает обработчик ApiHandler для взаимодействия с API.
        /// </summary>
        /// <param name="apiHandler">Обработчик ApiHandler для выполнения запросов к API.</param>
        public DownloadArchiveCommand(IApiHandler apiHandler)
        {
            this.apiHandler = apiHandler;
        }

        /// <summary>
        /// Асинхронный метод, выполняемый при запуске команды.
        /// </summary>
        /// <returns>Задача, представляющая результат выполнения команды.</returns>
        public async Task<int> OnExecuteAsync()
        {
            // Проверка на наличие переданных идентификатора процесса и пути для сохранения архива
            if (ProcessId == 0 || string.IsNullOrEmpty(OutputPath))
            {
                Console.WriteLine("Error: Null id of process or path");
                return 1;
            }

            try
            {
                // Вызов метода API для скачивания архива
                if (await apiHandler.DownloadArchiveAsync(ProcessId, OutputPath))
                {
                    // Вывод сообщения в случае успешного скачивания архива
                    Console.WriteLine($"Archive downloaded to {OutputPath}");
                }
                else
                {
                    Console.WriteLine($"Error: Failed downloaded to {OutputPath}");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений и вывод сообщения об ошибке
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Возвращение кода завершения 0, что означает успешное выполнение
            return 0;
        }
    }
}
