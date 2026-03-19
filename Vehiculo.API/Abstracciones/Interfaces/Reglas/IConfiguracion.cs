namespace Abstracciones.Interfaces.Reglas
{
    public interface IConfiguracion
    {
        public string ObtenerMetodo(string seccion, string nombre);
        public string ObtenerValor(string llave);

    }
}
