using gestion_tarjetas_umg.Models.Interfaces;

namespace gestion_tarjetas_umg.Models.Domain
{
    public class Tarjeta
    {
        public required string numeroTarjeta { get; set; }
        public required string cvv { get; set; }
        public required int mesExp { get; set; }
        public required int anioExp { get; set; }
        public required string nombreTarjeta { get; set; }
        public required string red { get; set; }

        public List<Transaccion> transacciones { get; set; }

        public Tarjeta()
        {
            transacciones = new List<Transaccion>();
        }
    }
}
