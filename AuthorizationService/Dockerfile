# Используем официальный образ .NET SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем sln и csproj файлы для кэширования
COPY AuthorizationService/AuthorizationService.sln AuthorizationService/
COPY AuthorizationService/Core/Core.csproj AuthorizationService/Core/
COPY AuthorizationService/Infrastructure/Infrastructure.csproj AuthorizationService/Infrastructure/
COPY AuthorizationService/Shared/Shared.csproj AuthorizationService/Shared/
COPY AuthorizationService/Web/Web.csproj AuthorizationService/Web/
COPY AuthorizationService/IntegrationTests/IntegrationTests.csproj AuthorizationService/IntegrationTests/
COPY AuthorizationService/ModularTests/ModularTests.csproj AuthorizationService/ModularTests/
COPY Common/Common.csproj Common/

# Восстанавливаем зависимости
RUN dotnet restore AuthorizationService/AuthorizationService.sln

# Копируем весь код проекта
COPY AuthorizationService AuthorizationService/
COPY Common Common/

# Копируем .proto файл вручную
COPY AuthorizationService/Shared/Protos/auth-proto.proto AuthorizationService/Shared/Protos/

# Сборка проекта
WORKDIR /app/AuthorizationService/Web
RUN dotnet publish -c Release -o /out --no-restore

# Используем Runtime для минимизации образа
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Настраиваем порты и переменные среды
ENV ASPNETCORE_URLS=https://+:5120
EXPOSE 5120

# Запуск приложения
ENTRYPOINT ["dotnet", "Web.dll"]
