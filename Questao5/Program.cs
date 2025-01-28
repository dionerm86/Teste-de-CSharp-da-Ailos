using FluentAssertions.Common;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.OpenApi.Models;
using Questao5.Api.Examples;
using Questao5.Application.AutoMapper;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Database.Repositorios;
using Questao5.Infrastructure.Sqlite;
using Swashbuckle.AspNetCore.Filters;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());


builder.Services.AddScoped<IContaCorrenteRepositorio, ContaCorrenteRepositorio>();
builder.Services.AddScoped<IIdempotenciaRepositorio, IdempotenciaRepositorio>();
builder.Services.AddScoped<IMovimentacaoRepositorio, MovimentacaoRepository>();


builder.Services.AddScoped<IDbConnection>(sp =>
{
    var databaseConfig = sp.GetRequiredService<DatabaseConfig>();
    return new SqliteConnection(databaseConfig.Name);
});

// sqlite
builder.Services.AddSingleton(new DatabaseConfig { Name = builder.Configuration.GetValue<string>("DatabaseName", "Data Source=database.sqlite") });
builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });
    c.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<CriarMovimentacaoRequestExemplo>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// sqlite
#pragma warning disable CS8602 // Dereference of a possibly null reference.
app.Services.GetService<IDatabaseBootstrap>().Setup();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

app.Run();

// Informações úteis:
// Tipos do Sqlite - https://www.sqlite.org/datatype3.html


