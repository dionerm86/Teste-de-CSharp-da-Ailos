using System.Data;
using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Database.Repositorios;

public class ContaCorrenteRepositorio : IContaCorrenteRepositorio
{
    private readonly IDbConnection _dbConnection;

    public ContaCorrenteRepositorio(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<ContaCorrente> ObterPorId(string idContaCorrente)
    {
        var query = @"SELECT IdContaCorrente, Numero, Nome, Ativo FROM ContaCorrente WHERE IdContaCorrente = @IdContaCorrente";

        return await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(query, new { IdContaCorrente = idContaCorrente });
    }
}
