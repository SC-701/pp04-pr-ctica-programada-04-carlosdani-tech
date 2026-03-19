using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.Extensions.Configuration;

namespace Reglas
{
    public class Configuracion : IConfiguracion
    {
        private readonly IConfiguration _configuracion;

        public Configuracion(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        public string ObtenerMetodo(string seccion, string nombre)
        {
            var apiEndPoint = _configuracion.GetSection(seccion).Get<APIEndPoint>();
            if (apiEndPoint == null)
                return string.Empty;

            var metodo = apiEndPoint.Metodos?.FirstOrDefault(m => m.Nombre == nombre)?.Valor;
            if (string.IsNullOrWhiteSpace(metodo))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(apiEndPoint.UrlBase))
                return metodo;

            return $"{apiEndPoint.UrlBase.TrimEnd('/')}/{metodo.TrimStart('/')}";
        }
    }
}
