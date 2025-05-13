namespace gestion_tarjetas_umg.Models.Interfaces
{
    public interface Comparador<T>
    {
        public bool MayorQue(T valor);
        public bool MenorQue(T valor);
        public bool IgualQue(T valor);
    }
}
