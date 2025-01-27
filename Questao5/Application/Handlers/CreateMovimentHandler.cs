using Dapper;
using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;
using System.Data;

namespace Application.Commands.Handlers
{
    public class CreateMovementHandler : IRequestHandler<CreateMovementCommand, string>
    {
        private readonly IDbConnection _dbConnection;

        public CreateMovementHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> Handle(CreateMovementCommand request, CancellationToken cancellationToken)
        {
            // Validations
            var account = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente",
                new { request.IdContaCorrente });

            if (account == null)
                throw new Exception("INVALID_ACCOUNT: Conta corrente não encontrada.");

            if (account.Ativo == 0)
                throw new Exception("INACTIVE_ACCOUNT: Conta corrente inativa.");

            if (request.Valor <= 0)
                throw new Exception("INVALID_VALUE: O valor deve ser positivo.");

            if (request.TipoMovimentacao != "C" && request.TipoMovimentacao != "D")
                throw new Exception("INVALID_TYPE: Tipo de movimento inválido.");

            // Idempotency check
            var existingRequest = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT * FROM idempotencia WHERE chave_idempotencia = @IdContaCorrente",
                new { request.IdContaCorrente });

            if (existingRequest != null)
                return existingRequest.resultado;

            // Serializa a requisição em JSON
            var requestJson = JsonConvert.SerializeObject(request);

            // Cria o movimento na conta corrente
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                TipoMovimento = request.TipoMovimentacao,
                Valor = request.Valor
            };

            // Insert movement
            var movementId = Guid.NewGuid().ToString();
            var date = DateTime.UtcNow.ToString("dd/MM/yyyy");

            await _dbConnection.ExecuteAsync(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                new { IdMovimento = movementId, IdContaCorrente = request.IdContaCorrente, DataMovimento = date, TipoMovimento = request.TipoMovimentacao, request.Valor });

            var resultJson = JsonConvert.SerializeObject(new { movimento.IdMovimento });

            // Save idempotency
            var insertIdempotencySql = @"
                INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
                VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";

            var idempotencia = new Idempotencia
            {
                ChaveIdempotencia = request.IdContaCorrente,
                Requisicao = requestJson,
                Resultado = resultJson
            };

            await _dbConnection.ExecuteAsync(insertIdempotencySql, idempotencia);

            return movementId;
        }
    }
}
