using gestion_tarjetas_umg.Models.Interfaces;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;

namespace gestion_tarjetas_umg.Models.Domain
{
    public class Usuario : Comparador<Usuario>
    {
        public string claveUnica {  get; set; } = Guid.NewGuid().ToString();
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

        public byte[] GenerarPdfDesdeLista(List<Usuario> elementos)
        {
            using MemoryStream ms = new();
            using PdfWriter writer = new(ms);
            using PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            Table tabla = new(3); // Puedes ajustar el número de columnas según la clase

            tabla.AddCell("Clave");
            tabla.AddCell("NombreUsuario");
            tabla.AddCell("Contraseña");

            foreach (var item in elementos)
            {
                tabla.AddCell(item.claveUnica);
                tabla.AddCell(item.nombreUsuario);
                tabla.AddCell(item.contrasena);
            }

            doc.Add(tabla);
            doc.Close();

            return ms.ToArray();
        }
    }
}
