namespace gestion_tarjetas_umg.Models.Responses
{
    public class Respuesta<T>
    {
        public bool IsSuccess { get; set; }
        public string Msg { get; set; }
        public T? Data { get; set; }

        public Respuesta() 
        {
            Msg = "";
            IsSuccess = false;
        }
    }
}
