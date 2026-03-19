using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;

namespace DA
{
    public class CatalogoDA : ICatalogoDA
    {
        private readonly IRepositorioDapper _repositorioDapper;

        public CatalogoDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
        }

        public async Task<IEnumerable<Marca>> ObtenerMarcas()
        {
            const string query = """
                SELECT Id, Nombre
                FROM dbo.Marcas
                ORDER BY Nombre
                """;

            return await _repositorioDapper.ObtenerRepositorio().QueryAsync<Marca>(query);
        }

        public async Task<IEnumerable<Modelo>> ObtenerModelos(Guid idMarca)
        {
            const string query = """
                SELECT Id, IdMarca, Nombre
                FROM dbo.Modelos
                WHERE IdMarca = @IdMarca
                ORDER BY Nombre
                """;

            return await _repositorioDapper.ObtenerRepositorio().QueryAsync<Modelo>(query, new
            {
                IdMarca = idMarca
            });
        }
    }
}
