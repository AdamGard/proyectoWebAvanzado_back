# Docker - Proyecto Web Avanzado API

## Requisitos

- Docker Desktop
- Docker Compose
- Puertos libres:
  - API host: `5246`
  - API contenedor: `8080`
  - MariaDB Docker host: `3307`
  - MariaDB Docker contenedor: `3306`

## Archivos

- `Dockerfile`: imagen multi-stage de ASP.NET Core .NET 8.
- `.dockerignore`: excluye `bin`, `obj`, secretos y archivos locales.
- `docker-compose.host-db.yml`: ejecuta solo la API en Docker usando MariaDB instalado en Windows.
- `docker-compose.yml`: ejecuta API + MariaDB en Docker.
- `.env.example`: plantilla de variables.
- `docker/init/01-schema.sql`: esquema y datos semilla para MariaDB Docker.

## Crear `.env`

Copia `.env.example` a `.env` y cambia los valores:

```powershell
copy .env.example .env
```

Variables:

```env
MARIADB_ROOT_PASSWORD=CAMBIA_LA_CLAVE_ROOT
MARIADB_APP_PASSWORD=CAMBIA_LA_CLAVE_APP
HOST_DB_PASSWORD=CAMBIA_LA_CLAVE_DE_MARIADB_LOCAL
JWT_KEY=CAMBIA_ESTA_CLAVE_JWT_POR_UNA_MUY_LARGA_Y_ALEATORIA
```

`.env` está ignorado por Git. No subas credenciales reales.

## Construir Imagen

Desde `proyectoWebAvanzado_back/proyectoWebAvanzado`:

```powershell
docker build -t proyecto-web-api:latest .
```

## Modo 1: API en Docker con MariaDB de Windows

MariaDB debe estar activo en Windows en `127.0.0.1:3306`.

El contenedor usa `host.docker.internal`, porque `localhost` dentro del contenedor apunta al contenedor, no a Windows.

```powershell
docker compose -f docker-compose.host-db.yml up --build -d
docker compose -f docker-compose.host-db.yml logs -f api
docker compose -f docker-compose.host-db.yml down
```

API:

- Host: `http://localhost:5246`
- Swagger: `http://localhost:5246/swagger`

## Modo 2: API y MariaDB en Docker

```powershell
docker compose up --build -d
docker compose ps
docker compose logs -f api
docker compose logs -f mariadb
```

API:

- Host: `http://localhost:5246`
- Contenedor: `http://api:8080` solo dentro de Docker

MariaDB:

- Host: `127.0.0.1`
- Puerto: `3307`
- Usuario: `proyecto_app`
- Contraseña: valor de `MARIADB_APP_PASSWORD`
- Base: `ProyectoWebAvanzadoDB`

## Probar Login

```powershell
$body = @{
  email = "admin@proyecto.local"
  password = "12345"
} | ConvertTo-Json

Invoke-RestMethod `
  -Uri "http://localhost:5246/api/Auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body $body
```

El resultado debe incluir un `token`.

Para un endpoint protegido:

```powershell
$login = Invoke-RestMethod -Uri "http://localhost:5246/api/Auth/login" -Method Post -ContentType "application/json" -Body $body
$headers = @{ Authorization = "Bearer $($login.token)" }
Invoke-RestMethod -Uri "http://localhost:5246/api/Actividad" -Headers $headers
```

## Detener y Reconstruir

Detener sin borrar datos:

```powershell
docker compose down
```

Reconstruir después de cambiar código:

```powershell
docker compose up --build -d
```

Reiniciar contenedores conservando datos:

```powershell
docker compose restart
```

## Reiniciar Base de Datos Docker

Esto elimina permanentemente el volumen de MariaDB:

```powershell
docker compose down -v
docker compose up --build -d
```

Los scripts en `docker/init` solo se ejecutan cuando el volumen está vacío.

## Diferencias de Red

- `localhost`: desde tu navegador apunta a Windows; desde un contenedor apunta al mismo contenedor.
- `host.docker.internal`: desde un contenedor apunta al host Windows.
- `mariadb`: nombre DNS interno del servicio MariaDB dentro de Docker Compose.

El frontend Angular debe seguir usando:

```ts
apiUrl: 'http://localhost:5246/api'
```

No uses `http://api:8080` en Angular; el navegador no resuelve nombres internos de Docker.
