using ApiService.Data;
using ApiService.Models;

namespace ApiService.Middlewares
{
    public class LogDataMiddleware
    {
        private readonly RequestDelegate next;

        public LogDataMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, LogDataContext logDataContext)
        {
            // Создание модели данных
            var requestLog = new LogDataModel
            {
                Method = context.Request.Method,
                QueryPath = context.Request.Path
            };

            // Считывание данных из тела запроса и запись в модель
            using (var reader = new StreamReader(context.Request.Body))
            {
                // Читаем все данные из тела запроса
                requestLog.Body = await reader.ReadToEndAsync();
                // Перезаписываем тело запроса снова в MemoryStream, чтобы его можно было прочитать еще раз
                context.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestLog.Body));
            }

            // Добавление в базу данных
            await logDataContext.Logs.AddAsync(requestLog);
            // Фиксация изменений
            await logDataContext.SaveChangesAsync();

            // Переход к следующему элементу конвеера запроса
            await next.Invoke(context);
        }
    }
}
