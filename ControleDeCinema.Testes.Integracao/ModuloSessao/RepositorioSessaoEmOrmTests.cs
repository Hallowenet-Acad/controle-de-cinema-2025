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
        var sessao = new Sessao(DateTime.UtcNow, 50, new Filme("Interestelar", 3, true, new GeneroFilme("Ficção Científica")), new Sala(1, 50));
        
        repositorioSessao.Cadastrar(sessao);

        dbContext.SaveChanges();

        var registroSelecionado = repositorioSessao.SelecionarRegistroPorId(sessao.Id);

        Assert.AreEqual(sessao, registroSelecionado);
    }
}
