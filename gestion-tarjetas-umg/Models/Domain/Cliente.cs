using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace gestion_tarjetas_umg.Models.Domain
{
    public class Cliente
    {
        public required string nombre { get; set; }
        public required long dpi { get; set; }
        public required string nit { get; set; }
        public required string telefono { get; set; }
        public required string direccion { get; set; }
        public required string email { get; set; }

        // Referencia al usuario asociado con este cliente
        public Usuario? Usuario { get; set; }

        // Estructura personalizada para tarjetas
        public List<Tarjeta> Tarjetas { get; set; }

        public Cliente()
        {
            Tarjetas = new List<Tarjeta>();
        }
    }
}
