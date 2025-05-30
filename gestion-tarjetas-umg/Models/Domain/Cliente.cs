using gestion_tarjetas_umg.Models.Interfaces;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using gestion_tarjetas_umg.Models.Estructuras.Listas;

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
        public ListaSimple<Tarjeta> Tarjetas { get; set; }

        public Cliente()
        {
            Tarjetas = new ListaSimple<Tarjeta>();
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

        void Comparador<Cliente>.Actualizar(Cliente valor)
        {
            this.nombre = valor.nombre;
            this.dpi = valor.dpi;
            this.nit = valor.nit;
            this.telefono = valor.telefono;
            this.direccion = valor.direccion;
            this.email = valor.email;
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

        public byte[] ListadoTarjetas()
        {
            using MemoryStream ms = new();
            using PdfWriter writer = new(ms);
            using PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            // TÍTULO
            var titulo = new Paragraph("Lista de Tarjetas")
                .SetFontSize(20)
                .SimulateBold()
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
            doc.Add(titulo);

            // Fecha de datos de reporte
            var fechaGeneracion = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");
            doc.Add(new Paragraph($"Fecha de generación del reporte: {fechaGeneracion}"));
            doc.Add(new Paragraph($"Cliente: {this.nombre}"));
            doc.Add(new Paragraph($"DPI: {this.dpi}"));
            doc.Add(new Paragraph($"NIT: {this.nit}"));

            // Espaciado
            doc.Add(new Paragraph("\n"));

            List<Tarjeta> tarjetasCliente = this.Tarjetas.ToList();
            
            // TABLA DE TARJETAS
            Table tabla = new(9);

            tabla.AddHeaderCell("# Tarjeta");
            tabla.AddHeaderCell("CVV");
            tabla.AddHeaderCell("Expira");
            tabla.AddHeaderCell("Nombre");
            tabla.AddHeaderCell("Red");
            tabla.AddHeaderCell("PIN");
            tabla.AddHeaderCell("Limite de Credito");
            tabla.AddHeaderCell("Activa");
            tabla.AddHeaderCell("Bloqueada");

            foreach (var tarjeta in tarjetasCliente)
            {
                tabla.AddCell($"**** **** **** {tarjeta.numeroTarjeta[^4..]}");
                tabla.AddCell(tarjeta.cvv);
                tabla.AddCell($"{tarjeta.mesExp:D2}/{tarjeta.anioExp % 100:D2}");
                tabla.AddCell(tarjeta.nombreTarjeta);
                tabla.AddCell(tarjeta.red);
                tabla.AddCell($"{tarjeta.pin}");
                tabla.AddCell($"Q.{tarjeta.limiteCredito}");
                tabla.AddCell(tarjeta.activa ? "SI" :"NO");
                tabla.AddCell(tarjeta.bloqueada ? "SI" :"NO");
            }

            doc.Add(tabla);
            doc.Close();

            return ms.ToArray();
        }
    }
}
