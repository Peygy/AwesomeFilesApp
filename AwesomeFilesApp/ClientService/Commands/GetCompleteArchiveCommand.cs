using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    /// <summary>
    /// Класс команды для создания архива, проверки статуса и скачивания архива.
    /// </summary>
    [Command(Name = "complete-archive", Description = "Создать архив, проверить статус и скачать его")]
    public class GetCompleteArchiveCommand
    {
        private readonly IApiHandler apiHandler;

        /// <summary>
        /// Путь для сохранения архива, передаваемый в командной строке.
        /// </summary>
        [Argument(0, "outputPath", "Путь для сохранения архива")]
        public string OutputPath { get; set; } = null!;

        /// <summary>
        /// Список имен файлов для архивации, передаваемый в командной строке.
        /// </summary>
        [Argument(1, "fileNames", "Список имен файлов для архивации")]
        public List<string> FileNames { get; set; } = new List<string>();

        /// <summary>
        /// Конструктор класса команды, принимает обработчик ApiHandler для взаимодействия с API.
        /// </summary>
        /// <param name="apiHandler">Обработчик ApiHandler для выполнения запросов к API.</param>
        public GetCompleteArchiveCommand(IApiHandler apiHandler)
        {
            this.apiHandler = apiHandler;
        }

        /// <summary>
        /// Асинхронный метод, выполняемый при запуске команды.
        /// </summary>
        /// <returns>Задача, представляющая результат выполнения команды.</returns>
        public async Task<int> OnExecuteAsync()
        {
            // Проверка на наличие переданных имен файлов и пути для сохранения архива
            if (FileNames == null || FileNames.Count == 0 || string.IsNullOrEmpty(OutputPath))
            {
                Console.WriteLine("Error: Необходимо указать имена файлов и путь для сохранения архива.");
                return 1;
            }

            try
            {
                // Вызов метода API для создания архива с указанными именами файлов
                var processId = await apiHandler.CreateArchiveAsync(FileNames);
                // Проверка на созданный процесс создания архива
                if (processId == 0)
                {
                    Console.WriteLine("Ошибка создания архива");
                    return 1;
                }
                Console.WriteLine($"Create archive task is started, id: {processId}");

                // Цикл для проверки статуса процесса архивации
                while (true)
                {
                    // Вызов метода API для получения статуса выполнения процесса
                    var status = await apiHandler.GetArchiveStatusAsync(processId);
                    Console.WriteLine($"Process status: {status}");

                    // Выход из цикла при успешном завершении архивации
                    if (status == "Успешно")
                    {
                        break;
                    }

                    // Ожидание перед повторной проверкой статуса
                    await Task.Delay(3000);
                }

                // Скачивание архива при успешном завершении процесса архивации
                if (await apiHandler.DownloadArchiveAsync(processId, OutputPath))
                {
                    Console.WriteLine($"Archive downloaded to {OutputPath}");
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений и вывод сообщения об ошибке
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }

            // Возвращение кода завершения 0, что означает успешное выполнение
            return 0;
        }
    }
}
