using gestion_tarjetas_umg.Models.Interfaces;

namespace gestion_tarjetas_umg.Models.Domain
{
    public class Usuario : Comparador<Usuario>
    {
        public required string nombreUsuario { get; set; }
        public required string contrasena { get; set; }

        // Referencia al cliente asociado a este usuario
        // Esto facilita la navegación bidireccional
        public Cliente? Cliente { get; set; }

        bool Comparador<Usuario>.IgualQue(Usuario valor)
        {
            return this.nombreUsuario.CompareTo(valor.nombreUsuario) == 0;
        }

        bool Comparador<Usuario>.MayorQue(Usuario valor)
        {
            return this.nombreUsuario.CompareTo(valor.nombreUsuario) < 0;
        }

        bool Comparador<Usuario>.MenorQue(Usuario valor)
        {
            return this.nombreUsuario.CompareTo(valor.nombreUsuario) > 0;
        }
    }
}
