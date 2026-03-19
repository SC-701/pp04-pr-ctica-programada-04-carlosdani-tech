using System.Text.Json;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos.Servicios.Revision;

namespace Servicios
{
    public class RevisionServicio : IRevisionServicio
    {
        private readonly IConfiguracion _configuracion;
        private readonly IHttpClientFactory _httpClient;

        public RevisionServicio(IConfiguracion configuracion, IHttpClientFactory httpClient)
        {
            _configuracion = configuracion;
            _httpClient = httpClient;
        }

        public async Task<Revision> Obtener(string placa)
        {
            var endPoint = _configuracion.ObtenerMetodo("ApiEndPointsRevision", "ObtenerRevision");
            if (!Uri.TryCreate(string.Format(endPoint, placa), UriKind.Absolute, out var uri))
                return null;

            var servicioRevision = _httpClient.CreateClient("ServicioRevision");

            try
            {
                var respuesta = await servicioRevision.GetAsync(uri);
                respuesta.EnsureSuccessStatusCode();

                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var resultadoDeserializado = JsonSerializer.Deserialize<List<Revision>>(resultado, opciones);
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
