using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ControleDeCinema.Testes.Integracao.ModuloSessao;

[TestClass]
[TestCategory("Testes de Integração de Sessão")]
public sealed class RepositorioSessaoEmOrmTests
{
    private ControleDeCinemaDbContext dbContext;
    private RepositorioSessaoEmOrm repositorioSessao;
    private RepositorioGeneroFilmeEmOrm repositorioGeneroFilme;
    private RepositorioSalaEmOrm repositorioSala;
    private RepositorioFilmeEmOrm repositorioFilme;

    [TestInitialize]
    public void ConfigurarTestes()
    {
        var assembly = typeof(RepositorioSessaoEmOrmTests).Assembly;

        var configuracao = new ConfigurationBuilder()
            .AddUserSecrets(assembly)
            .Build();

        var connectionString = configuracao["SQL_CONNECTION_STRING"];

        var options = new DbContextOptionsBuilder<ControleDeCinemaDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        dbContext = new ControleDeCinemaDbContext(options);
        repositorioSessao = new RepositorioSessaoEmOrm(dbContext);
        repositorioSala = new RepositorioSalaEmOrm(dbContext);
        repositorioFilme = new RepositorioFilmeEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Filme>(repositorioFilme.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<Sala>(repositorioSala.Cadastrar);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    [TestMethod]
    public void Deve_Cadastrar_Registro_Corretamente()
    {
        // Arrange
        var filme = Builder<Filme>.CreateNew().Persist();
        var sala = Builder<Sala>.CreateNew().Persist();
        
        var sessao = new Sessao(DateTime.UtcNow, 50, filme, sala);
        
        //Act
        repositorioSessao.Cadastrar(sessao);
        dbContext.SaveChanges();

        //Assert
        var registroSelecionado = repositorioSessao.SelecionarRegistroPorId(sessao.Id);

        Assert.AreEqual(sessao, registroSelecionado);
    }
    [TestMethod]
    public void Deve_Selecionar_Registros_Corretamente()
    {
        // Arrange
        var sessao = new Sessao(DateTime.UtcNow,
            10,
            new Filme("Duna",
            2,
            true,
            new GeneroFilme("Ficção")),
            new Sala(3, 34));

        repositorioSessao.Cadastrar(sessao);
        

        dbContext.SaveChanges();

        List<Sessao> sessoesEsperadas = [sessao];

        var sessoesEsperadasOrdenadas = sessoesEsperadas
            .OrderBy(s => s.Encerrada)
            .ToList();

        // Act
        var sessoesRecebidas = repositorioSessao
            .SelecionarRegistros();

        var sessoesRecebidasOrdenadas = sessoesRecebidas
            .OrderBy(s => s.Encerrada)
            .ToList();

        // Assert
        CollectionAssert.AreEqual(sessoesEsperadasOrdenadas, sessoesRecebidas);
    }
    [TestMethod]
    public void Deve_Editar_Registros_Corretamente()
    {
        //Arrange
        var sessao = new Sessao(DateTime.UtcNow,
            23,
            new Filme("Filme3",
            1,
            true,
            new GeneroFilme("Ação")),
            new Sala(2, 40));
        repositorioSessao.Cadastrar(sessao);
        dbContext.SaveChanges();

        var sessaoEditada = new Sessao(DateTime.UtcNow,
            46,
            new Filme("Filme4",
            4,
            true,
            new GeneroFilme("Suspense")),
            new Sala(2, 40));
        //Act
        var conseguiuEditar = repositorioSessao.Editar(sessao.Id, sessaoEditada);
        dbContext.SaveChanges();

        //Assert
        var registroSelecionado = repositorioSessao.SelecionarRegistroPorId(sessao.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(sessao, registroSelecionado);
    }
    [TestMethod]
    public void Deve_Excluir_Registros_Corretamente()
    {
        // Arrange
        var sessao = new Sessao(DateTime.UtcNow,
            8,
            new Filme("Filme5",
            1,
            true,
            new GeneroFilme("Terror")),
            new Sala(2, 40));
        repositorioSessao.Cadastrar(sessao);
        dbContext.SaveChanges();

        // Act
        var conseguiuExcluir = repositorioSessao.Excluir(sessao.Id);
        dbContext.SaveChanges();

        // Assert 
        var registroSelecionado = repositorioSessao.SelecionarRegistroPorId(sessao.Id);

        Debug.WriteLine(registroSelecionado);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(registroSelecionado);
    }
}
