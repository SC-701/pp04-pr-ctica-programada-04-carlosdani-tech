using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;

namespace Reglas
{
    public class RegistroReglas : IRegistroReglas
    {
        private readonly IRegistroServicio _registroServicio;

        public RegistroReglas(IRegistroServicio registroServicio, IConfiguracion configuracion)
        {
            _registroServicio = registroServicio;
        }

        public async Task<bool> VehiculoEstaRegistrado(string placa, string email)
        {
            var resultadoRegistro = await _registroServicio.Obtener(placa);
            return resultadoRegistro != null &&
                   !string.IsNullOrWhiteSpace(resultadoRegistro.Email) &&
                   string.Equals(resultadoRegistro.Email, email, StringComparison.OrdinalIgnoreCase);
        }
    }
}
