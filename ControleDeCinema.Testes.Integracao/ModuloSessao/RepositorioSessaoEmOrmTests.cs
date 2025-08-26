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
        repositorioGeneroFilme = new RepositorioGeneroFilmeEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Filme>(repositorioFilme.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<Sala>(repositorioSala.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<GeneroFilme>(repositorioGeneroFilme.Cadastrar);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    [TestMethod]
    public void Deve_Cadastrar_Registro_Corretamente()
    {
        // Arrange
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ação")
            .Persist();
        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();
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
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ação")
            .Persist();
        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();
        var sala = Builder<Sala>.CreateNew().Persist();

        var sessao = new Sessao(DateTime.UtcNow, 30, filme, sala);
        var sessao2 = new Sessao(DateTime.UtcNow, 10, filme, sala);
        var sessao3 = new Sessao(DateTime.UtcNow, 55, filme, sala);


        repositorioSessao.Cadastrar(sessao);
        repositorioSessao.Cadastrar(sessao2);
        repositorioSessao.Cadastrar(sessao3);

        dbContext.SaveChanges();

        List<Sessao> sessoesEsperadas = [sessao, sessao2, sessao3];

        var sessoesEsperadasOrdenadas = sessoesEsperadas
            .OrderBy(s => s.NumeroMaximoIngressos)
            .ToList();

        // Act
        var sessoesRecebidas = repositorioSessao
            .SelecionarRegistros();

        var sessoesRecebidasOrdenadas = sessoesRecebidas
            .OrderBy(s => s.NumeroMaximoIngressos)
            .ToList();

        // Assert
        CollectionAssert.AreEqual(sessoesEsperadasOrdenadas, sessoesRecebidas);
    }
    [TestMethod]
    public void Deve_Editar_Registros_Corretamente()
    {
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ação")
            .Persist();
        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();
        var sala = Builder<Sala>.CreateNew().Persist();

        //Arrange
        var sessao = new Sessao(DateTime.UtcNow, 43, filme, sala);
        repositorioSessao.Cadastrar(sessao);

        dbContext.SaveChanges();

        var sessaoEditada = new Sessao(DateTime.UtcNow, 29, filme, sala);

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
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ação")
            .Persist();
        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();
        var sala = Builder<Sala>.CreateNew().Persist();

        // Arrange
        var sessao = new Sessao(DateTime.UtcNow, 37, filme, sala);

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
