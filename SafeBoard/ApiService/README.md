# AwesomeFiles API

## ��������

���� ������ ������������� REST API ��� ��������� ���������� � ������, �������� ����� �� �������� ������ � ����� � ���������� ���� �������.

## ������ �������

1. ���������, ��� � ��� ���������� .NET SDK.
2. ����������� ����������� � ��������� � ���������� ������� `SafeBoard`.
3. �������� ���������� `filees` � `ArchivesDirectory` � �������� �����.
4. ��������� ������ �������� (Powershell):

```bash
dotnet run
```

#### ������ ������� ����� Docker

1. ���������, ��� � ��� ���������� Docker
2. �������� ����� ������� �������� (Powershell):

```bash
docker build -f "./ApiService/Dockerfile" --force-rm -t apiservice --build-arg "BUILD_CONFIGURATION=Release" --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=ApiService" .
```

4. ����� ������ ������, ��������� ��������� �������� (Powershell):

```bash
docker run -d -p 8080:8080 -v "$(pwd)/files:/app/files" --name api apiservicedocker run -d -p 8080:8080 -v "$(pwd)/files:/app/files" --name api apiservice
```

## ������ �������

#### ������ ����� �������� �� ������ http://localhost:8080.

__�������� ����� ����������� API:__
1. __GET__ /api/files: �������� ������ ���� ������.
2. __POST__ /api/files/archive: ���������������� ������� �������������. ��������� ������ ���� ������ � ������� JSON � ���������� ���������� ID ��������.
3. __GET__ /api/files/archive/ID: ��������� ������ �������� �������������.
4. __GET__ /api/files/archive/ID/download: ������� ����� �� ID ��������.

___����������:___
��� ������� ���������� � �������.
� ������ ������������� �����, ������������ ������������ ��������� �� ������.

## ������������

#### ������

��������� ������ � ��������� ������ ���������� � ������� Postman ��� ������� ������� API.

#### ��������������

1. ��������� � �������� ������ SafeBoard/ApiService.Tests
2. ��������� ���������� ������ � ������� �������:

```bash
dotnet test
```

3. ����� ���������� ������, ����� ������� ����� ���������� ������