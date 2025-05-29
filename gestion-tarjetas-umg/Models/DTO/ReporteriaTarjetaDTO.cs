namespace gestion_tarjetas_umg.Models.DTO
{
    public class ReporteriaTarjetaDTO : TarjetaConsultaDTO
    {
        public DateTime fechaInicio {  get; set; }
        public DateTime fechaFinal {  get; set; }
    }
}
