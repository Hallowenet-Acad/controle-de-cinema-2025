using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Testcontainers.PostgreSql;

namespace ControleDeCinema.Testes.Interface.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected static IWebDriver? driver;
    protected static ControleDeCinemaDbContext? dbContext;

    protected const string emailCliente = "clienteTeste@gmail.com";
    protected const string emailEmpresa = "empresaTeste@gmail.com";
    protected const string senhaPadrao = "Teste123!";

    protected static string enderecoBase = "https://localhost:7131";
    private static string connectionString = "Host=localhost;Port=5432;Database=ControleDeCinemaDb;Username=postgres;Password=YourStrongPassword";

    private static IDatabaseContainer? dbContainer;
    private readonly static int dbPort = 5432;

    private static IContainer? appContainer;
    private readonly static int appPort = 8080;

    private static IContainer? seleniumContainer;
    private readonly static int seleniumPort = 4444;

    private static IConfiguration? configuracao;
    private static DotNet.Testcontainers.Networks.INetwork rede;

    [AssemblyInitialize]
    public static async Task ConfigurarTestes(TestContext _)
    {
        configuracao = new ConfigurationBuilder()
            .AddUserSecrets<TestFixture>()
            .AddEnvironmentVariables()
            .Build();

        rede = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsyc();

        await InicializarAplicacaoAsync();

        await InicializarWebDriverAsync();
    }

    [AssemblyCleanup]
    public static async Task EncerrarTestes()
    {
        await EncerrarWebDriverAsync();

        await EncerrarAplicacaoAsync();

        await EncerrarBancoDadosAsyc();
    }

    [TestInitialize]
    public virtual void InicializarTeste()
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
        dbContext.Filmes.RemoveRange(dbContext.Filmes);
        dbContext.GenerosFilme.RemoveRange(dbContext.GenerosFilme);

        dbContext.UserRoles.RemoveRange(dbContext.UserRoles);
        dbContext.Users.RemoveRange(dbContext.Users);

        dbContext.SaveChanges();
    }

    private static async Task InicializarBancoDadosAsyc()
    {
        dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithPortBinding(dbPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("controle-de-cinema-e2e-testdb")
            .WithName("controle-de-cinema-e2e-testdb")
            .WithDatabase("ControleDeCinemaDbTestes")
            .WithUsername("postgres")
            .WithPassword("YourStrongPassword")
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilExternalTcpPortIsAvailable(dbPort)
            )
            .Build();

        await dbContainer.StartAsync();
    }

    private static async Task InicializarAplicacaoAsync()
    {
        //Configura a imagem à partir do Dockerfile

        //var imagem = new ImageFromDockerfileBuilder()
        //    .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
        //    .WithDockerfile("Dockerfile")
        //    .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
        //    .WithName("controle-de-cinema-e2e:latest")
        //    .Build();
        //await imagem.CreateAsync().ConfigureAwait(false);

        //Configura o container da aplicação e inicializa enderecoBase
        var connectionStringRede = dbContainer?.GetConnectionString()
            .Replace(dbContainer.Hostname, "controle-de-cinema-e2e-testdb")
            .Replace(dbContainer.GetMappedPublicPort(dbPort).ToString(), "5432");

        appContainer = new ContainerBuilder()
            .WithImage("controledecinemawebapp:latest")
            .WithPortBinding(appPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("controle-de-cinema-webapp")
            .WithName("controle-de-cinema-webapp")
            .WithEnvironment("SQL_CONNECTION_STRING", connectionStringRede)
            .WithEnvironment("NEWRELIC_LICENSE_KEY", configuracao?["NEWRELIC_LICENSE_KEY"])
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilExternalTcpPortIsAvailable(appPort)
                .UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)appPort).ForPath("/health"))
            )
            .WithCleanUp(true)
            .Build();

        await appContainer.StartAsync();

        enderecoBase = $"http://{appContainer.Name}:{appPort}";
    }

    private static async Task InicializarWebDriverAsync()
    {
        seleniumContainer = new ContainerBuilder()
            .WithImage("selenium/standalone-chrome:nightly")
            .WithPortBinding(seleniumPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("controle-de-cinema-selenium-e2e")
            .WithExtraHost("host.docker.internal", "host-gateway")
            .WithName("controle-de-cinema-selenium-e2e")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilExternalTcpPortIsAvailable(seleniumPort)
                .UntilInternalTcpPortIsAvailable(seleniumPort)
            )
            .Build();

        await seleniumContainer.StartAsync();

        var enderecoSelenium = new Uri($"http://{seleniumContainer.Hostname}:{seleniumContainer.GetMappedPublicPort(seleniumPort)}/wd/hub");

        var options = new ChromeOptions();
        options.AddArgument("--window-size=1920,2000");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");
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

    protected static void RegistrarContaEmpresarial()
    {
        driver.Navigate().GoToUrl($"{enderecoBase}/autenticacao/registro");

        IWebElement inputEmail = driver.FindElement(By.CssSelector("input[data-se='inputEmail']"));
        IWebElement inputSenha = driver.FindElement(By.CssSelector("input[data-se='inputSenha']"));
        IWebElement inputConfirmarSenha = driver.FindElement(By.CssSelector("input[data-se='inputConfirmarSenha']"));
        SelectElement selectTipoUsuario = new(driver.FindElement(By.CssSelector("select[data-se='selectTipoUsuario']")));

        inputEmail.Clear();
        inputEmail.SendKeys(emailEmpresa);

        inputSenha.Clear();
        inputSenha.SendKeys(senhaPadrao);

        inputConfirmarSenha.Clear();
        inputConfirmarSenha.SendKeys(senhaPadrao);

        selectTipoUsuario.SelectByText("Empresa");

        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(20));

        wait.Until(d =>
        {
            IWebElement btn = d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"));
            if (!btn.Enabled || !btn.Displayed) return false;
            btn.Click();
            return true;
        });

        wait.Until(d =>
            !d.Url.Contains("/autenticacao/registro", StringComparison.OrdinalIgnoreCase) &&
            d.FindElements(By.CssSelector("form[action='/autenticacao/registro']")).Count == 0
        );

        wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count > 0);
    }

    protected static void RegistrarContaCliente()
    {
        driver.Navigate().GoToUrl($"{enderecoBase}/autenticacao/registro");

        IWebElement inputEmail = driver.FindElement(By.CssSelector("input[data-se='inputEmail']"));
        IWebElement inputSenha = driver.FindElement(By.CssSelector("input[data-se='inputSenha']"));
        IWebElement inputConfirmarSenha = driver.FindElement(By.CssSelector("input[data-se='inputConfirmarSenha']"));
        SelectElement selectTipoUsuario = new(driver.FindElement(By.CssSelector("select[data-se='selectTipoUsuario']")));

        inputEmail.Clear();
        inputEmail.SendKeys(emailCliente);

        inputSenha.Clear();
        inputSenha.SendKeys(senhaPadrao);

        inputConfirmarSenha.Clear();
        inputConfirmarSenha.SendKeys(senhaPadrao);

        selectTipoUsuario.SelectByText("Cliente");

        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(20));

        wait.Until(d =>
        {
            IWebElement btn = d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"));
            if (!btn.Enabled || !btn.Displayed) return false;
            btn.Click();
            return true;
        });

        wait.Until(d =>
            !d.Url.Contains("/autenticacao/registro", StringComparison.OrdinalIgnoreCase) &&
            d.FindElements(By.CssSelector("form[action='/autenticacao/registro']")).Count == 0
        );

        wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count > 0);
    }

    protected static void FazerLogin(string tipoConta)
    {
        driver.Navigate().GoToUrl($"{enderecoBase}/autenticacao/login");

        IWebElement inputEmail = driver.FindElement(By.CssSelector("input[data-se='inputEmail']"));
        IWebElement inputSenha = driver.FindElement(By.CssSelector("input[data-se='inputSenha']"));

        inputEmail.Clear();
        if (tipoConta == "Cliente")
            inputEmail.SendKeys(emailCliente);
        else if (tipoConta == "Empresa")
            inputEmail.SendKeys(emailEmpresa);


        inputSenha.Clear();
        inputSenha.SendKeys(senhaPadrao);

        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(20));

        wait.Until(d =>
        {
            IWebElement btn = d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"));
            if (!btn.Enabled || !btn.Displayed) return false;
            btn.Click();
            return true;
        });

        wait.Until(d =>
            !d.Url.Contains("/autenticacao/registro", StringComparison.OrdinalIgnoreCase) &&
            d.FindElements(By.CssSelector("form[action='/autenticacao/registro']")).Count == 0
        );

        wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count > 0);
    }

    protected static void FazerLogout()
    {
        driver.Navigate().GoToUrl($"{enderecoBase}");

        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(5));

        wait.Until(d => d.FindElements(By.CssSelector("form[action='/autenticacao/logout']")).Count > 0);
        wait.Until(d => d.FindElement(By.CssSelector("form[action='/autenticacao/logout']"))).Submit();
    }
}