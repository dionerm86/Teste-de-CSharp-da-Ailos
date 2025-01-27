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

        public async Task CriarMovimentacao(Movimento movimento)
        {
            await _dbConnection.ExecuteAsync(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                movimento);
        }

        public async Task<decimal> ObterSaldoAtualAsync(string idContaCorrente)
        {
            var credito = await ObterTotalMovimenacaoCreditoAsync(idContaCorrente);
            var debito = await ObterTotalMovimenacaoDebitoAsync(idContaCorrente);
            return credito - debito;
        }

        private async Task<decimal> ObterTotalMovimenacaoCreditoAsync(string idContaCorrente)
        {
            var query = @"SELECT COALESCE(SUM(Valor), 0)
                 FROM movimento
                 WHERE IdContaCorrente = @IdContaCorrente AND TipoMovimento = 'C'";

            return await _dbConnection.QueryFirstOrDefaultAsync<decimal>(query, new { IdContaCorrente = idContaCorrente });
        }

        private async Task<decimal> ObterTotalMovimenacaoDebitoAsync(string idContaCorrente)
        {
            var query = @"SELECT COALESCE(SUM(Valor), 0)
                      FROM movimento
                      WHERE IdContaCorrente = @IdContaCorrente AND TipoMovimento = 'D'";

            return await _dbConnection.QueryFirstOrDefaultAsync<decimal>(query, new { IdContaCorrente = idContaCorrente });
        }
    }
}
