using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ControleDeCinema.Testes.Integracao.ModuloSessao;

[TestClass]
[TestCategory("Testes de Integração de Sessão")]
public sealed class RepositorioSessaoEmOrmTests
{
    private ControleDeCinemaDbContext dbContext;
    private RepositorioSessaoEmOrm repositorioSessao;

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

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    [TestMethod]
    public void Deve_Cadastrar_Registro_Corretamente()
    {
        var sessao = new Sessao(DateTime.UtcNow, 
            50, 
            new Filme("Interestelar", 
                3, 
                true, 
                new GeneroFilme("Ficção Científica")), 
            new Sala(1, 50));
        
        repositorioSessao.Cadastrar(sessao);

        dbContext.SaveChanges();

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
}
