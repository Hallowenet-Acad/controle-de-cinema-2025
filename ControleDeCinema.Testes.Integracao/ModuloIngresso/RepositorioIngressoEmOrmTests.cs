using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ControleDeCinema.Testes.Integracao.ModuloIngresso;

[TestClass]
[TestCategory("Testes de Integração de Ingresso")]
public sealed class RepositorioIngressoEmOrmTests
{
    private ControleDeCinemaDbContext dbContext;
    private RepositorioIngressoEmOrm repositorioIngresso;

    [TestInitialize]
    public void ConfigurarTestes()
    {
        var assembly = typeof(RepositorioIngressoEmOrmTests).Assembly;

        var configuracao = new ConfigurationBuilder()
            .AddUserSecrets(assembly)
            .Build();

        var connectionString = configuracao["SQL_CONNECTION_STRING"];

        var options = new DbContextOptionsBuilder<ControleDeCinemaDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        dbContext = new ControleDeCinemaDbContext(options);
        repositorioIngresso = new RepositorioIngressoEmOrm(dbContext);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }
    [TestMethod]
    public void Deve_Cadastrar_Registro_Corretamente()
    {
        var ingresso = new Ingresso(50,
            true,
            new Sessao(DateTime.UtcNow,
            20,
            new Filme("FilmeTeste",
            2,
            true,
            new GeneroFilme("Comédia")),
            new Sala(2, 40)));

        repositorioIngresso.
    }

}