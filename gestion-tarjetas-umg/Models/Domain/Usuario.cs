namespace gestion_tarjetas_umg.Models.Domain
{
    public class Usuario
    {
        public required string nombreUsuario { get; set; }
        public required string contrasena { get; set; }

        // Referencia al cliente asociado a este usuario
        // Esto facilita la navegación bidireccional
        public Cliente? Cliente { get; set; }
    }
}
