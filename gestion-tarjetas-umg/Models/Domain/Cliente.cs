using gestion_tarjetas_umg.Models.Interfaces;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace gestion_tarjetas_umg.Models.Domain
{
    public class Cliente : Comparador<Cliente>
    {
        public required string nombre { get; set; }
        public required long dpi { get; set; }
        public required string nit { get; set; }
        public required string telefono { get; set; }
        public required string direccion { get; set; }
        public required string email { get; set; }

        // Referencia al usuario asociado con este cliente
        public Usuario? Usuario { get; set; }

        // Estructura personalizada para tarjetas
        public List<Tarjeta> Tarjetas { get; set; }

        public Cliente()
        {
            Tarjetas = new List<Tarjeta>();
        }

        bool Comparador<Cliente>.MayorQue(Cliente valor)
        {
            return this.dpi > valor.dpi;
        }

        bool Comparador<Cliente>.MenorQue(Cliente valor)
        {
            return this.dpi < valor.dpi;
        }

        bool Comparador<Cliente>.IgualQue(Cliente valor)
        {
            return this.dpi == valor.dpi;
        }

        public byte[] GenerarPdfDesdeLista(List<Cliente> elementos)
        {
            using MemoryStream ms = new();
            using PdfWriter writer = new(ms);
            using PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            Table tabla = new(6); // Puedes ajustar el número de columnas según la clase

            tabla.AddCell("Nombre");
            tabla.AddCell("DPI");
            tabla.AddCell("NIT");
            tabla.AddCell("Telefono");
            tabla.AddCell("Direccion");
            tabla.AddCell("E-Mail");

            foreach (var item in elementos)
            {
                tabla.AddCell(item.nombre);
                tabla.AddCell($"{item.dpi}");
                tabla.AddCell(item.nit);
                tabla.AddCell(item.telefono);
                tabla.AddCell(item.direccion);
                tabla.AddCell(item.email);
            }

            doc.Add(tabla);
            doc.Close();

            return ms.ToArray();
        }
    }
}
