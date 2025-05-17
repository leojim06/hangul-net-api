# build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solución y proyecto
COPY HangulApi.sln .
COPY HangulApi/ ./HangulApi/
WORKDIR /src/HangulApi

# Restaurar y publicar
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HangulApi.dll"]