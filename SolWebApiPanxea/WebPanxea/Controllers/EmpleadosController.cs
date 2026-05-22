using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebPanxea.Controllers
{
    [ApiController]
    [Route("api/empleados")]
    public class EmpleadosController : ControllerBase
    {
        private readonly INominaService _nominaService;

        // 1.2 DATOS DE PRUEBA REQUERIDOS (En memoria con fechas reales para la antigüedad)
        private static readonly List<Empleado> BaseDatosEmpleados = new()
        {
            new Empleado { Id = 1, NumeroEmpleado = "EMP-0001", Nombre = "Ana López", SalarioMensual = 12000, DepartamentoId = 1, FechaIngreso = new DateTime(2024, 1, 15) },
            new Empleado { Id = 2, NumeroEmpleado = "EMP-0002", Nombre = "Carlos Gómez", SalarioMensual = 20000, DepartamentoId = 1, FechaIngreso = new DateTime(2025, 6, 1) },
            new Empleado { Id = 3, NumeroEmpleado = "EMP-0003", Nombre = "Patricia Ruiz", SalarioMensual = 36000, DepartamentoId = 2, FechaIngreso = new DateTime(2026, 2, 20) }
        };

        public EmpleadosController(INominaService nominaService)
        {
            _nominaService = nominaService;
        }

        // Consulta A: Empleados activos ordenados por antigüedad
        [HttpGet]
        public IActionResult ObtenerActivos()
        {
            var listaResultado = new List<EmpleadoAuxiliar>(); // <-- CORREGIDO AQUÍ
            var hoy = DateTime.Today;

            foreach (var e in BaseDatosEmpleados)
            {
                if (e.Activo)
                {
                    // Cálculo de meses de antigüedad
                    int meses = ((hoy.Year - e.FechaIngreso.Year) * 12) + hoy.Month - e.FechaIngreso.Month;
                    string deptoNombre = e.DepartamentoId == 1 ? "Sistemas" : "Recursos Humanos";

                    listaResultado.Add(new EmpleadoAuxiliar // <-- CORREGIDO AQUÍ
                    {
                        NumeroEmpleado = e.NumeroEmpleado,
                        Nombre = e.Nombre,
                        Departamento = deptoNombre,
                        SalarioMensual = e.SalarioMensual,
                        AntiguedadMeses = meses
                    });
                }
            }

            // Ordenamos de mayor a menor antigüedad de forma clara
            var resultadoOrdenado = listaResultado.OrderByDescending(x => x.AntiguedadMeses).ToList();
            return Ok(resultadoOrdenado);
        }

        // Consulta B: Nómina quincenal completa de todos los activos
        [HttpGet("nomina-completa")]
        public IActionResult ObtenerNominaCompleta()
        {
            var listaNomina = new List<object>();

            foreach (var e in BaseDatosEmpleados)
            {
                if (e.Activo)
                {
                    decimal quincenal = e.SalarioMensual / 2;
                    decimal imss = quincenal * 0.03m;

                    decimal tasa = 0.25m;
                    if (quincenal <= 7500) tasa = 0.09m;
                    else if (quincenal <= 15000) tasa = 0.16m;

                    decimal isr = quincenal * tasa;
                    decimal neto = quincenal - imss - isr;

                    listaNomina.Add(new
                    {
                        e.NumeroEmpleado,
                        e.Nombre,
                        e.SalarioMensual,
                        SalarioQuincenal = Math.Round(quincenal, 2),
                        DeduccionIMSS = Math.Round(imss, 2),
                        DeduccionISR = Math.Round(isr, 2),
                        TotalNeto = Math.Round(neto, 2)
                    });
                }
            }

            return Ok(listaNomina);
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] Empleado nuevo)
        {
            if (nuevo.SalarioMensual <= 0)
            {
                return BadRequest("El salario debe ser mayor a 0.");
            }

            nuevo.Id = BaseDatosEmpleados.Count + 1;
            nuevo.NumeroEmpleado = $"EMP-{nuevo.Id:D4}";
            nuevo.FechaIngreso = DateTime.Today;
            nuevo.Activo = true;

            BaseDatosEmpleados.Add(nuevo);

            return StatusCode(201, nuevo);
        }

        [HttpGet("{id}/nomina")]
        public IActionResult ObtenerNomina(int id)
        {
            var emp = BaseDatosEmpleados.FirstOrDefault(e => e.Id == id && e.Activo);

            if (emp == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            var nomina = _nominaService.CalcularNomina(emp);
            return Ok(nomina);
        }
    }

    // Estructura de apoyo para evitar problemas de nulos en la Consulta A
    public class EmpleadoAuxiliar // <-- NOMBRE UNIFICADO AQUÍ
    {
        public string NumeroEmpleado { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public decimal SalarioMensual { get; set; }
        public int AntiguedadMeses { get; set; }
    }
}