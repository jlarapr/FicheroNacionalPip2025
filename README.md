# Fichero Nacional Pip 2025

Sistema de gestión para el Fichero Nacional Pip, versión 2025.

## 📋 Descripción

Aplicación de escritorio moderna desarrollada en WPF que proporciona una interfaz intuitiva y eficiente para la gestión del Fichero Nacional Pip. El sistema está diseñado con una arquitectura en capas y utiliza las últimas tecnologías de .NET.

## 🚀 Características Principales

- Interfaz moderna con Material Design
- Soporte para temas claro/oscuro
- Sistema multilenguaje (Español/Inglés)
- Gestión de usuarios y roles
- Panel de administración
- Configuración personalizable
- Sistema de logging integrado

## 🛠️ Tecnologías

- **.NET 9.0** - Framework base
- **WPF** - Framework de interfaz de usuario
- **MVVM** - Patrón de arquitectura (CommunityToolkit.Mvvm)
- **Material Design** - Diseño de interfaz
- **Entity Framework Core** - ORM para acceso a datos
- **Microsoft.Extensions.DependencyInjection** - Inyección de dependencias
- **Microsoft.Extensions.Logging** - Sistema de logging

## 📁 Estructura del Proyecto

```
FicheroNacionalPip2025/
├── FicheroNacionalPip.Presentation/    # Capa de presentación (WPF)
│   ├── Views/                          # Interfaces de usuario
│   ├── ViewModels/                     # Lógica de presentación
│   ├── Models/                         # Modelos de vista
│   └── Services/                       # Servicios de la capa de presentación
├── FicheroNacionalPip.Business/        # Capa de lógica de negocio
└── FicheroNacionalPip.Data/           # Capa de acceso a datos
```

## 🔧 Requisitos del Sistema

- Windows 10/11
- .NET 9.0 Runtime
- SQL Server (para base de datos principal)
- SQLite (para configuraciones locales)

## ⚙️ Configuración y Despliegue

1. Clonar el repositorio
2. Restaurar paquetes NuGet
3. Configurar la cadena de conexión en la capa de datos
4. Compilar la solución
5. Ejecutar FicheroNacionalPip.Presentation

## 👥 Acceso al Sistema

**Credenciales por defecto:**
- Usuario: admin
- Contraseña: admin123
- Rol: Administrador

## 🔄 Versionado

- Versión actual: 0.0.1
- Fecha de versión: 2025.04.01

## 📝 Notas de Desarrollo

- La aplicación utiliza hot reload para desarrollo
- Incluye configuración específica para publicación como archivo único
- Soporta inyección de dependencias para mejor mantenibilidad
- Implementa logging para seguimiento de errores

## 🤝 Contribución

Para contribuir al proyecto:
1. Fork del repositorio
2. Crear una rama para la nueva característica
3. Commit de los cambios
4. Push a la rama
5. Crear un Pull Request

## 📄 Licencia

Este proyecto está bajo licencia privada y es propiedad de la organización.
