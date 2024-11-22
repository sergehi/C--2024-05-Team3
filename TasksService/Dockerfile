# Используем официальный образ .NET SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем sln и csproj файлы для кэширования
COPY TasksService/TasksService/TasksService.csproj TasksService/TasksService/
COPY TasksService/BLL/TasksService.BusinessLogic.DTO/TasksService.BusinessLogic.DTO.csproj TasksService/BLL/TasksService.BusinessLogic.DTO/
COPY TasksService/BLL/TasksService.BusinessLogic.Services.Abstractions/TasksService.BusinessLogic.Services.Abstractions.csproj TasksService/BLL/TasksService.BusinessLogic.Services.Abstractions/
COPY TasksService/BLL/TasksService.BusinessLogic.Services.Implementations/TasksService.BusinessLogic.Services.Implementations.csproj TasksService/BLL/TasksService.BusinessLogic.Services.Implementations/
COPY TasksService/DAL/TasksService.DataAccess.Entities/TasksService.DataAccess.Entities.csproj TasksService/DAL/TasksService.DataAccess.Entities/
COPY TasksService/DAL/TasksService.DataAccess.EntityFramework/TasksService.DataAccess.EntityFramework.csproj TasksService/DAL/TasksService.DataAccess.EntityFramework/
COPY TasksService/DAL/TasksService.DataAccess.Repositories.Abstractions/TasksService.DataAccess.Repositories.Abstractions.csproj TasksService/DAL/TasksService.DataAccess.Repositories.Abstractions/
COPY TasksService/DAL/TasksService.DataAccess.Repositories.Implementations/TasksService.DataAccess.Repositories.Implementations.csproj TasksService/DAL/TasksService.DataAccess.Repositories.Implementations/
COPY TasksService/Tests/TasksTests/TasksTests.csproj TasksService/Tests/TasksTests/
COPY Common/Common.csproj Common/

RUN dotnet restore TasksService/TasksService/TasksService.csproj

# Копируем весь код проекта
COPY TasksService TasksService/
COPY Common Common/

# Копируем .proto файл вручную
COPY TasksService/TasksService/Protos/tasksService.proto TasksService/TasksService/Protos/

# Сборка проекта
WORKDIR /app/TasksService/TasksService
RUN dotnet publish -c Release -o /out --no-restore

# Используем Runtime для минимизации образа
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

EXPOSE 7279

# Запуск приложения
ENTRYPOINT ["dotnet", "TasksService.dll"]
