# MBUNAY_S6

Aplicación .NET MAUI que consume el web service de estudiantes (MySQL / XAMPP).

## CRUD completo

**Modificación, ingreso y eliminación** de estudiantes.

| Acción | Pantalla |
|--------|----------|
| Listado y consulta | `Estudiante` |
| Ingreso | `vNuevoEstudiante` |
| Modificación y eliminación | `vEditarEstudiante` (desde fila seleccionada en el grid) |

Servicio: `EstudianteApiService` — GET, POST, PUT, DELETE contra `estudiante.php`.
