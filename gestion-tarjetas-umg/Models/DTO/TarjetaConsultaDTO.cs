namespace gestion_tarjetas_umg.Models.DTO
{
    public class TarjetaConsultaDTO
    {
        public required string numTarjeta {  get; set; }
        public required string cvv {  get; set; }
        public required int mesExp { get; set; }
        public required int anioExp { get; set; }
    }
}
