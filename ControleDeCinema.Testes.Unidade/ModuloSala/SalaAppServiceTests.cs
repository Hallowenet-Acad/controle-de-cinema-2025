using Castle.Core.Logging;
using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSala;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSala;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloSala;

[TestClass]
[TestCategory("Testes de Unidade de Disciplina")]
public class SalaAppServiceTests
{
    private Mock<ITenantProvider>? tenantProviderMock;
    private Mock<IRepositorioSala>? repositorioSalaMock;
    private Mock<IUnitOfWork>? unitOfWorkMock;
    private Mock<ILogger<SalaAppService>>? loggerMock;

    private SalaAppService? salaAppService;

    [TestInitialize]
    public void ConfigurarTeste()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioSalaMock = new Mock<IRepositorioSala>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<SalaAppService>>();
        salaAppService = new SalaAppService(
            tenantProviderMock.Object,
            repositorioSalaMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
            );
    }

    [TestMethod]
    public void Deve_Cadastrar_Sala_Com_Sucesso()
    {
        var sala = new Sala(1, 50);

        repositorioSalaMock?.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>());

        var resultado = salaAppService!.Cadastrar(sala);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioSalaMock?.Verify(r => r.Cadastrar(sala), Times.Once);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);
    }
}
