# 🏥 Smart-Medical IA — Frontend (Administrador/Médico)

Módulo frontend del sistema **Smart-Medical IA**, una plataforma de análisis clínico con inteligencia artificial. Este frontend consume una API REST desarrollada en .NET y proporciona interfaces para administradores, médicos, enfermeras y usuarios de consulta.

---

## 📋 Tabla de Contenidos

- [Funcionalidades incluidas](#funcionalidades-incluidas)
- [Tecnologías utilizadas](#tecnologias-utilizadas)
- [Requisitos previos](#requisitos-previos)
- [Clonar el repositorio](#clonar-el-repositorio)
- [Configurar la base de datos](#configurar-la-base-de-datos)
- [Configurar y ejecutar el Backend (.NET)](#configurar-y-ejecutar-el-backend-net)
- [Configurar y ejecutar el Frontend (Vue 3)](#configurar-y-ejecutar-el-frontend-vue-3)
- [Probar el sistema](#probar-el-sistema)
- [Estructura del proyecto](#estructura-del-proyecto)

---

<a id="funcionalidades-incluidas"></a>
## ✨ Funcionalidades incluidas

### Módulo de Autenticación
- Inicio de sesión con nombre de usuario y contraseña
- Registro de nuevos usuarios
- Recuperación y restablecimiento de contraseña
- Protección de rutas por rol (Administrator, Doctor, Nurse, ConsultationUser)
- Sesión persistente via JWT almacenado en localStorage

### Módulo de Dashboard
- Dashboard del médico con resumen de actividad
- Dashboard del administrador con estadísticas generales (totales de pacientes, citas, usuarios, análisis IA)
- Tarjetas de estadísticas reutilizables

### Módulo de Pacientes
- Listado de pacientes conectado a la API con filtros de búsqueda
- Creación, edición y eliminación de pacientes
- Visualización de expediente clínico completo con datos del backend
- Historial de citas agendadas del paciente

### Módulo de Citas
- Listado de citas con filtros por estado y fecha
- Creación de citas con selección de paciente
- Vista de detalle de cita con pestañas:
  - Información general
  - Síntomas (registro y visualización)
  - Signos vitales (registro y visualización)
  - Análisis IA (diagnóstico asistido)
- Alertas y recomendaciones generadas por IA

### Módulo de Registros Clínicos (Medical Records)
- Creación de registros clínicos asociados a citas
- Diagnóstico inicial, notas, antecedentes y observaciones
- Visualización en el expediente del paciente

### Módulo de Signos Vitales
- Registro de presión arterial, frecuencia cardíaca, temperatura, saturación de oxígeno, peso y talla
- Asociación a citas médicas

### Módulo de Síntomas
- Registro de síntomas con severidad y duración
- Asociación a citas médicas

### Módulo de Análisis IA
- Análisis de diagnóstico asistido por inteligencia artificial
- Resultados y recomendaciones generadas automáticamente

### Módulo de Alertas y Recomendaciones
- Alertas generadas por el sistema de IA
- Recomendaciones personalizadas por paciente
- Visualización en el detalle de la cita

### Módulo de Documentos Médicos
- Subida y gestión de documentos clínicos (imágenes, PDFs)
- Asociación a pacientes y citas

### Módulo de Reportes
- Generación de reportes de pacientes
- Descarga en formato PDF

### Módulo de Administración
- Gestión de usuarios (CRUD completo)
- Activación/desactivación de cuentas
- Asignación de roles (Administrator, Doctor, Nurse, ConsultationUser)
- Registro de auditoría con filtros por usuario, acción y rango de fechas

### Módulo de Perfil
- Visualización del perfil del usuario autenticado
- Cambio de contraseña

---

<a id="tecnologias-utilizadas"></a>
## 🛠 Tecnologías utilizadas

| Capa       | Tecnología                              |
|------------|-----------------------------------------|
| Framework  | Vue 3 (Composition API, `<script setup>`) |
| Router     | Vue Router 5                            |
| Estado     | Reactive API (store plano, sin Pinia)   |
| HTTP       | Axios 1.18                              |
| CSS        | Bootstrap 5.3.8                         |
| Fuentes    | Google Fonts (Poppins)                  |
| Build tool | Vite 8                                  |
| Backend    | .NET (Web API, C#)                      |
| Base datos | PostgreSQL                              |

---

<a id="requisitos-previos"></a>
## 📦 Requisitos previos

| Herramienta       | Versión mínima                     |
|-------------------|------------------------------------|
| Node.js           | ^20.19.0 \|\| >=22.12.0           |
| npm               | 10 o superior                      |
| .NET SDK          | 8.0 o superior                     |
| PostgreSQL        | 14 o superior                      |

Verifica tus versiones:

```bash
node --version
npm --version
dotnet --version
psql --version
```

---

<a id="clonar-el-repositorio"></a>
## 📥 Clonar el repositorio

Clona la rama `frontend-module-complete` del repositorio:

```bash
git clone -b frontend-module-complete https://github.com/DarieC18/proyecto-final-analisis-medico-ia-frontend.git
cd proyecto-final-analisis-medico-ia-frontend
```

Esto descargará tanto el frontend (`prototipo-medico/`) como el backend (`MedAnalyzer/`).

---

<a id="configurar-la-base-de-datos"></a>
## 🗄 Configurar la base de datos

### 1. Crear la base de datos en PostgreSQL

Accede a PostgreSQL y crea la base de datos:

```bash
psql -U postgres
```

```sql
CREATE DATABASE "MedAnalyzerDb";
\q
```

### 2. Configurar la cadena de conexión

Edita el archivo `MedAnalyzer/MedAnalyzer.Api/appsettings.json` y ajusta la cadena de conexión si tu usuario o contraseña son diferentes:

```json
"ConnectionStrings": {
  "ConnectionDb": "Host=localhost;Port=5432;Database=MedAnalyzerDb;Username=postgres;Password=1234"
}
```

### 3. Seed de datos (opcional)

Si deseas datos de prueba, puedes ejecutar el script `prototipo-medico/seed.sql`:

```bash
psql -U postgres -d MedAnalyzerDb -f prototipo-medico/seed.sql
```

> **Nota:** El backend también tiene un `RunSeedAsync()` en `Program.cs` que ejecuta un seed automático al iniciar, incluyendo un usuario administrador por defecto.

---

<a id="configurar-y-ejecutar-el-backend-net"></a>
## ⚙️ Configurar y ejecutar el Backend (.NET)

### 1. Configurar la cadena de conexión

Edita `MedAnalyzer/MedAnalyzer.Api/appsettings.json` y ajusta la conexión a PostgreSQL:
```json
"ConnectionStrings": {
  "ConnectionDb": "Host=localhost;Port=5432;Database=MedAnalyzerDb;Username=postgres;Password=1234"
}
```

### 2. Restaurar paquetes NuGet

```bash
cd MedAnalyzer
dotnet restore
```

### 3. Ejecutar migraciones (la base de datos se crea automáticamente al iniciar)

El proyecto usa Entity Framework Core con migraciones automáticas. Al ejecutar el backend por primera vez, las tablas se crearán solas.

### 4. Iniciar el servidor backend

```bash
cd MedAnalyzer.Api
dotnet run --launch-profile http
```

El backend estará disponible en: **http://localhost:5197**

- Swagger UI: http://localhost:5197/swagger
- Endpoints API: http://localhost:5197/api/v1/Auth

> Para detener el servidor: `Ctrl + C`

---

<a id="configurar-y-ejecutar-el-frontend-vue-3"></a>
## 🖥 Configurar y ejecutar el Frontend (Vue 3)

### 1. Instalar dependencias

```bash
cd prototipo-medico
npm install
```

### 2. Iniciar el servidor de desarrollo

```bash
npm run dev
```

El frontend estará disponible en: **http://localhost:5173**

El servidor de Vite tiene un proxy configurado (`vite.config.js`) que redirige las peticiones `/api` al backend en `http://localhost:5197`, por lo que no hay necesidad de configurar CORS manualmente.

### 3. Compilar para producción

```bash
npm run build
```

Los archivos estáticos se generarán en la carpeta `dist/`.

---

<a id="probar-el-sistema"></a>
## 🧪 Probar el sistema

### Credenciales por defecto (seed automático del backend)

| Rol              | Usuario           | Contraseña      |
|------------------|-------------------|-----------------|
| Administrator    | Administrador     | 123Pas$$word!   |
| Doctor           | Doctor            | 123Pas$$word!   |
| ConsultationUser | ConsultationUser  | 123Pas$$word!   |

### Flujo de prueba recomendado

1. **Iniciar backend** — `dotnet run --launch-profile http` (puerto 5197)
2. **Iniciar frontend** — `npm run dev` (puerto 5173)
3. **Abrir** http://localhost:5173
4. **Iniciar sesión** con las credenciales de administrador
5. **Explorar** el dashboard de administración
6. **Probar** las secciones:
   - Gestión de usuarios (crear, editar, desactivar)
   - Registro de auditoría
   - Perfil de usuario
7. **Probar recuperación de contraseña** desde la pantalla de login
8. **Probar roles:** crea un usuario con rol Doctor, cierra sesión e inicia con ese usuario para ver el dashboard médico

### Verificar la conexión backend

El frontend se comunica con el backend a través del proxy de Vite. Puedes verificar que la comunicación funciona:

- Al iniciar sesión exitosamente, el dashboard se carga con datos del backend
- En la consola del navegador (F12 → Network) las peticiones a `/api/*` deben devolver `200 OK`

---

<a id="estructura-del-proyecto"></a>
## 📁 Estructura del proyecto

```
proyecto-final-analisis-medico-ia-frontend/
├── prototipo-medico/                    # Frontend Vue 3
│   ├── src/
│   │   ├── api/                         # Servicios API (axios)
│   │   │   ├── axios.js                 # Configuración de Axios + interceptores
│   │   │   ├── auth.js                  # Auth endpoints
│   │   │   ├── account.js               # CRUD de usuarios
│   │   │   ├── dashboard.js             # Estadísticas del dashboard
│   │   │   ├── auditLog.js              # Registro de auditoría
│   │   │   ├── patients.js              # Pacientes
│   │   │   ├── appointments.js          # Citas
│   │   │   ├── medicalRecords.js        # Registros clínicos
│   │   │   ├── vitalSigns.js            # Signos vitales
│   │   │   ├── symptoms.js              # Síntomas
│   │   │   ├── aiAnalysis.js            # Análisis IA
│   │   │   ├── alerts.js                # Alertas
│   │   │   └── recommendations.js       # Recomendaciones
│   │   ├── components/                  # Componentes reutilizables
│   │   │   ├── Navbar.vue               # Barra de navegación
│   │   │   ├── StatCard.vue             # Tarjeta de estadística
│   │   │   ├── StatusBadge.vue          # Badge de estado
│   │   │   └── ConfirmDialog.vue        # Diálogo de confirmación
│   │   ├── router/
│   │   │   └── index.js                 # Configuración de rutas + guard
│   │   ├── stores/
│   │   │   └── auth.js                  # Store de autenticación
│   │   ├── views/                       # Páginas/vistas
│   │   │   ├── LoginView.vue
│   │   │   ├── RegisterView.vue
│   │   │   ├── ForgotPasswordView.vue
│   │   │   ├── ResetPasswordView.vue
│   │   │   ├── DoctorDashboardView.vue
│   │   │   ├── AdminDashboardView.vue
│   │   │   ├── PatientListView.vue
│   │   │   ├── AppointmentsView.vue
│   │   │   ├── AppointmentDetailView.vue
│   │   │   ├── UsersView.vue
│   │   │   ├── AuditLogView.vue
│   │   │   └── ProfileView.vue
│   │   ├── App.vue
│   │   └── main.js
│   ├── seed.sql                         # Datos de prueba
│   ├── vite.config.js                   # Proxy hacia backend
│   └── package.json
│
├── MedAnalyzer/                         # Backend .NET
│   ├── MedAnalyzer.Api/                 # API (controladores, Program.cs)
│   │   └── Controllers/
│   │       ├── AuthController.cs
│   │       ├── AccountController.cs
│   │       ├── AuditLogController.cs
│   │       ├── PatientController.cs
│   │       ├── AppointmentController.cs
│   │       ├── MedicalRecordController.cs
│   │       ├── VitalSignController.cs
│   │       ├── SymptomController.cs
│   │       ├── AiAnalysisController.cs
│   │       ├── MedicalDocumentController.cs
│   │       ├── AlertController.cs
│   │       ├── RecommendationController.cs
│   │       ├── ReportController.cs
│   │       ├── DashboardController.cs
│   │       └── AdminDashboardController.cs
│   ├── MedAnalyzer.Core.Application/    # Lógica de aplicación (DTOs, interfaces, servicios)
│   │   ├── Base/
│   │   ├── Dto/
│   │   ├── Interfaces/
│   │   ├── Mappers/
│   │   └── Services/
│   ├── MedAnalyzer.Core.Domain/         # Entidades del dominio
│   ├── MedAnalyzer.Infraestructure.Identity/  # Identity (usuarios, JWT)
│   ├── MedAnalyzer.Infraestructure.Persistences/ # EF Core, DbContext
│   └── MedAnalyzer.Infraestructure.Shared/      # Servicios compartidos (email, archivos)
```

---

## ⚙️ Configuración adicional

### Puerto del backend

Si cambias el puerto del backend (`launchSettings.json`), actualiza también el proxy en `prototipo-medico/vite.config.js`:

```js
server: {
  proxy: {
    '/api': {
      target: 'http://localhost:NUEVO_PUERTO',
      changeOrigin: true
    }
  }
}
```

### FrontendBaseUrl (para recuperación de contraseña)

En `MedAnalyzer/MedAnalyzer.Api/appsettings.json`, ajusta:

```json
"AppSettings": {
  "FrontendBaseUrl": "http://localhost:5173"
}
```

Esto asegura que el enlace de restablecimiento de contraseña enviado por correo apunte al frontend correcto.

---

## 📌 Notas importantes

- Todas las vistas del frontend están conectadas a la API REST del backend.
- El proyecto no usa variables de entorno (`.env`); toda la configuración está en `appsettings.json` y `vite.config.js`.
- El interceptor de Axios redirige automáticamente al login si recibe un `401 Unauthorized`.
