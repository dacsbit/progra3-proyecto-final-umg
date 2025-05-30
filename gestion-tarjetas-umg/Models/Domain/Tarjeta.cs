using gestion_tarjetas_umg.Models.Interfaces;
using gestion_tarjetas_umg.Models.Estructuras.Listas;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using gestion_tarjetas_umg.Models.Estructuras.Arboles.AVL;
using System;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace gestion_tarjetas_umg.Models.Domain
{
    public class Tarjeta : Comparador<Tarjeta>
    {
        public string numeroTarjeta { get; set; }
        public string cvv { get; set; }
        public int mesExp { get; set; }
        public int anioExp { get; set; }
        public required string nombreTarjeta { get; set; }
        public string red { get; set; }
        public int pin {  get; set; }
        public double limiteCredito { get; set; }
        public bool activa { get; set; }
        public bool bloqueada { get; set; }

        public ArbolAvl<Transaccion> transacciones { get; set; }

        public Tarjeta()
        {
            numeroTarjeta = GenerarNumeroTarjetaConLuhn();
            Random rnd = new();
            cvv = rnd.Next(100, 999).ToString();
            mesExp = rnd.Next(1, 13);
            anioExp = DateTime.UtcNow.Year + rnd.Next(1, 5);
            red = rnd.Next(0, 2) == 0 ? "Visa" : "MasterCard";
            pin = rnd.Next(1000, 9999);
            limiteCredito = 6000;
            activa = true;
            bloqueada = true;
            transacciones = new ArbolAvl<Transaccion>();
        }

        bool Comparador<Tarjeta>.MayorQue(Tarjeta valor)
        {
            throw new NotImplementedException();
        }

        bool Comparador<Tarjeta>.MenorQue(Tarjeta valor)
        {
            throw new NotImplementedException();
        }

        bool Comparador<Tarjeta>.IgualQue(Tarjeta valor)
        {
            return (this.numeroTarjeta == valor.numeroTarjeta && this.cvv == valor.cvv && this.mesExp == valor.mesExp && this.anioExp == valor.anioExp);
        }

        void Comparador<Tarjeta>.Actualizar(Tarjeta valor)
        {
            throw new NotImplementedException();
        }

        public byte[] GenerarMovimientos(DateTime fechaInicio, DateTime fechaFinal)
        {
            using MemoryStream ms = new();
            using PdfWriter writer = new(ms);
            using PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            // TÍTULO
            var titulo = new Paragraph("Estado de Cuenta")
                .SetFontSize(20)
                .SimulateBold()
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
            doc.Add(titulo);

            // Fecha de generación y rango
            var fechaGeneracion = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");
            doc.Add(new Paragraph($"Fecha de generación del reporte: {fechaGeneracion}"));
            doc.Add(new Paragraph($"Movimientos del {fechaInicio:dd/MM/yyyy} al {fechaFinal:dd/MM/yyyy}"));

            // Espaciado
            doc.Add(new Paragraph("\n"));

            List<Transaccion> movimientos = transacciones.ToList();
            double saldoCalculado = 0.00;

            // SALDO INICIAL
            foreach (var movimiento in movimientos)
            {
                if (movimiento.fecha.Date < fechaInicio.Date)
                {
                    saldoCalculado += movimiento.montoCredito - movimiento.montoDebito;
                }
            }

            Table tablaSaldoAnterior = new(2);
            tablaSaldoAnterior.AddCell("Saldo Inicial:");
            tablaSaldoAnterior.AddCell($"{Math.Round(saldoCalculado, 2, MidpointRounding.AwayFromZero)}");
            doc.Add(tablaSaldoAnterior);

            // Espaciado
            doc.Add(new Paragraph("\n"));

            // TABLA DE MOVIMIENTOS
            Table tabla = new(7);

            tabla.AddHeaderCell("ID");
            tabla.AddHeaderCell("Referencia");
            tabla.AddHeaderCell("Crédito");
            tabla.AddHeaderCell("Débito");
            tabla.AddHeaderCell("Saldo");
            tabla.AddHeaderCell("Fecha");
            tabla.AddHeaderCell("Descripción");

            foreach (var movimiento in movimientos)
            {
                if (movimiento.fecha.Date >= fechaInicio.Date && movimiento.fecha.Date <= fechaFinal.Date)
                {
                    saldoCalculado += movimiento.montoCredito - movimiento.montoDebito;

                    tabla.AddCell(movimiento.id);
                    tabla.AddCell(movimiento.referencia);
                    tabla.AddCell($"{movimiento.montoCredito:F2}");
                    tabla.AddCell($"{movimiento.montoDebito:F2}");
                    tabla.AddCell($"{Math.Round(saldoCalculado, 2, MidpointRounding.AwayFromZero)}");
                    tabla.AddCell(movimiento.fecha.ToString("g")); // formato corto y legible
                    tabla.AddCell(movimiento.descripcion);
                }
            }

            doc.Add(tabla);

            // Espaciado
            doc.Add(new Paragraph("\n"));

            // SALDO FINAL
            foreach (var movimiento in movimientos)
            {
                if (movimiento.fecha.Date > fechaFinal.Date)
                {
                    saldoCalculado += movimiento.montoCredito - movimiento.montoDebito;
                }
            }

            Table tablaSaldoTarjeta = new(2);
            tablaSaldoTarjeta.AddCell("Saldo en tarjeta:");
            tablaSaldoTarjeta.AddCell($"{Math.Round(saldoCalculado, 2, MidpointRounding.AwayFromZero)}");

            doc.Add(tablaSaldoTarjeta);
            doc.Close();

            return ms.ToArray();
        }

        public string GenerarNumeroTarjetaConLuhn()
        {
            Random random = new();
            int[] tarjeta = new int[16];

            // Prefijo VISA (4xxx)
            tarjeta[0] = 4;
            for (int i = 1; i < 15; i++)
            {
                tarjeta[i] = random.Next(0, 10);
            }

            // Calcular dígito verificador (Luhn)
            int suma = 0;
            for (int i = 0; i < 15; i++)
            {
                int val = tarjeta[14 - i];
                if (i % 2 == 0)
                {
                    val *= 2;
                    if (val > 9) val -= 9;
                }
                suma += val;
            }

            tarjeta[15] = (10 - (suma % 10)) % 10;

            return string.Concat(tarjeta);
        }
    }
}
