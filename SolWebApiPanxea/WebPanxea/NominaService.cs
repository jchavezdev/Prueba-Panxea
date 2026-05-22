using System;

namespace WebPanxea
{
    public class Empleado
    {
        public int Id { get; set; }
        public string NumeroEmpleado { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal SalarioMensual { get; set; }
        public int DepartamentoId { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Activo { get; set; } = true;
    }

    public interface INominaService
    {
        object CalcularNomina(Empleado emp);
    }

    public class NominaService : INominaService
    {
        public object CalcularNomina(Empleado emp)
        {
            decimal quincenal = emp.SalarioMensual / 2;
            decimal imss = quincenal * 0.03m;

            // Rangos de ISR según la tabla del documento
            decimal tasa = 0.25m;
            if (quincenal <= 7500)
                tasa = 0.09m;
            else if (quincenal <= 15000)
                tasa = 0.16m;

            decimal isr = quincenal * tasa;

            return new
            {
                numeroEmpleado = emp.NumeroEmpleado,
                nombre = emp.Nombre,
                salarioQuincenal = Math.Round(quincenal, 2),
                deduccionIMSS = Math.Round(imss, 2),
                deduccionISR = Math.Round(isr, 2),
                salarioNeto = Math.Round(quincenal - imss - isr, 2)
            };
        }
    }
}