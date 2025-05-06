namespace gestion_tarjetas_umg.Models.Domain
{
    public class Transaccion
    {
        public required string id { get; set; }
        public required string referencia { get; set; }
        public required double montoCredito { get; set; }
        public required double montoDebito { get; set; }
        public required DateTime fecha { get; set; }
        public string? descripcion { get; set; }
    }
}
