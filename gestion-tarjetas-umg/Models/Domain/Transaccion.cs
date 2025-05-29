using gestion_tarjetas_umg.Models.Interfaces;

namespace gestion_tarjetas_umg.Models.Domain
{
    public class Transaccion : Comparador<Transaccion>
    {
        public required string id { get; set; }
        public required string referencia { get; set; }
        public required double montoCredito { get; set; }
        public required double montoDebito { get; set; }
        public required DateTime fecha { get; set; }
        public string? descripcion { get; set; }

        void Comparador<Transaccion>.Actualizar(Transaccion valor)
        {
            throw new NotImplementedException();
        }

        bool Comparador<Transaccion>.IgualQue(Transaccion valor)
        {
            throw new NotImplementedException();
        }

        bool Comparador<Transaccion>.MayorQue(Transaccion valor)
        {
            return this.fecha > valor.fecha;
        }

        bool Comparador<Transaccion>.MenorQue(Transaccion valor)
        {
            return this.fecha <= valor.fecha;
        }
    }
}
