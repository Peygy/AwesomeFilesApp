namespace ApiService.Models
{
    /// <summary>
    /// Модель, описывающая процесс создания zip-архива файлов
    /// </summary>
    public class ArchiveCreationProcess
    {
        // Универсальная идентификатор 
        public int Id { get; set; }
        // Статус выполнения процесса создания архива
        public string Status { get; set; } = null!;
        // Время создания процесса (по умолчанию задается текущее время с момента создания объекта класса)
        public DateTime CreationTime{ get; set; } = DateTime.Now;

        // Коллекция путей файлов, находящихся внутри zip-архива
        public IEnumerable<string> FilePaths { get; set; } = null!;
    }
}
