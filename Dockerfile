FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ERPManifestoAPI.sln .
COPY Core/Domain/Domain.csproj Core/Domain/
COPY Core/Application/Application.csproj Core/Application/
COPY Infrastructure/Persistence/Persistence.csproj Infrastructure/Persistence/
COPY Infrastructure/Services/Services.csproj Infrastructure/Services/
COPY Presentation/WebApi/WebApi.csproj Presentation/WebApi/

RUN dotnet restore

# Copiar todo el código y compilar
COPY . .
RUN dotnet publish Presentation/WebApi/WebApi.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WebApi.dll"]
