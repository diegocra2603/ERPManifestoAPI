# ERPManifestoAPI - .NET 10

API de gestión de restaurantes basada en **Domain-Driven Design (DDD)** con .NET 10 y arquitectura en capas.

## Estructura del proyecto

```
├── Core/                    # Núcleo del dominio
│   ├── Domain/              # Entidades, value objects, agregados
│   └── Application/        # Casos de uso, interfaces, DTOs
├── Infrastructure/          # Implementaciones externas
│   ├── Persistence/         # Acceso a datos (EF Core, etc.)
│   └── Services/            # Servicios de infraestructura
├── Presentation/            # Puntos de entrada
│   └── WebApi/              # API REST (ASP.NET Core)
└── ERPManifestoAPI.sln
```

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Ejecutar el proyecto

### Desde la raíz de la solución

```bash
dotnet run --project Presentation/WebApi/WebApi.csproj
```

### Con F5 / Ctrl+F5 en Cursor o VS Code

Abre la carpeta de la solución y pulsa **F5** (con depurador) o **Ctrl+F5** (sin depurador). Por defecto se inicia el proyecto **WebApi**.

La API se sirve en:

- HTTP: `http://localhost:5004`
- HTTPS: `https://localhost:7251`

### Usuario inicial

Para acceder a la aplicación puedes usar el usuario creado por defecto:

- **Email:** `admin@example.com`
- **Contraseña:** `Admin123@`

## Build

```bash
dotnet build
```

## Licencia

Uso libre según las necesidades del proyecto.
