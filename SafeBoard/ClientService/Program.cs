using ClientService;
using ClientService.Commands;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Главный класс программы, который служит точкой входа для приложения.
/// </summary>
[Command(Name = "program", Description = "Точка входа приложения")]
[Subcommand(typeof(ClientStartCommand))]
public class Program
{
    /// <summary>
    /// Главный метод, точка входа для приложения.
    /// </summary>
    /// <param name="args">Аргументы командной строки.</param>
    static void Main(string[] args)
    {
        // Создание коллекции сервисов и внедрение зависимостей
        var services = new ServiceCollection()
            // Регистрация ApiHandler как сервиса
            .AddSingleton(_ => new ApiHandler("http://localhost:8080"))
            // Построение провайдера сервисов.
            .BuildServiceProvider();

        // Создание экземпляра приложения
        var app = new CommandLineApplication<Program>();
        // Настройка приложения с использованием стандартных соглашений и внедрения зависимостей через конструктор
        app.Conventions.UseDefaultConventions().UseConstructorInjection(services);

        // Выполнение приложения с переданными аргументами
        app.Execute(args);
    }

    /// <summary>
    /// Асинхронный метод, выполняемый при запуске команды.
    /// </summary>
    /// <param name="app">Экземпляр приложения командной строки.</param>
    /// <param name="console">Интерфейс консоли для ввода и вывода данных.</param>
    /// <returns>Задача, представляющая результат выполнения команды.</returns>
    private async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
    {
        while (true)
        {
            console.Write("> ");
            // Чтение команды из консоли
            var input = Console.ReadLine();

            // Проверка на пустой ввод
            if (string.IsNullOrEmpty(input))
            {
                console.WriteLine("Empty command");
                continue;
            }

            // Проверка введенной команды
            if (input == "client")
            {
                try
                {
                    // Поиск команды с именем "client"
                    var commandApp = app.Commands.SingleOrDefault(c => c.Name == input);
                    // Если команда не найдена, вывод сообщения об ошибке
                    if (commandApp == null)
                    {
                        console.WriteLine("Unknown command");
                        continue;
                    }

                    // Выполнение найденной команды
                    await commandApp.ExecuteAsync([]);
                    return 0;
                }
                catch (Exception ex)
                {
                    // Обработка исключений и вывод сообщения об ошибке
                    console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                // Вывод сообщения, если команда не распознана
                console.WriteLine("The client is not running");
                continue;
            }

            return 0;
        }
    }
}