# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY HotelManagement.sln .
COPY HotelManagement.Api/HotelManagement.Api.csproj HotelManagement.Api/
COPY HotelManagement.Application/HotelManagement.Application.csproj HotelManagement.Application/
COPY HotelManagement.Domain/HotelManagement.Domain.csproj HotelManagement.Domain/
COPY HotelManagement.Infrastructure/HotelManagement.Infrastructure.csproj HotelManagement.Infrastructure/

# Restaurar dependencias
RUN dotnet restore HotelManagement.sln

# Copiar el resto de los archivos
COPY . .

# Publicar el proyecto
WORKDIR /src/HotelManagement.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 5024
ENTRYPOINT ["dotnet", "HotelManagement.Api.dll"]