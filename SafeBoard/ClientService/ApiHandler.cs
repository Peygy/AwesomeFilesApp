using System.Net.Http.Json;

namespace ClientService
{
    internal class ApiHandler
    {
        private readonly HttpClient httpClient;
        private int archivesCounter;

        public ApiHandler(string baseAddress)
        {
            httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            archivesCounter = 0;
        }

        public async Task<IEnumerable<string>> GetFilesAsync()
        {
            var response = await httpClient.GetAsync("api/files");
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
        }

        public async Task<int> CreateArchiveAsync(IEnumerable<string> fileNames)
        {
            var content = new { fileNames = fileNames };
            var response = await httpClient.PostAsJsonAsync("api/files/archive", content);
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response);
                return 0;
            }
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<string> GetArchiveStatusAsync(int processId)
        {
            // 725456
            var response = await httpClient.GetAsync($"api/files/archive/{processId}");
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response);
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> DownloadArchiveAsync(int processId, string outputPath)
        {
            var response = await httpClient.GetAsync($"api/files/archive/{processId}/download");
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response);
                return false;
            }

            outputPath = Path.Combine(outputPath, $"archive{archivesCounter}.zip");
            archivesCounter++;

            await using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            await response.Content.CopyToAsync(fs);
            return true;
        }

        private async Task HandleErrorResponseAsync(HttpResponseMessage response)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
        }
    }
}
