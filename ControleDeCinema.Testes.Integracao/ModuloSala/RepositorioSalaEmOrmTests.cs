using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ControleDeCinema.Testes.Integracao.ModuloSala;

[TestClass]
[TestCategory("Testes de Integração de Sala")]
public sealed class RepositorioSalaEmOrmTests
{
    private ControleDeCinemaDbContext dbContext;
    private RepositorioSalaEmOrm repositorioSala;
    
    [TestInitialize]        
    public void ConfigurarTestes()
    {
        var assembly = typeof(RepositorioSalaEmOrmTests).Assembly;

        var configuracao = new ConfigurationBuilder()
            .AddUserSecrets(assembly)
            .Build();

        var connectionString = configuracao["SQL_CONNECTION_STRING"];

        var options = new DbContextOptionsBuilder<ControleDeCinemaDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        dbContext = new ControleDeCinemaDbContext(options);
        repositorioSala = new RepositorioSalaEmOrm(dbContext);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    [TestMethod]
    public void Deve_Cadastrar_Registro_Corretamente()
    {
        // Arrange
        var sala = new Sala(3, 60);
        
        // Act
        repositorioSala.Cadastrar(sala);

        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioSala.SelecionarRegistroPorId(sala.Id);

        Assert.AreEqual(sala, registroSelecionado);
    }
}
