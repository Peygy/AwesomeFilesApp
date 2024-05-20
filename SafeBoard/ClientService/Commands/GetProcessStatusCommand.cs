using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    /// <summary>
    /// Класс команды для получения статуса процесса создания архива.
    /// </summary>
    [Command(Name = "status", Description = "Команда для получения статуса процесса создания архива")]
    internal class GetProcessStatusCommand
    {
        private readonly ApiHandler apiHandler;

        /// <summary>
        /// Идентификатор процесса архивации, передаваемый в командной строке.
        /// </summary>
        [Argument(0, "processId", "Идентификатор процесса архивации")]
        public int ProcessId { get; set; }

        /// <summary>
        /// Конструктор класса команды, принимает обработчик ApiHandler для взаимодействия с API.
        /// </summary>
        /// <param name="apiHandler">Обработчик ApiHandler для выполнения запросов к API.</param>
        public GetProcessStatusCommand(ApiHandler apiHandler)
        {
            this.apiHandler = apiHandler;
        }

        /// <summary>
        /// Асинхронный метод, выполняемый при запуске команды.
        /// </summary>
        /// <returns>Задача, представляющая результат выполнения команды.</returns>
        private async Task<int> OnExecuteAsync()
        {
            try
            {
                // Вызов метода API для получения статуса процесса архивации
                var status = await apiHandler.GetArchiveStatusAsync(ProcessId);

                // Формирование сообщения в зависимости от статуса процесса
                string message = status switch
                {
                    "Выполняется" => "Process in progress, please wait...",
                    "Успешно" => "Archive has been created.",
                    "Провален" => "Archive creation is failed.",
                    _ => "Unknown status."
                };

                Console.WriteLine(message);
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
