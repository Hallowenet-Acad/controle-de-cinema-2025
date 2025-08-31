using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using Testcontainers.PostgreSql;

namespace ControleDeCinema.Testes.Integracao.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected ControleDeCinemaDbContext? dbContext;

    protected RepositorioGeneroFilmeEmOrm? repositorioGenero;
    protected RepositorioFilmeEmOrm? repositorioFilme;
    protected RepositorioIngressoEmOrm? repositorioIngresso;
    protected RepositorioSalaEmOrm? repositorioSala;
    protected RepositorioSessaoEmOrm? repositorioSessao;

    private static IDatabaseContainer? container;

    [AssemblyInitialize]
    public static async Task Setup(TestContext _)
    {
        container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName("controle-cinema-testdb")
            .WithDatabase("AcademiaDoProgramadorDb")
            .WithUsername("postgres")
            .WithPassword("YourStrongPassword")
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsyc();
    }

    [AssemblyCleanup]
    public static async Task Cleanup()
    {
        await EncerrarBancoDadosAsync();
    }

    [TestInitialize]
    public virtual void ConfigurarTestes()
    {
        if (container is null)
            throw new ArgumentNullException("O banco de dados não foi incializado.");

        dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(container.GetConnectionString());

        ConfigurarTabelas(dbContext);

        repositorioFilme = new RepositorioFilmeEmOrm(dbContext);
        repositorioGenero = new RepositorioGeneroFilmeEmOrm(dbContext);
        repositorioIngresso = new RepositorioIngressoEmOrm(dbContext);
        repositorioSala = new RepositorioSalaEmOrm(dbContext);
        repositorioSessao = new RepositorioSessaoEmOrm(dbContext);
    }

    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Filmes.RemoveRange();
        dbContext.GenerosFilme.RemoveRange();
        dbContext.Ingressos.RemoveRange();
        dbContext.Salas.RemoveRange();
        dbContext.Sessoes.RemoveRange();

        dbContext.SaveChanges();
    }

    private static async Task InicializarBancoDadosAsyc()
    {
        await container.StartAsync();
    }

    private static async Task EncerrarBancoDadosAsync()
    {
        if (container is null)
            throw new ArgumentNullException("O banco de dados não foi incializado.");

        await container.StopAsync();
        await container.DisposeAsync();
    }
}
