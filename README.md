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
podman run -d --name hangul-api --network hangul-net -e "ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=hangul-db;User Id=sa;Password=admin123!;MultipleActiveResultSets=true;TrustServerCertificate=true" -e "ASPNETCORE_URLS=http://0.0.0.0:8080" -p 5000:8080 hangul-api:dev

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
$PLAN = "hangul-plan"
$APP = "hangul-api"
$IMAGE = "tuusuario/hangul-api:latest"

# 1. Crear grupo de recursos
az group create `
    --name $RG `
    --location $LOCATION

# 2. Crear app service
az appservice plan create `
  --name $PLAN `
  --resource-group $RG `
  --sku FREE `
  --is-linux

# 3. Crear web app
az webapp create `
  --name $APP `
  --resource-group $RG `
  --plan $PLAN `
  --deployment-container-image-name $IMAGE

# 4 Configurar app
az webapp config appsettings set `
  --name $APP `
  --resource-group $RG `
  --settings WEBSITES_PORT=80
```

## Crear Azure Container App (una sola vez)

**Ejecutar en la terminal los siguientes comandos**

```bash
# Habilita extensiones necesarias
az extension add --name containerapp --upgrade

# Crear la Container App (vacía por ahora)
az containerapp create \
  --name $APP_NAME \
  --resource-group $RG \
  --environment "${APP_NAME}-env" \
  --image $ACR_NAME.azurecr.io/$IMAGE_NAME:latest \
  --registry-server $ACR_NAME.azurecr.io \
  --ingress external \
  --target-port 80 \
  --min-replicas 0 \
  --max-replicas 1
```

## Agregar Secrets en GitHub

Ve a tu repositorio → Settings → Secrets → Actions y agrega:
| Secret name | Valor |
| ------------------------- | ---------------------------------------- |
| `ACR_USERNAME` | usuario del registro ACR |
| `ACR_PASSWORD` | contraseña del registro ACR |
| `ACR_LOGIN_SERVER` | ejemplo: `oscarregistry123.azurecr.io` |
| `AZURE_CONTAINERAPP_NAME` | nombre de tu container app (`oscar-api`) |
| `AZURE_RESOURCE_GROUP` | nombre del grupo de recursos (`rg-api`) |
