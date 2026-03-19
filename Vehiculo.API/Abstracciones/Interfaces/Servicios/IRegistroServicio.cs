using Abstracciones.Modelos.Servicios.Registro;

namespace Abstracciones.Interfaces.Servicios
{
    public interface IRegistroServicio
    {
        Task<Propietario> Obtener(string placa);
    }
}
