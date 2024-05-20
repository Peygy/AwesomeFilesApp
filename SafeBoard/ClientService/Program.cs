using ClientService;
using ClientService.Commands;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

[Command(Name = "program", Description = "Точка заупска приложения")]
[Subcommand(typeof(ClientStartCommand))]
public class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddSingleton(_ => new ApiHandler("http://localhost:8080"))
            .BuildServiceProvider();

        var app = new CommandLineApplication<Program>();
        app.Conventions.UseDefaultConventions().UseConstructorInjection(services);

        app.Execute(args);
    }

    private async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
    {
        while (true)
        {
            console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                console.WriteLine("Empty command");
                continue;
            }

            if (input == "client")
            {
                try
                {
                    var commandApp = app.Commands.SingleOrDefault(c => c.Name == input);
                    if (commandApp == null)
                    {
                        console.WriteLine("Unknown command");
                        continue;
                    }

                    await commandApp.ExecuteAsync([]);
                    return 0;
                }
                catch (Exception ex)
                {
                    console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                console.WriteLine("The client is not running");
                continue;
            }

            return 0;
        }
    }
}