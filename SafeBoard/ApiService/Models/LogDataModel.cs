namespace ApiService.Models
{
    /// <summary>
    /// Класс модели данных в базе данных
    /// </summary>
    public class LogDataModel
    {
        // Уникальный идентификатор
        public int Id { get; set; }
        // HTTP-метод запроса
        public string Method { get; set; } = null!;
        // Путь запроса
        public string QueryPath { get; set; } = null!;
        // Тело запроса
        public string Body { get; set; } = null!;
        // Время отправки запроса
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
