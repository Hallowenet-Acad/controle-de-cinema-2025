using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ControleDeCinema.Testes.Interface.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected static IWebDriver? driver;
    protected static ControleDeCinemaDbContext? dbContext;

    protected static string enderecoBase = "https://localhost:7131";
    private static string connectionString = "Host=localhost;Port=5433;Database=ControleDeCinemaDb;Username=postgres;Password=YourStrongPassword";

    [AssemblyInitialize]
    public static void ConfigurarTestes(TestContext _)
    {
        dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(connectionString);

        ConfigurarTabelas(dbContext);

        InicializarWebDriver();
    }

    [AssemblyCleanup]
    public static void EncerrarTestes()
    {
        EncerrarWebDriver();
    }
    
    [TestInitialize]
    public void InicializarTeste()
    {
        dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(connectionString);

        ConfigurarTabelas(dbContext);
    }

    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Salas.RemoveRange(dbContext.Salas);
        dbContext.Sessoes.RemoveRange(dbContext.Sessoes);
        dbContext.Ingressos.RemoveRange(dbContext.Ingressos);

        dbContext.SaveChanges;
    }
    
    private static void InicializarWebDriver()
    {
        var options = new ChromeOptions();
        options.AddArgument("-headless=new");

        driver = new ChromeDriver(options);
    }
    
    private static void EncerrarWebDriver()
    {
        driver?.Quit();
        driver?.Dispose();
    }
}
