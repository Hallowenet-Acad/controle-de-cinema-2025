using ControleDeCinema.Aplicacao.ModuloSessao;
using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.Extensions.Logging;
using FizzWare.NBuilder;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloSala;

[TestClass]
[TestCategory("Testes de Unidade de Sessao")]
public sealed class SessaoAppServiceTests
{
    private Mock<ITenantProvider>? tenantProviderMock;
    private Mock<IRepositorioSessao> repositorioSessaoMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<SessaoAppService>>? loggerMock;

    private SessaoAppService? sessaoAppService;

    [TestInitialize]
    public void ConfigurarTeste()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioSessaoMock = new Mock<IRepositorioSessao>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<SessaoAppService>>();
        sessaoAppService = new SessaoAppService(
            tenantProviderMock.Object,
            repositorioSessaoMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
            );
    }


    [TestMethod]
    public void Deve_Cadastrar_Sessao_Corretamente()
    {
        var genero = new GeneroFilme("Ação");
        var filme = new Filme("Heat", 160, false, genero);
        var sala = new Sala(1, 100);
        var sessao = new Sessao(DateTime.Parse("05/07/2005 19:30:00"), 100, filme, sala);

        repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { });

        var resultado = sessaoAppService!.Cadastrar(sessao);

        repositorioSessaoMock?.Verify(r => r.Cadastrar(sessao), Times.Once);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Deve_Retornar_Falha_Caso_Exceda_Limite()
    {
        // Arrange
        var genero = new GeneroFilme("Ficção Cientifica");
        var filme = new Filme("Interestelar", 2, true, genero);
        var sala = new Sala(2, 40);
        var sessao = new Sessao(DateTime.UtcNow, 50, filme, sala);

        repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { });

        // Act
        var resultado = sessaoAppService!.Cadastrar(sessao);

        // Assert
        repositorioSessaoMock?.Verify(r => r.Cadastrar(sessao), Times.Never);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultado);
        Assert.IsFalse(resultado.IsSuccess);
    }

    [TestMethod]
    public void Deve_Retornar_Erro_Com_Sessoes_Em_Horarios_Iguais()
    {
        var genero = new GeneroFilme("Ficção Cientifica");
        var filme = new Filme("Interestelar", 2, true, genero);
        var sala = new Sala(2, 100);
        var sessao = new Sessao(DateTime.Parse("04/09/2025 11:00:00"), 50, filme, sala);

        var filme2 = new Filme("Silksong", 100, false, genero);
        var sessao2 = new Sessao(DateTime.Parse("04/09/2025 11:00:00"), 100, filme2, sala);

        repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { sessao });

        var resultado = sessaoAppService!.Cadastrar(sessao2);

        repositorioSessaoMock?.Verify(r => r.Cadastrar(sessao2), Times.Never);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultado);
        Assert.IsFalse(resultado.IsSuccess);
    }

    
}
