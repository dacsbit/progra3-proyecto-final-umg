namespace gestion_tarjetas_umg.Models.DTO
{
    public class ClienteSimpleDTO
    {
        public string? nombre { get; set; }
        public long dpi { get; set; }
        public string? nit { get; set; }
        public string? telefono { get; set; }
        public string? direccion { get; set; }
        public string? email { get; set; }
        public UsuarioDTO? usuario { get; set; }
    }
}
