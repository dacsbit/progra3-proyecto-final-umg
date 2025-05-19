namespace gestion_tarjetas_umg.Models.DTO
{
    public class TarjetaDTO
    {
        public string numeroTarjeta { get; set; }
        public string cvv { get; set; }
        public int mesExp { get; set; }
        public int anioExp { get; set; }
        public string nombreTarjeta { get; set; }
        public string red { get; set; }
        public int pin { get; set; }
        public List<TransaccionDTO> transacciones { get; set; }
    }
}
