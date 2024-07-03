using System.Net.Http.Json;

namespace ClientService
{
    /// <summary>
    /// Класс для обработки запросов к API.
    /// </summary>
    public class ApiHandler : IApiHandler
    {
        private readonly HttpClient httpClient;
        private int archivesCounter;

        /// <summary>
        /// Конструктор, инициализирующий HttpClient с базовым адресом.
        /// </summary>
        /// <param name="apiAddress">Базовый адрес для HttpClient.</param>
        public ApiHandler(string apiAddress)
        {
            httpClient = new HttpClient { BaseAddress = new Uri(apiAddress) };
            // Счетчик пользовательских архивов, который добавляется к названию архива
            archivesCounter = 0;
        }

        /// <summary>
        /// Асинхронный метод для получения списка файлов с сервера.
        /// </summary>
        /// <returns>Коллекция имен файлов.</returns>
        public async Task<IEnumerable<string>> GetFilesAsync()
        {
            // Отправка GET-запроса к API для получения списка файлов
            var response = await httpClient.GetAsync("api/files");

            // Проверка успешности запроса
            if (!response.IsSuccessStatusCode)
            {
                // Обработка ошибки в случае неудачного запроса
                await HandleErrorAsync(response);
                return null;
            }

            // Чтение и возврат списка имен файлов из ответа
            return await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
        }

        /// <summary>
        /// Асинхронный метод для создания архива из списка файлов.
        /// </summary>
        /// <param name="fileNames">Список имен файлов для архивации.</param>
        /// <returns>Идентификатор процесса архивации.</returns>
        public async Task<int> CreateArchiveAsync(IEnumerable<string> fileNames)
        {
            // Создание содержимого запроса
            var content = new { fileNames = fileNames };

            // Отправка POST-запроса к API для создания архива
            var response = await httpClient.PostAsJsonAsync("api/files/archive", content);

            // Проверка успешности запроса
            if (!response.IsSuccessStatusCode)
            {
                // Обработка ошибки в случае неудачного запроса
                await HandleErrorAsync(response);
                return 0;
            }

            // Чтение и возврат идентификатора процесса архивации
            return await response.Content.ReadFromJsonAsync<int>();
        }

        /// <summary>
        /// Асинхронный метод для получения статуса архивации по идентификатору процесса.
        /// </summary>
        /// <param name="processId">Идентификатор процесса архивации.</param>
        /// <returns>Статус архивации в виде строки.</returns>
        public async Task<string> GetArchiveStatusAsync(int processId)
        {
            // Отправка GET-запроса к API для получения статуса архивации
            var response = await httpClient.GetAsync($"api/files/archive/{processId}");

            // Проверка успешности запроса
            if (!response.IsSuccessStatusCode)
            {
                // Обработка ошибки в случае неудачного запроса
                await HandleErrorAsync(response);
                return null;
            }

            // Чтение и возврат статуса архивации
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Асинхронный метод для скачивания архива по идентификатору процесса.
        /// </summary>
        /// <param name="processId">Идентификатор процесса архивации.</param>
        /// <param name="outputPath">Путь для сохранения скачанного архива.</param>
        /// <returns>Истина, если архив успешно скачан, иначе ложь.</returns>
        public async Task<bool> DownloadArchiveAsync(int processId, string outputPath)
        {
            // Отправка GET-запроса к API для скачивания архива
            var response = await httpClient.GetAsync($"api/files/archive/{processId}/download");

            // Проверка успешности запроса
            if (!response.IsSuccessStatusCode)
            {
                // Обработка ошибки в случае неудачного запроса
                await HandleErrorAsync(response);
                return false;
            }

            // Создание уникального имени файла для архива
            outputPath = Path.Combine(outputPath, $"archive{archivesCounter}.zip");
            archivesCounter++;

            // Сохранение скачанного архива на диск
            await using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            // Копирование скачанного архива в указанную директорию
            await response.Content.CopyToAsync(fileStream);

            return true;
        }

        /// <summary>
        /// Асинхронный метод для обработки ошибок при запросах к API.
        /// </summary>
        /// <param name="response">Ответ с ошибкой от API.</param>
        /// <returns>Задача, представляющая завершение обработки ошибки.</returns>
        private async Task HandleErrorAsync(HttpResponseMessage response)
        {
            // Чтение сообщения об ошибке из ответа
            var errorMessage = await response.Content.ReadAsStringAsync();

            // Вывод сообщения об ошибке в консоль
            Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
        }
    }
}
