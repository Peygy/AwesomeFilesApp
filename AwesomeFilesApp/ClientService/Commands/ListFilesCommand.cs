﻿using McMaster.Extensions.CommandLineUtils;

namespace ClientService.Commands
{
    /// <summary>
    /// Класс команды для вывода списка файлов.
    /// </summary>
    [Command(Name = "list", Description = "Команда вывода файлов")]
    public class ListFilesCommand
    {
        private readonly IApiHandler apiHandler;

        /// <summary>
        /// Конструктор класса команды, принимает обработчик ApiHandler для взаимодействия с API.
        /// </summary>
        /// <param name="apiHandler">Обработчик для выполнения запросов к API.</param>
        public ListFilesCommand(IApiHandler apiHandler)
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
                // Вызов метода API для получения списка файлов
                var files = await apiHandler.GetFilesAsync();
                // Вывод списка файлов в консоль
                Console.WriteLine(string.Join(" ", files));
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
