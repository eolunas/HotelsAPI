# Hotel Management Solution

Esta es una solución completa para la gestión de un sistema de reservas de hotel, desarrollada con **.NET Core**, utilizando principios de **Arquitectura Limpia**, **SOLID** y **DDD**. La solución incluye API RESTful con autenticación JWT, migración de base de datos con EF Core y despliegue en **Docker** y **AWS ECS**.

---

## **Tabla de Contenidos**

- [Características Principales](#características-principales)
- [Requisitos Previos](#requisitos-previos)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Configuración Local](#configuración-local)
- [Dockerización](#dockerización)
- [Despliegue en AWS ECS](#despliegue-en-aws-ecs)
- [Uso de la API](#uso-de-la-api)
- [Contribuciones](#contribuciones)
- [Licencia](#licencia)

---

## **Características Principales**

1. **Gestión de Reservas**:
   - Listar todas las reservas realizadas.
   - Ver detalles de una reserva específica, incluyendo datos del huésped, habitación y contacto de emergencia.

2. **Autenticación JWT**:
   - Solo administradores tienen acceso a endpoints específicos.
   - Configuración de roles y autenticación segura.

3. **Arquitectura Limpia**:
   - Separación de responsabilidades en capas: **API**, **Aplicación**, **Dominio** e **Infraestructura**.

4. **Base de Datos SQL Server**:
   - Creación y migración con EF Core (**Code-First**).

5. **Dockerización y Despliegue en AWS ECS**:
   - Contenedores para la API y SQL Server.
   - Imágenes gestionadas con **Amazon ECR**.

---

## **Requisitos Previos**

### Software Necesario:

1. **.NET SDK 8.0 o superior**
2. **Docker**
3. **Docker Compose**
4. **SQL Server**
5. **AWS CLI**
6. **AWS Elastic Beanstalk CLI (opcional)**

### Configuración Inicial:

- Crea un archivo `appsettings.json` en el proyecto `HotelManagement.Api` con el siguiente contenido:

```json
{
  "Jwt": {
    "Key": "YourJwtSecretKey",
    "Issuer": "YourIssuer",
    "Audience": "YourAudience"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver,1433;Database=HotelManagementDB;User Id=sa;Password=YourPassword123;"
  }
}
```

---

## **Estructura del Proyecto**

```
HotelManagement
├── HotelManagement.Api                # Capa de presentación (API RESTful)
├── HotelManagement.Application        # Capa de lógica de negocio
├── HotelManagement.Domain             # Entidades y reglas de dominio
├── HotelManagement.Infrastructure     # Configuración de base de datos y repositorios
├── Dockerfile                         # Archivo Docker para la API
├── docker-compose.yml                 # Configuración para Docker Compose
└── README.md                          # Documentación del proyecto
```

---

## **Configuración Local**

1. **Clonar el Repositorio**:
   ```bash
   git clone <url-del-repositorio>
   cd HotelManagement
   ```

2. **Aplicar Migraciones de Base de Datos**:
   ```bash
   dotnet ef database update --project HotelManagement.Infrastructure
   ```

3. **Ejecutar Localmente**:
   ```bash
   dotnet run --project HotelManagement.Api
   ```
   La API estará disponible en `http://localhost:5024`.

---

## **Dockerización**

1. **Construir y Ejecutar con Docker Compose**:
   ```bash
   docker-compose up --build
   ```
   - La API estará disponible en: `http://localhost:5024`
   - SQL Server estará disponible en el puerto `1433`.

2. **Docker Compose File**:

   ```yaml
   version: '3.8'
   services:
     sqlserver:
       image: mcr.microsoft.com/mssql/server:2022-latest
       container_name: sqlserver
       environment:
         - ACCEPT_EULA=Y
         - SA_PASSWORD=YourPassword123
       ports:
         - "1433:1433"

     api:
       build:
         context: .
         dockerfile: Dockerfile
       container_name: hotelmanagement-api
       ports:
         - "5024:5024"
       environment:
         - ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=HotelManagementDB;User Id=sa;Password=YourPassword123;
       depends_on:
         - sqlserver
   ```

---

## **Despliegue en AWS ECS**

1. **Subir las Imágenes a ECR**:
   - Etiqueta las imágenes Docker:
     ```bash
     docker tag hotelmanagement-api:latest <aws_account_id>.dkr.ecr.us-east-1.amazonaws.com/hotelmanagement-api:latest
     docker tag mcr.microsoft.com/mssql/server:2022-latest <aws_account_id>.dkr.ecr.us-east-1.amazonaws.com/sqlserver:2022-latest
     ```
   - Sube las imágenes a ECR:
     ```bash
     docker push <aws_account_id>.dkr.ecr.us-east-1.amazonaws.com/hotelmanagement-api:latest
     docker push <aws_account_id>.dkr.ecr.us-east-1.amazonaws.com/sqlserver:2022-latest
     ```

2. **Configurar un Clúster en ECS**:
   - Crea un clúster en la consola de **Amazon ECS**.
   - Configura las definiciones de tarea con las imágenes subidas.
   - Crea un servicio para ejecutar las tareas en Fargate.

---

## **Uso de la API**

### Endpoints Principales:

1. **Listar Reservas**
   ```http
   GET /api/Reservations
   ```

2. **Ver Detalles de una Reserva**
   ```http
   GET /api/Reservations/{id}
   ```

3. **Asignar Habitaciones a un Hotel**
   ```http
   POST /api/Hotels/assign-rooms
   Body:
   {
       "hotelId": 1,
       "roomIds": [101, 102, 103]
   }
   ```

4. **Buscar Hoteles**
   ```http
   POST /api/Hotels/search
   Body:
   {
       "location": "Bogotá",
       "priceRange": {
           "min": 100,
           "max": 500
       }
   }
   ```

---

## **Contribuciones**

1. Crea un fork del repositorio.
2. Realiza tus cambios en una rama nueva.
3. Envía un pull request para revisión.

---

## **Licencia**

Este proyecto está bajo la licencia MIT. Puedes consultarla en el archivo `LICENSE` del repositorio.
