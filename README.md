# Desafio Técnico - Empresa Ailos

Este repositório contém as soluções para as questões enviadas pela empresa Ailos. Abaixo está uma breve descrição do que foi feito para cada questão.

## Questões

1. **[Questão 1: Cadastro de Conta Bancária]**
   - Descrição: A questão envolvia o cadastro de uma conta bancária, sendo opcional um valor de depósito inicial. A aplicação deveria ser capaz de realizar Depósitos e Saques das contas
     dos usuários cadastros.
   - Solução: Criei uma entidade chamada ContaBancaria, contendo métodos para Depósito e Saque, conforme solicitados.

2. **[Questão 2: Requisição em api externa e Lógica para contabilizar Gols de um time de Futebol]**
   - Descrição: Nesta questão, foi necessário fazer uma requisição Http em uma api externa, que fornecia informações sobre partidas entre times de futebol. Com o retorno da requisição, eu deveria
     calcular o total de gols feitos em 2013 pelo Paris Saint-Germain e o total de gols feitos em 2014 pelo Chelsea
   - Solução: No método GetTotalScoredGoals vazio que foi forncedido, eu criei uma lista das partidas realizadas no período solicitado. Então criei uma lógica para buscar e percorrer as 3 páginas
     que a api retornava e com isso, buscar os gols do time em questão, somando-os a cada iteração da lista.

3. **[Questão 3: : Git e nano]**
   - Descrição: A questão envolvia a criação de arquivos e commits utilizando o Git, além de edições simples em arquivos com o editor de texto `nano`.
   - Solução: Interpretei as intruções git e nano para chegar ao resultado. Validei a resposta reproduzindo as instruções no GitBach que possui suporte aos comando git e nano.

4. **[Questão 4: Criação de consulta SQL]**
   - Descrição: Na questão 4 foi fornecido um script sql para criar uma tabela e inserir registro e então fazer uma consulta com por uma coluna que tenha mais de 3 ocorrências num determinado período
   - Solução: Criei o comando select e o executei no SQLite, retornando os valores esperados.

5. **[Questão 5: Criação de API para realizar movimentações bancárias]**
   - Descrição: A última questão tratava de criar uma api que pudesse receber requisições para fazer depósitos e débitos numa conta bancária previamente cadastrada. A api também deveria fornecer uma endponit
     para consultar o saldo de um determinado cliente.
   - Solução: Foi fornecido um projeto com estrutura de pastas e banco de dados previamente criados. Para simular uma arquitetura em camadas, eu organizei as pastas de forma a simular uma arquitetura em camadas.
     Para esta aplicação, utilizei as seguintes tecnologias:
     . Padrão Mediator para reduzir as dependências entre os objetos
     . Dapper para realizar as consultas e inserções de registros no banco de dados
     . Swagger para documentar os serviços
     . Testes unitário com NSubstitute e XUnit para testar o fluxo do sistema, mas cenários de sucesso e falhas
     . AutoMapper para mapeamento de objetos
     . Polly para implementar Polític de Retry e CircuitBreaker nas requisições
    Criei classes Handlers para implementar tratar das movimentações e consultas de saldos, fazendo as interações necessárias com o banco de dados e retornando as informações pertinentes.
