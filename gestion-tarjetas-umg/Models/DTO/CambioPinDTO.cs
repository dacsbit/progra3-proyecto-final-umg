namespace gestion_tarjetas_umg.Models.DTO
{
    public class CambioPinDTO
    {
        public required string numTarjeta { get; set; }
        public required string cvv { get; set; }
        public required int mesExp { get; set; }
        public required int anioExp { get; set; }
        public int nuevoPin { get; set; }
    }
}
