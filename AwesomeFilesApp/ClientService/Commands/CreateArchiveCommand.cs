using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    /// <summary>
    /// Класс команды для создания архива.
    /// </summary>
    [Command(Name = "create-archive", Description = "Команда для создания архива")]
    public class CreateArchiveCommand
    {
        private readonly IApiHandler apiHandler;

        /// <summary>
        /// Список имен файлов для создания архива, передаваемый в командной строке.
        /// </summary>
        [Argument(0, "fileNames", "Список имен файлов для создания архива")]
        public List<string> FileNames { get; set; } = new List<string>();

        /// <summary>
        /// Конструктор класса команды, принимает обработчик ApiHandler для взаимодействия с API.
        /// </summary>
        /// <param name="apiHandler">Обработчик ApiHandler для выполнения запросов к API.</param>
        public CreateArchiveCommand(IApiHandler apiHandler)
        {
            this.apiHandler = apiHandler;
        }

        /// <summary>
        /// Асинхронный метод, выполняемый при запуске команды.
        /// </summary>
        /// <returns>Задача, представляющая результат выполнения команды.</returns>
        public async Task<int> OnExecuteAsync()
        {
            try
            {
                // Вызов метода API для создания архива с указанными именами файлов
                var processId = await apiHandler.CreateArchiveAsync(FileNames);
                // Вывод идентификатора процесса архивации, если он не равен 0
                if (processId != 0)
                {
                    Console.WriteLine(processId);
                }
                else
                {
                    Console.WriteLine("Error: Failed to create archive");
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
