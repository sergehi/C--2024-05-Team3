#ASPNETCORE_ENVIRONMENT=Development

# ���������� ����������� ����� .NET SDK ��� ������
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
EXPOSE 5248

# �������� .csproj � ��������������� �����������
COPY TasksService/*.csproj ./TasksService/
COPY CommonNet/*.csproj ./CommmonNet/

# ��������������� �����������
RUN dotnet restore TasksService/TasksService.csproj #--interactive 

# �������� ��������� ��� � �������� ����������
COPY TasksService/. ./TasksService/
COPY CommonNet/. ./CommonNet/

RUN dotnet publish TasksService/TasksService.csproj -c Release -o out

# ���������� ����������� ����� .NET Runtime ��� ������� ����������
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .


ENV ASPNETCORE_URLS=http://+:5248

# ��������� ������� ��� ������� ����������
ENTRYPOINT ["dotnet", "TasksService.dll"]

# � ��������� ������ ��������� ���������������:
# docker build -t tasksservice .
#���� ������������ postgresql � ����������:
	#// ���������� postgres � ������
	#docker run --name postgres-container -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=########## -p 5433:5432 -d postgres
	#// ������ �����������/�������� ���� � ������
	#psql -h 172.17.0.3 -U postgres -d TasksDb
	#// � ���� ������� ��������� ��������� tasksservice. 
	#// ��� ���� � ����������
	#docker run -d -p 5248:5248 -e "ConnectionStrings__TasksDb=Host=172.17.0.3;Port=5433;Database=TasksDb;Username=postgres;Password=##########;Persist Security Info=True" --name tasksservice d9dacce6aca8031f1e860756c1d98bd50890e9a36693e2a20f1f2ebecce3abe9
#
#���� ������������ postgresql �� ��������� ������:
	#// ��� ����������� � ��������� ������
	#docker run -d -p 5248:5248 -e "ConnectionStrings__TasksDb=Host=192.168.10.83;Port=5432;Database=TasksDb;Username=postgres;Password=##########;Persist Security Info=True" --name tasksservice d9dacce6aca8031f1e860756c1d98bd50890e9a36693e2a20f1f2ebecce3abe9
#
#

