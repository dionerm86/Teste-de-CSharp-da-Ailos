using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using System.Data;
using System.Data.Common;

namespace Questao5.Infrastructure.Database.Repositorios
{
    public class IdempotenciaRepositorio : IIdempotenciaRepositorio
    {
        private readonly IDbConnection _dbConnection;

        public IdempotenciaRepositorio(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task CriarIdempotencia(Idempotencia idempotencia)
        {
            await _dbConnection.ExecuteAsync(@"INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
                VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)", idempotencia);
        }

        public async Task<Idempotencia> IsExisteChaveIdempotente(string chaveIdempotencia)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Idempotencia>(
                "SELECT * FROM idempotencia WHERE chave_idempotencia = @ChaveIdempotencia",
                new { ChaveIdempotencia = chaveIdempotencia }); // Corrigido o nome do parâmetro
        }

    }
}
