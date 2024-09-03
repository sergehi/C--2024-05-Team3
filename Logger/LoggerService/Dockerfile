# Используем официальный образ .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 5113

RUN echo /app
# Копируем .csproj и зависимости
COPY ["Logger/LoggerService/LoggerService.csproj", "Logger/LoggerService/"]
COPY ["Logger/BLL/Logger.BusinessLogic.Services.Implementations/Logger.BusinessLogic.Services.Implementations.csproj", "Logger/BLL/Logger.BusinessLogic.Services.Implementations/"]
COPY ["Logger/DAL/Logger.DataAccess.Entities/Logger.DataAccess.Entities.csproj", "Logger/DAL/Logger.DataAccess.Entities/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Logger/DAL/Logger.DataAccess.Repositories.Abstractions/Logger.DataAccess.Repositories.Abstractions.csproj", "Logger/DAL/Logger.DataAccess.Repositories.Abstractions/"]
COPY ["Logger/BLL/Logger.BusinessLogic.DTO/Logger.BusinessLogic.DTO.csproj", "Logger/BLL/Logger.BusinessLogic.DTO/"]
COPY ["Logger/BLL/Logger.BusinessLogic.Services.Abstractions/Logger.BusinessLogic.Services.Abstractions.csproj", "Logger/BLL/Logger.BusinessLogic.Services.Abstractions/"]
COPY ["Logger/DAL/Logger.DataAccess.EntityFramework/Logger.DataAccess.EntityFramework.csproj", "Logger/DAL/Logger.DataAccess.EntityFramework/"]
COPY ["Logger/DAL/Logger.DataAccess.Repositories.Implementations/Logger.DataAccess.Repositories.Implementations.csproj", "Logger/DAL/Logger.DataAccess.Repositories.Implementations/"]

# Восстанавливаем зависимости
RUN dotnet restore "Logger/LoggerService/LoggerService.csproj"
COPY . .
RUN dotnet build "Logger/LoggerService/LoggerService.csproj" -c Release -o /app/out

# Используем официальный образ .NET Runtime для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:5113

# Указываем команду для запуска приложения
ENTRYPOINT ["dotnet", "LoggerService.dll"]

#Сборка Docker-образа: docker build -f Logger/LoggerService/Dockerfile -t loggerservice .
#Запуск контейнера: docker run -d -p 5113:5113 -e "ConnectionStrings__LoggerDB=Server=31.128.43.53;Port=5432;Username=postgres;Database=LoggerService;Password=XXXXX" --name LoggerService b71f18fa6f2a63110fd768cd879c4c22611cf758fe40d3117422454f961176e3