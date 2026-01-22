# CurrencyProject

Микросервисная система для работы с курсами валют на базе .NET 8 и PostgreSQL.

## Структура проекта

- **CurrencyProject.Domain** - доменные сущности
- **CurrencyProject.Infrastructure** - EF Core, миграции
- **CurrencyProject.MigrationService** - применение миграций БД
- **CurrencyProject.CurrencyUpdater** - фоновый сервис обновления курсов с ЦБ РФ
- **CurrencyProject.UserService** - микросервис пользователя (Clean Architecture + CQRS)
- **CurrencyProject.FinanceService** - микросервис финансов (Clean Architecture + CQRS)
- **CurrencyProject.ApiGateway** - API Gateway для маршрутизации запросов

## Быстрый старт

### 1. Настройка базы данных

Обновите строку подключения в `appsettings.json` всех сервисов:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CurrencyDb;Username=postgres;Password=your_password"
  }
}
```

Примените миграции:
```bash
cd src/CurrencyProject.MigrationService
dotnet run
```

### 2. Запуск сервисов

```bash
# Фоновый сервис обновления курсов
cd src/CurrencyProject.CurrencyUpdater
dotnet run

# Микросервис пользователя (порт 5001)
cd src/CurrencyProject.UserService
dotnet run

# Микросервис финансов (порт 5002)
cd src/CurrencyProject.FinanceService
dotnet run

# API Gateway (порт 5000)
cd src/CurrencyProject.ApiGateway
dotnet run
```

## Технологии

- .NET 8
- PostgreSQL
- Entity Framework Core
- MediatR (CQRS)
- JWT Authentication
- YARP (API Gateway)
- xUnit
