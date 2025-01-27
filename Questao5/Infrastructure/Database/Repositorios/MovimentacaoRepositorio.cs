using System.Data;
using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Database.Repositorios
{
    public class MovimentacaoRepository : IMovimentacaoRepositorio
    {
        private readonly IDbConnection _dbConnection;

        public MovimentacaoRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<decimal> GetTotalCreditosAsync(Guid idContaCorrente)
        {
            var query = @"SELECT ISNULL(SUM(Valor), 0)
                      FROM Movimentacoes
                      WHERE IdContaCorrente = @IdContaCorrente AND TipoMovimentacao = 'C'";

            return await _dbConnection.QueryFirstOrDefaultAsync<decimal>(query, new { IdContaCorrente = idContaCorrente });
        }

        public async Task<decimal> GetTotalDebitosAsync(Guid idContaCorrente)
        {
            var query = @"SELECT ISNULL(SUM(Valor), 0)
                      FROM Movimentacoes
                      WHERE IdContaCorrente = @IdContaCorrente AND TipoMovimentacao = 'D'";

            return await _dbConnection.QueryFirstOrDefaultAsync<decimal>(query, new { IdContaCorrente = idContaCorrente });
        }

        public async Task CriarMovimentacao(Movimento movimento)
        {
            await _dbConnection.ExecuteAsync(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                movimento);
        }
    }
}
