using Abstracciones.Modelos.Servicios.Revision;

namespace Abstracciones.Interfaces.Servicios
{
    public interface IRevisionServicio
    {
        Task<Revision> Obtener(string placa);
    }
}
