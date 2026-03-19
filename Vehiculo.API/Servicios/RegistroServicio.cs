using System.Text.Json;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos.Servicios.Registro;

namespace Servicios
{
    public class RegistroServicio : IRegistroServicio
    {
        private readonly IConfiguracion _configuracion;
        private readonly IHttpClientFactory _httpClient;

        public RegistroServicio(IConfiguracion configuracion, IHttpClientFactory httpClient)
        {
            _configuracion = configuracion;
            _httpClient = httpClient;
        }

        public async Task<Propietario> Obtener(string placa)
        {
            var endPoint = _configuracion.ObtenerMetodo("ApiEndPointsRegistro", "ObtenerRegistro");
            if (!Uri.TryCreate(string.Format(endPoint, placa), UriKind.Absolute, out var uri))
                return null;

            var servicioRegistro = _httpClient.CreateClient("ServicioRegistro");

            try
            {
                var respuesta = await servicioRegistro.GetAsync(uri);
                respuesta.EnsureSuccessStatusCode();

                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var resultadoDeserializado = JsonSerializer.Deserialize<List<Propietario>>(resultado, opciones);
                return resultadoDeserializado?.FirstOrDefault();
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
