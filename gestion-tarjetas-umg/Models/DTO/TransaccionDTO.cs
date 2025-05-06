namespace gestion_tarjetas_umg.Models.DTO
{
    public class TransaccionDTO
    {
        public string id { get; set; }
        public string referencia { get; set; }
        public double montoCredito { get; set; }
        public double montoDebito { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
    }
}
