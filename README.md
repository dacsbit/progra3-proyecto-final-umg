# progra3-proyecto-final-umg

API desarrollada en ASP.NET Core 7.0 para la gestión de usuarios, clientes, tarjetas de crédito y transacciones, utilizando estructuras de datos en memoria (Árbol AVL, listas enlazadas, Tabla Hash, Pilas, Colas y Árbol ABB). El sistema cuenta con autenticación JWT, documentación Swagger y generación de reportes en PDF.

---

## ✅ Requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 / Visual Studio Code
- NuGet (se restaura automáticamente al abrir el proyecto)

---

## 📦 Paquetes NuGet utilizados

- `Swashbuckle.AspNetCore` – Generación de documentación Swagger.
- `System.IdentityModel.Tokens.Jwt` – Generación y validación de tokens JWT.
- `itext7` – Creación de documentos PDF.
- `itext7.bouncy-castle-adapter` – Creación de documentos PDF.
- `Newtonsoft.Json` – Framework para trabajar con JSON en .NET.
- `Microsoft.AspNetCore.Authentication.JwtBearer` – Middleware que le permite a la app recibir tokens Bearer.

---

## 🚀 Cómo ejecutar el proyecto

1. Clona el repositorio (acceso previo requerido):

   ```bash
   git clone https://github.com/dacsbit/progra3-proyecto-final-umg.git
   ```

2. Abre el proyecto en Visual Studio o VS Code.

3. Restaura los paquetes:

   ```bash
   dotnet restore
   ```

4. Ejecuta la API:

   ```bash
   dotnet run
   ```

5. Accede a la documentación interactiva Swagger:

   ```
   https://localhost:7115/swagger
   ```

---

## 🧪 Flujo de uso del proyecto (paso a paso)

### 1. 📂 **Carga inicial de datos**

- Endpoint: `POST /api/Cliente/crear`
- Body requerido: el contenido del archivo JSON `clientes_con_usuarios_y_tarjetas.json` ubicado en la carpeta `schemas`.

Este archivo contiene la estructura de clientes, usuarios, tarjetas y transacciones necesarias para empezar a usar el sistema.

---

### 2. 🔐 **Autenticación (simular inicio de sesión)**

- Endpoint: `POST /api/Usuario/login`
- Body esperado:

```json
{
  "username": "string",
  "password": "string"
}
```

- En la respuesta, encontrarás una propiedad `"Data"` que contiene el **token JWT** que deberás usar para autenticar las siguientes solicitudes.

---

### 3. ✅ **Autenticarse en Swagger con el token**

1. Copia el token retornado en el login (campo `"Data"`).
2. Ve a la parte superior derecha de Swagger UI y haz clic en el botón **"Authorize"**.
3. Pega el token, el prefijo `Bearer` se colocara automaticamente en los headers de la peticion:
   ```
   Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```
4. Haz clic en **Authorize** y luego en **Close**.

Desde ese momento, podrás acceder a todos los endpoints protegidos con `[Authorize]`.

---

## 📂 Estructura del proyecto

- `/Controllers` – Endpoints de la API REST.
- `/Models/Domain` – Entidades principales (Cliente, Usuario, Tarjeta, etc.).
- `/Models/DTO` – Clases de entrada y salida para cada operación.
- `/Models/Estructuras` – Clases de estructuras para el manejo y almacenado de datos en memoria.
- `/Models/Interfaces` – Interfaces utilizadas para ayudar a las estructuras con la comparativa de datos genericos.
- `/Models/Responses` – Clases de respuesta usadas como el objeto que se devuelve al finalizar la operacion de los endpoints.
- `/Services` – Lógica de negocio, seguridad y estructuras en memoria.

---

## 🛡️ Seguridad

Este proyecto utiliza autenticación basada en **JWT (JSON Web Tokens)**.  
Todos los endpoints sensibles requieren autorización mediante un token válido.

---

## 📄 Licencia

Proyecto desarrollado como entrega final para la materia de **Programación 3**.  
Puedes reutilizarlo con fines educativos o personales.
