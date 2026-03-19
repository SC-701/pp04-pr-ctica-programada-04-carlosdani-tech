using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DA
{
    public class VehiculoDA : IVehiculoDA
    {
        private readonly IRepositorioDapper _repositorioDapper;
        private readonly SqlConnection _sqlConnection;

        public VehiculoDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _sqlConnection = _repositorioDapper.ObtenerRepositorio();
        }

        public async Task<Guid> Agregar(VehiculoRequest vehiculo)
        {
            const string query = @"AgregarVehiculo";
            var resultadoConsulta = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new
            {
                Id = Guid.NewGuid(),
                IdModelo = vehiculo.IdModelo,
                Placa = vehiculo.Placa,
                Color = vehiculo.Color,
                Anio = vehiculo.Anio,
                Precio = vehiculo.Precio,
                CorreoPropietario = vehiculo.CorreoPropietario,
                TelefonoPropietario = vehiculo.TelefonoPropietario
            }, commandType: CommandType.StoredProcedure);

            return resultadoConsulta;
        }

        public async Task<Guid> Editar(Guid id, VehiculoRequest vehiculo)
        {
            await VerificarVehiculoExiste(id);

            const string query = @"EditarVehiculo";
            var resultadoConsulta = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new
            {
                Id = id,
                IdModelo = vehiculo.IdModelo,
                Placa = vehiculo.Placa,
                Color = vehiculo.Color,
                Anio = vehiculo.Anio,
                Precio = vehiculo.Precio,
                CorreoPropietario = vehiculo.CorreoPropietario,
                TelefonoPropietario = vehiculo.TelefonoPropietario
            }, commandType: CommandType.StoredProcedure);

            return resultadoConsulta;
        }

        public async Task<Guid> Eliminar(Guid id)
        {
            await VerificarVehiculoExiste(id);

            const string query = @"EliminarVehiculo";
            var resultadoConsulta = await _sqlConnection.ExecuteScalarAsync<Guid>(query, new
            {
                Id = id
            }, commandType: CommandType.StoredProcedure);

            return resultadoConsulta;
        }

        public async Task<IEnumerable<VehiculoResponse>> Obtener()
        {
            const string query = @"ObtenerVehiculos";
            return await _sqlConnection.QueryAsync<VehiculoResponse>(query, commandType: CommandType.StoredProcedure);
        }

        public async Task<VehiculoDetalle> Obtener(Guid id)
        {
            const string query = @"ObtenerVehiculo";
            var resultadoConsulta = await _sqlConnection.QueryAsync<VehiculoDetalle>(query, new
            {
                Id = id
            }, commandType: CommandType.StoredProcedure);

            return resultadoConsulta.FirstOrDefault();
        }

        private async Task VerificarVehiculoExiste(Guid id)
        {
            VehiculoResponse? resultadoConsultaVehiculo = await Obtener(id);
            if (resultadoConsultaVehiculo == null)
            {
                throw new Exception("El vehículo no existe.");
            }
        }
    }
}
