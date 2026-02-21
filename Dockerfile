# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SchoolSystem.Backend/SchoolSystem.Backend.csproj", "SchoolSystem.Backend/"]
COPY ["SchoolSystem.Domain/SchoolSystem.Domain.csproj", "SchoolSystem.Domain/"]

RUN dotnet restore "SchoolSystem.Backend/SchoolSystem.Backend.csproj"
COPY . .
RUN dotnet build "SchoolSystem.Backend/SchoolSystem.Backend.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "SchoolSystem.Backend/SchoolSystem.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchoolSystem.Backend.dll"]
