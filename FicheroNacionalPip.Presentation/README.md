# Fichero Nacional Pip

Aplicación WPF moderna con login, configuración, temas e idioma.

## Tecnologías

- .NET 8
- WPF + MVVM
- Material Design in XAML
- Entity Framework Core (SQL Server + SQLite)
- Soporte para tema claro/oscuro
- Multilenguaje (Español/Inglés)

## Estructura

- `Data` → EF Core y entidades
- `Business` → Lógica de servicios
- `Presentation` → WPF UI
- `Tests` → Pruebas unitarias

## Usuario por defecto

```
Username: admin
Password: admin123
Rol: Admin
```

## Configuración

- SQL Server: cambiar en `AppDbContext.cs`
- SQLite: se usa para guardar tema/idioma local

## Ejecución

1. Abrir la solución en JetBrains Rider o Visual Studio
2. Restaurar paquetes NuGet
3. Ejecutar el proyecto `FicheroNacionalPip.Presentation`

## Pruebas

```bash
dotnet test FicheroNacionalPip.Tests
```
