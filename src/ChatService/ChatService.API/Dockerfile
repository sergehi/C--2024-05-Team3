FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5104

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/ChatService/ChatService.API/ChatService.API.csproj", "src/ChatService/ChatService.API/"]
COPY ["src/ChatService/Infrastructure/ChatService.Infrastructure.EntityFramework/ChatService.Infrastructure.EntityFramework.csproj", "ChatService.Infrastructure.EntityFramework/"]
COPY ["src/ChatService/Infrastructure/ChatService.Infrastructure.Repositories.Implementations/ChatService.Infrastructure.Repositories.Implementations.csproj", "ChatService.Infrastructure.Repositories.Implementations/"]
COPY ["src/ChatService/Services/ChatService.Services.Abstractions/ChatService.Services.Abstractions.csproj", "ChatService.Services.Abstractions/"]
COPY ["src/ChatService/Services/ChatService.Services.Implementations/ChatService.Services.Implementations.csproj", "ChatService.Services.Implementations/"]
COPY ["Common/Common.csproj", "Common/Common.csproj"]
RUN dotnet restore "src/ChatService/ChatService.API/ChatService.API.csproj"
COPY . .

WORKDIR /src/src/ChatService/ChatService.API
RUN dotnet build "ChatService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5104
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "ChatService.API.dll"]