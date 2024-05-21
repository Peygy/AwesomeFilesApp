namespace ClientService
{
    /// <summary>
    /// Интерфейс класса для обработки запросов к API.
    /// </summary>
    public interface IApiHandler
    {
        Task<IEnumerable<string>> GetFilesAsync();
        Task<int> CreateArchiveAsync(IEnumerable<string> fileNames);
        Task<string> GetArchiveStatusAsync(int processId);
        Task<bool> DownloadArchiveAsync(int processId, string outputPath);
    }
}
