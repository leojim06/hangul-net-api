# Creacion de Docker

Ejecutar en la terminal para construir la imagen

```bash
podman build -t hangul-api:dev .
```

Ejecutar en la terminal para crear red, volumen y contenedores

```bash
# Crear net
podman network create hangul-net

# Crear volumen de datos
podman volume create hangul-sqlserver-data

# Eliminar contenedores existentes (opcional)
podman stop hangul-api sqlserver
podman rm hangul-api sqlserver


# Iniciar contenedor de SQL Server con volumen y red
podman run -d --name sqlserver --network hangul-net -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=admin123!" -v hangul-sqlserver-data:/var/opt/mssql -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest

# Iniciar contenedor de API
podman run -d --name hangul-api --network hangul-net -e "ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=hangul-db;User Id=sa;Password=admin123!;MultipleActiveResultSets=true;TrustServerCertificate=true" -e ASPNETCORE_ENVIRONMENT=Development -e "ASPNETCORE_URLS=http://0.0.0.0:8080" -p 5000:8080 hangul-api:dev

podman run -d --name hangul-api -p 5000:80 hangul-api:dev
```

Ver los logs de un contenedor

```bash
podman logs hangul-api
podman logs -f hangul-api
```

Hacer llamado al API con curl

```bash

curl http://localhost:5000/api/v1/health

curl http://localhost:5000/api/v1/jamos

curl http://localhost:5000/api/v1/jamos `
  -Method POST `
  -Headers @{ "Content-Type" = "application/json" } `
  -Body '{"character": "ㅏ", "type": "Vocal", "pronunciation": "/ah/", "characterRomaji": "a", "name": "아", "nameRomaji": "a" }'

```

# Configuración de recursos Azure

## Crear recursos en Azure

**Ejecutar en la terminal los siguientes comandos**

```bash
# Inicia sesión
az login

# Variables (cámbialas según tu caso)
$RG="hangul-rg"
$LOCATION="eastus"
$PLAN_NAME = "hangul-plan"
$APP_NAME = "hangul-api-lucy-j-dev"
$IMAGE_NAME = "hangul-api-dotnet9"
$ACR_NAME = "hangulacr"

RG=hangul-rg
LOCATION=eastus
PLAN_NAME=hangul-plan
APP_NAME=hangul-api-lucy-j-dev
IMAGE_NAME=hangul-api-dotnet9
ACR_NAME=hangulacr

# 1. Crear grupo de recursos
az group create --name $RG --location $LOCATION

# 2. Crear app service
az appservice plan create --name $PLAN_NAME --resource-group $RG --sku F1 --is-linux

# 3. Crea la app web
az webapp create --resource-group $RG --plan $PLAN_NAME --name $APP_NAME --runtime "DOTNETCORE:9.0" --output json

# 4. Obtener perfil de publicacion
az webapp deployment list-publishing-profiles --name $APP_NAME --resource-group $RG --output tsv > publish-profile.xml

# Variables
$SQL_SERVER_NAME = "hangul-sql-server"
$SQL_ADMIN = "sqladminuser"
$SQL_PASSWORD = "Admin123!"

az sql server create `
  --name $SQL_SERVER_NAME `
  --resource-group $RG `
  --location $LOCATION `
  --admin-user $SQL_ADMIN `
  --admin-password $SQL_PASSWORD

```

```bash
# Revisar logs de la aplicación
az webapp log tail --name hangul-api-lucy-j-dev --resource-group hangul-rg

# Comandos adicionales
az provider register --namespace Microsoft.ContainerRegistry
az provider show --namespace Microsoft.ContainerRegistry --query "registrationState"

# Mostrar la lista de recursos dentro de un resource group
az resource list --resource-group $RG --output table

# Mostrar log de una aplicación
az webapp log tail --name $APP_NAME --resource-group $RG
```

Este ultimo paso va a generar un archivo. Se debe copiar todo el contenido de ese archivo y crear un secret en GitHub para colocar ese contenido en el value del secrete con Key=AZURE_WEBAPP_PUBLISH_PROFILE
