using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.DA
{
    public interface ICatalogoDA
    {
        Task<IEnumerable<Marca>> ObtenerMarcas();
        Task<IEnumerable<Modelo>> ObtenerModelos(Guid idMarca);
    }
}
