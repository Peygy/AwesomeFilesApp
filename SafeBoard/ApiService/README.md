# AwesomeFiles API

## Описание

Этот проект предоставляет REST API для получения информации о файлах, создания задач на упаковку файлов в архив и скачивания этих архивов.

## Запуск сервиса

1. Убедитесь, что у вас установлен .NET SDK.
2. Склонируйте репозиторий и перейдите в директорию проекта `SafeBoard`.
3. Создайте директорию `filees` и `ArchivesDirectory` и добавьте файлы.
4. Запустите сервис командой (Powershell):

```bash
dotnet run
```

#### Запуск сервиса через Docker

1. Убедитесь, что у вас установлен Docker
2. Соберите образ сервиса командой (Powershell):

```bash
docker build -f "./ApiService/Dockerfile" --force-rm -t apiservice --build-arg "BUILD_CONFIGURATION=Release" --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=ApiService" .
```

4. После сборки образа, запустите контейнер командой (Powershell):

```bash
docker run -d -p 8080:8080 -v "$(pwd)/files:/app/files" --name api apiservicedocker run -d -p 8080:8080 -v "$(pwd)/files:/app/files" --name api apiservice
```

## Работа сервиса

#### Сервис будет доступен по адресу http://localhost:8080.

__Конечные точки пользования API:__
1. __GET__ /api/files: Получить список всех файлов.
2. __POST__ /api/files/archive: Инициализировать процесс архивирования. Принимает список имен файлов в формате JSON и возвращает уникальный ID процесса.
3. __GET__ /api/files/archive/ID: Проверить статус процесса архивирования.
4. __GET__ /api/files/archive/ID/download: Скачать архив по ID процесса.

___Примечания:___
Все запросы логируются в консоль.
В случае некорректного ввода, пользователю возвращается сообщение об ошибке.

## Тестирование

#### Ручное

Запустите сервис и проверьте работу эндпоинтов с помощью Postman или другого клиента API.

#### Автоматическое

1. Перейдите в тестовый проект SafeBoard/ApiService.Tests
2. Запустите выполнение тестов с помощью команды:

```bash
dotnet test
```

3. После выполнения тестов, будет выведен отчет выполнения тестов