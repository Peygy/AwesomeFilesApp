# AwesomeFiles API

## Описание

Этот проект предоставляет консольную утилиту для взаимодействия с REST API, который позволяет получать информацию о файлах, создавать задачи на упаковку файлов в архив и скачивать эти архивы.

## Запуск сервиса

1. Убедитесь, что у вас установлен .NET SDK.
2. Склонируйте репозиторий и перейдите в директорию проекта `SafeBoard/ClientService`.
3. Запустите сервис командой (Powershell):

```bash
dotnet run --configuration Release
```

#### Запуск сервиса через dll

1. Убедитесь, что у вас установлен .NET SDK.
2. Перейдите в директорию `SafeBoard/ClientService/bin/Release/net8.0`.
3. Запустите сервис командой (Powershell):

```bash
dotnet ClientService.dll
```

## Использование клиента

После запуска клиента, введите client для начала работы:

```bash
> client
Client was started.
Press <Enter> to exit...
>
```

### Доступные команды

1. __list__: Получить список всех файлов

```bash
> list
file1 file2 file3 file4
```

2. __create-archive__: Создать задачу на архивирование файлов

```bash
> create-archive file1 file2 file4
291932
```

3. __status__: Проверить статус процесса архивирования

```bash
> status 291932
Process in progress, please wait...
```

4. __download__: Скачать архив по ID процесса
```bash
> download 291932 %folder_path%
```

5. __complete-archive__: Создать задачу на архивирование файлов, по завершению которой архив будет скачан в указанную директорию

```bash
> complete-archive %folder_path% file1 file2
Create archive task is started, id: 141140
Process status: Успешно
Archive downloaded to %folder_path%
```

___Примечания:___
Убедитесь, что API-сервис запущен и доступен по URL, указанному в BaseUrl (по умолчанию http://localhost:8080).
В случае некорректного ввода данных пользователю возвращается сообщение об ошибке.

## Тестирование

#### Ручное

Запустите клиент и проверьте работу команд.

#### Автоматическое (в DEBUG)

1. Перейдите в тестовый проект `SafeBoard/ClientService.Tests`.
2. Запустите выполнение тестов с помощью команды:

___Примечание:___ Лучше выполнять тесты по отдельности (кроме ApiHandlerTests), так как консольные выводы тестов перебивают другие тестовые выводы!

```bash
dotnet test --filter "FullyQualifiedName=ClientService.Tests.TestClassName.TestMethodName"
```

3. После выполнения тестов, будет выведен отчет выполнения тестов
