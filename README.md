
# 🚀 Proyecto .NET 8 con PostgreSQL y Docker

Este proyecto utiliza **.NET 8** como framework principal, **PostgreSQL** como base de datos y **Docker** para la creación y gestión de contenedores. A continuación, encontrarás las instrucciones para configurar, ejecutar y entender este proyecto.

---

## 📋 **Tecnologías utilizadas**

- **.NET 8**: Framework para el desarrollo de aplicaciones robustas y modernas.  
- **PostgreSQL**: Sistema de gestión de bases de datos relacional.  
- **Docker**: Herramienta para crear contenedores de la aplicación y base de datos, simplificando la ejecución y despliegue.

---

## ⚙️ **Requisitos previos**

Antes de ejecutar el proyecto, asegúrate de tener instaladas las siguientes herramientas:

1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. [Docker](https://www.docker.com/)
---

## 🛠️ **Configuración inicial**

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/Byte-Arquitect/UserService.git
   cd Users_Service
   ```

2. **Crear el contenedor con Docker:**

   El proyecto incluye un archivo `docker-compose.yml` para levantar tanto la aplicación como la base de datos PostgreSQL.

   Ejecuta el siguiente comando en la raíz del proyecto:
   ```bash
   docker-compose up --build
   ```

   Esto realizará las siguientes tareas:
   - Creará el contenedor de PostgreSQL y configurará la base de datos.
   - Creará el contenedor para la aplicación .NET 8.

## 🚀 **Ejecución del proyecto**

Una vez que los contenedores estén levantados, puedes acceder a la aplicación:

1. **Instalar dependencias:**
   ```
     dotnet restore
   ```
2. **Correr proyecto:**
   ```
     dotnet run
   ```

## 📞 **Contacto**

Si tienes alguna duda o sugerencia, no dudes en contactarme:

- **Nombre:** Franko Ignacio Yusta Gonzalez  
- **Email:** [Franko.yusta@alumnos.ucn.cl](mailto:Franko.yusta@alumnos.ucn.cl)

---
