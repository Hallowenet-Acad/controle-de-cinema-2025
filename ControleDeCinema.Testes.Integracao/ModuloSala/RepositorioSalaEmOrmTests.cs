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

    [TestMethod]
    public void Deve_Selecionar_Registros_Corretamente()
    {
        // Arrange
        var sala = new Sala(3, 60);
        var sala2 = new Sala(1, 70);
        var sala3 = new Sala(2, 58);

        repositorioSala.Cadastrar(sala);
        repositorioSala.Cadastrar(sala2);
        repositorioSala.Cadastrar(sala3);

        dbContext.SaveChanges();

        List<Sala> salasEsperadas = [sala, sala2, sala3];

        var salasEsperadasOrdenadas = salasEsperadas
            .OrderBy(s => s.Numero)
            .ToList();

        // Act
        var salasRecebidas = repositorioSala
            .SelecionarRegistros();

        var salasRecebidasOrdenadas = salasRecebidas
            .OrderBy(s => s.Numero)
            .ToList();

        // Assert
        CollectionAssert.AreEqual(salasEsperadasOrdenadas, salasRecebidas);
    }

}
