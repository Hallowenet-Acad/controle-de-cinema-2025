using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace ControleDeCinema.Testes.Interface.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected static IWebDriver? driver;
    protected static ControleDeCinemaDbContext? dbContext;

    protected static string enderecoBase = "https://localhost:7131";
    private static string connectionString = "Host=localhost;Port=5433;Database=ControleDeCinemaDb;Username=postgres;Password=YourStrongPassword";

    private static IDatabaseContainer? dbContainer;
    private readonly static int dbPort = 5433;

    private static IContainer? appContainer;
    private readonly static int appPort = 8080;
    
    private static IContainer? seleniumContainer;
    private readonly static int seleniumPort = 4444;

    private static IConfiguration? configuracao;

    [AssemblyInitialize]
    public static async Task ConfigurarTestes(TestContext _)
    {
        configuracao = new ConfigurationBuilder()
            .AddUserSecrets<TestFixture>()
            .AddEnvironmentVariables()
            .Build();

        var rede = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString())
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsyc(rede);

        await InicializarAplicacaoAsync(rede);

        await InicializarWebDriverAsync(rede);
    }

    [AssemblyCleanup]
    public static async Task EncerrarTestes()
    {
        EncerrarWebDriverAsync();

        await EncerrarAplicacaoAsync();

        await EncerrarBancoDadosAsyc();
    }
    
    [TestInitialize]
    public void InicializarTeste()
    {
        dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(dbContainer.GetConnectionString());

        ConfigurarTabelas(dbContext);
    }

    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Salas.RemoveRange(dbContext.Salas);
        dbContext.Sessoes.RemoveRange(dbContext.Sessoes);
        dbContext.Ingressos.RemoveRange(dbContext.Ingressos);

        dbContext.SaveChanges();
    }

    private static async Task InicializarBancoDadosAsyc(DotNet.Testcontainers.Networks.INetwork rede)
    {
        dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithPortBinding(dbPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("controle-de-cinema-e2e-testdb")
            .WithName("controle-cinema-testdb")
            .WithDatabase("AcademiaDoProgramadorDb")
            .WithUsername("postgres")
            .WithPassword("YourStrongPassword")
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilPortIsAvailable(dbPort)
            )
            .Build();

        await dbContainer.StartAsync();

        dbContainer.GetConnectionString();
    }

    private static async Task InicializarAplicacaoAsync(DotNet.Testcontainers.Networks.INetwork rede)
    {
        //Configura a imagem à partir do Dockerfile

        var imagem = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("Dockerfile")
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
            .WithName("controle-de-cinema-e2e:latest")
            .Build();
        await imagem.CreateAsync().ConfigureAwait(false);

        //Configura o container da aplicação e inicializa enderecoBase
        var connectionStringRede = dbContainer?.GetConnectionString()
            .Replace(dbContainer.Hostname, "controle-de-cinema-e2e-testdb")
            .Replace(dbContainer.GetMappedPublicPort(dbPort).ToString(), "5433");

        appContainer = new ContainerBuilder()
            .WithImage(imagem)
            .WithPortBinding(appPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("controle-de-cinema-webapp")
            .WithName("controle-de-cinema-webapp")
            .WithEnvironment("SQL_CONNECTION_STRING", connectionStringRede)
            .WithEnvironment("NEWRELIC_LICENSE_KEY", configuracao?["NEWRELIC_LICENSE_KEY"])
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(appPort)
                .UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)appPort).ForPath("/health"))
            )
            .WithCleanUp(true)
            .Build();

        await appContainer.StartAsync();

        enderecoBase = $"http://{appContainer.Name}:{appPort}";
    }
    
    private static async Task InicializarWebDriverAsync(DotNet.Testcontainers.Networks.INetwork rede)
    {
        seleniumContainer = new ContainerBuilder()
            .WithImage("selenium/standalone-chrome:nightly")
            .WithPortBinding(seleniumPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("teste-facil-selenium-e2e")
            .WithExtraHost("host.docker.internal", "host-gateway")
            .WithName("teste-facil-selenium-e2e")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(seleniumPort)
            )
            .Build();

        await seleniumContainer.StartAsync();

        var enderecoSelenium = new Uri($"http://{seleniumContainer.Hostname}:{seleniumContainer.GetMappedPublicPort(seleniumPort)}/wd/hub");

        var options = new ChromeOptions();
        //options.AddArgument("-headless=new");

        driver = new RemoteWebDriver(enderecoSelenium, options);
    }
    
    private static async Task EncerrarBancoDadosAsyc()
    {
        if (dbContainer is not null)
            await dbContainer.DisposeAsync();
    }

    private static async Task EncerrarAplicacaoAsync()
    {
        if (appContainer is not null)
            await appContainer.DisposeAsync();
    }

    private static async Task EncerrarWebDriverAsync()
    {
        driver?.Quit();
        driver?.Dispose();

        if (seleniumContainer is not null)
            await seleniumContainer.DisposeAsync();
    }
}
