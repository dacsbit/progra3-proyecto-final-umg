namespace gestion_tarjetas_umg.Models.DTO
{
    public class CobroPagoDTO
    {
        public required string numTarjeta { get; set; }
        public required string cvv { get; set; }
        public required int mesExp { get; set; }
        public required int anioExp { get; set; }
        public required double monto { get; set; }
    }
}
