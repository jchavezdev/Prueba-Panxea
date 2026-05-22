# Sistema de Gestión de Nómina - WebPanxea

Este proyecto es una aplicación web básica para la gestión y cálculo de nómina quincenal. Cuenta con una API REST desarrollada en .NET y una interfaz gráfica sencilla en HTML y JavaScript para consultar la información en tiempo real.

---

## Estructura del Proyecto y Archivos

Para facilitar la ubicación de la lógica de la aplicación, los archivos principales se distribuyen de la siguiente manera:

*   `Program.cs`: Configuración inicial de la aplicación, activación de servicios, controladores y políticas de CORS.
*   `nominaService.cs`: Contiene la definición del modelo `Empleado`, la interfaz `INominaService` y la clase `NominaService` donde se calcula el IMSS e ISR.
*   `Controllers/EmpleadoController.cs`: Controlador que expone los endpoints de la API, maneja las solicitudes HTTP y simula la base de datos en memoria (`BaseDatosEmpleados`).
*   `index.html`: Interfaz de usuario que consume la API mediante JavaScript para mostrar las tablas y desgloses de nómina.

---

## Características del proyecto

### Estructura de datos
Para cumplir con los requerimientos del ejercicio, el diseño contempla las entidades de Departamentos y Empleados con sus respectivas relaciones, tipos de datos e identificadores únicos (como el formato EMP-0000). Para facilitar la ejecución inmediata sin configuraciones adicionales de bases de datos, los datos de prueba se manejan en memoria dentro del controlador.

### Backend (API REST)
*   **Inyección de dependencias:** Se utiliza para conectar la interfaz INominaService con la lógica de negocio.
*   **Validaciones:** El endpoint de registro valida que el salario ingresado sea mayor a 0, devolviendo un código de error correspondiente si no se cumple.
*   **Códigos de estado HTTP:** El servidor responde con estados estándar (200 para éxito, 201 para creación, 400 para errores de validación y 404 si el recurso no existe).

### Frontend
*   Consumo de la API mediante Fetch en JavaScript de forma asíncrona.
*   Carga automática de empleados activos ordenados por su antigüedad de mayor a menor.
*   Cálculo visual e instantáneo del desglose de la nómina al presionar el botón de cada empleado.

---

## Reglas para el cálculo de nómina

La aplicación procesa los salarios de forma quincenal aplicando los siguientes criterios:

*   **Salario Quincenal:** Salario Mensual / 2
*   **Deducción IMSS:** Salario Quincenal * 3%
*   **Deducción ISR:** Se calcula sobre el salario quincenal usando los siguientes rangos:
    *   Hasta $7,500: Tasa del 9%
    *   De $7,501 a $15,000: Tasa del 16%
    *   Más de $15,000: Tasa del 25%
*   **Salario Neto:** Salario Quincenal - IMSS - ISR

---

## Endpoints de la API

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| GET | /api/empleados | Devuelve la lista de empleados activos con su antigüedad (Consulta A). |
| GET | /api/empleados/nomina-completa | Genera la nómina detallada de todos los trabajadores (Consulta B). |
| POST | /api/empleados | Registra un nuevo empleado (con validación de salario). |
| GET | /api/empleados/{id}/nomina | Retorna el JSON con el desglose del empleado especificado por ID. |

### Ejemplo de respuesta de nómina
{
  "numeroEmpleado": "EMP-0001",
  "nombre": "Ana López",
  "salarioQuincenal": 6000.00,
  "deduccionIMSS": 180.00,
  "deduccionISR": 540.00,
  "salarioNeto": 5280.00
}

### Cómo ejecutar la aplicación
1. Iniciar el Backend
Abre una terminal en la carpeta raíz del proyecto de .NET.

Ejecuta el comando:

dotnet run
Revisa el puerto local en el que se está ejecutando (por ejemplo, https://localhost:7214). Si tu puerto es diferente, actualiza la variable URL_API en el archivo index.html.

2. Abrir el Frontend
Ve al directorio donde se encuentra el archivo index.html.

Abre el archivo en el navegador web.

La tabla mostrará los empleados cargados en memoria y podrás interactuar con el botón "Ver Nómina" para ver los desgloses quincenales.
