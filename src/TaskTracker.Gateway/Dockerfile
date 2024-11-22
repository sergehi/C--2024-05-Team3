# Используем официальный образ .NET SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем sln и csproj файлы для кэширования
COPY src/TaskTracker.Gateway/TaskTracker.Gateway.csproj src/TaskTracker.Gateway/
COPY Common/Common.csproj Common/

# Восстанавливаем зависимости
RUN dotnet restore src/TaskTracker.Gateway/TaskTracker.Gateway.csproj

# Копируем исходный код
COPY src/TaskTracker.Gateway src/TaskTracker.Gateway/
COPY Common Common/

# Копируем .proto файлы вручную
COPY src/TaskTracker.Gateway/Protos/ src/TaskTracker.Gateway/Protos/
COPY AuthorizationService/Shared/Protos/auth-proto.proto AuthorizationService/Shared/Protos/
COPY TasksService/TasksService/Protos/tasksService.proto TasksService/TasksService/Protos/

# Сборка проекта
WORKDIR /app/src/TaskTracker.Gateway
RUN dotnet publish -c Release -o /out --no-restore

# Используем Runtime для минимизации образа
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем собранные файлы из предыдущего этапа
COPY --from=build /out ./

# Указываем порты и переменные среды
EXPOSE 8081

# Запускаем приложение
ENTRYPOINT ["dotnet", "TaskTracker.Gateway.dll"]
