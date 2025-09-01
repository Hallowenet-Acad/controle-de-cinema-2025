using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloGenero;

[TestClass]
[TestCategory("Testes de Unidade de Genêro")]
public sealed class GeneroAppServiceTests
{
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioGeneroFilme> repositorioGeneroFilmeMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<GeneroFilmeAppService>> loggerMock;

    private GeneroFilmeAppService generoAppService;

    [TestInitialize]
    public void setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioGeneroFilmeMock = new Mock<IRepositorioGeneroFilme>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<GeneroFilmeAppService>>();

        generoAppService = new GeneroFilmeAppService(
            tenantProviderMock.Object,
            repositorioGeneroFilmeMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
            );
    }

    [TestMethod]
    public void Cadastrar_Genero_Retorna_Correto()
    {
        GeneroFilme genero = new("Terror");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>());

        Result resultado = generoAppService.Cadastrar(genero);

        repositorioGeneroFilmeMock?.Verify(r => r.Cadastrar(genero), Times.Once);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }


    [TestMethod]
    public void Cadastrar_Genero_Retorna_Falha_Quando_Duplicado()
    {
        GeneroFilme genero = new("Suspense");
        GeneroFilme generoDuplicado = new("Suspense");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoDuplicado });

        Result resultado = generoAppService.Cadastrar(genero);

        repositorioGeneroFilmeMock?.Verify(r => r.Cadastrar(genero), Times.Never);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro duplicado", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Cadastrar_Genero_Retorna_Falha_Quando_Houver_Excecao()
    {
        GeneroFilme genero = new("Suspense");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>());

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro no cadastro"));

        Result resultado = generoAppService.Cadastrar(genero);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        Assert.IsNotNull(resultado);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_Genero_Retorna_Correto()
    {
        GeneroFilme genero = new("Terror");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { genero });

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        GeneroFilme generoEditado = new("Terror");

        Result resultado = generoAppService.Editar(genero.Id, generoEditado);

        repositorioGeneroFilmeMock.Verify(r => r.Editar(genero.Id, generoEditado), Times.Once);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Editar_Genero_Retorna_Falha_Quando_Duplicado()
    {
        GeneroFilme genero = new("Terror");

        List<GeneroFilme> listaGeneros = new()
        {
            genero,
            new("Animação")
        };

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(listaGeneros);

        GeneroFilme generoEditado = new("Animação");

        Result resultado = generoAppService.Editar(genero.Id, generoEditado);

        repositorioGeneroFilmeMock.Verify(r => r.Editar(genero.Id, generoEditado), Times.Never);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro duplicado", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_Genero_Retorna_Falha_Quando_Houver_Excecao()
    {
        GeneroFilme genero = new("Terror");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { genero });

        GeneroFilme generoEditado = new("Terror Slasher");

        repositorioGeneroFilmeMock
         .Setup(r => r.Editar(genero.Id, generoEditado))
         .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro na edição"));

        Result resultadoEdicao = generoAppService.Editar(genero.Id, generoEditado);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        Assert.IsNotNull(resultadoEdicao);

        string mensagemErro = resultadoEdicao.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultadoEdicao.IsFailed);
    }

    [TestMethod]
    public void Excluir_Genero_Retorna_Correto()
    {
        GeneroFilme genero = new("Terror");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { genero });

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        repositorioGeneroFilmeMock
            .Setup(r => r.Excluir(genero.Id))
            .Returns(true);

        Result resultado = generoAppService.Excluir(genero.Id);

        repositorioGeneroFilmeMock.Verify(r => r.Excluir(genero.Id), Times.Once);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }
    [TestMethod]
    public void Excluir_Genero_Inexistente_Retorna_Falha()
    {
        GeneroFilme genero = new("Terror");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { genero });

        repositorioGeneroFilmeMock
            .Setup(r => r.Excluir(Guid.NewGuid())) 
            .Returns(false);

        Result resultado = generoAppService.Excluir(Guid.NewGuid());

        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro não encontrado", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Excluir_Genero_Retorna_Falha_Quando_Houver_Excecao()
    {
        GeneroFilme genero = new("Terror");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { genero });

        repositorioGeneroFilmeMock
            .Setup(r => r.Excluir(genero.Id))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro na exclusão"));

        Result resultadoExclusao = generoAppService.Excluir(genero.Id);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        Assert.IsNotNull(resultadoExclusao);

        string mensagemErro = resultadoExclusao.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultadoExclusao.IsFailed);
    }

    [TestMethod]
    public void Selecionar_Genero_Por_Id_Retorna_Correto()
    {
        GeneroFilme genero = new("Terror");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        Result<GeneroFilme> resultado = generoAppService.SelecionarPorId(genero.Id);
        GeneroFilme generoSelecionado = resultado.ValueOrDefault;

        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistroPorId(genero.Id), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(genero, generoSelecionado);
    }

    [TestMethod]
    public void Selecionar_Genero_Por_Id_Inexistente_Retorna_Falha()
    {
        GeneroFilme genero = new("Terror");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Returns(genero);

        Result<GeneroFilme> resultado = generoAppService.SelecionarPorId(Guid.NewGuid());
        GeneroFilme generoSelecionado = resultado.ValueOrDefault;

        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistroPorId(genero.Id), Times.Never);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro não encontrado", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreNotEqual(genero, generoSelecionado);
    }


    [TestMethod]
    public void Selecionar_Genero_Por_Id_Retorna_Falha_Quando_Houver_Excecao()
    {
        GeneroFilme genero = new("Terror");

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(genero.Id))
            .Throws(new Exception("Erro inesperado"));

        Result<GeneroFilme> resultado = generoAppService.SelecionarPorId(genero.Id);
        GeneroFilme generoSelecionado = resultado.ValueOrDefault;

        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistroPorId(genero.Id), Times.Once);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Selecionar_Todos_Genero_Retorna_Correto()
    {
        GeneroFilme genero = new("Terror");

        List<GeneroFilme> generoExistentes = new()
        {
            genero,
            new("Animação")
        };

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(generoExistentes);

        Result<List<GeneroFilme>> resultado = generoAppService.SelecionarTodos();
        List<GeneroFilme> generoSelecionado = resultado.ValueOrDefault;

        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
        CollectionAssert.AreEquivalent(generoExistentes, generoSelecionado);
    }

    [TestMethod]
    public void Selecionar_Todos_Genero_Retorna_Falha_Quando_Houver_Excecao()
    {
        GeneroFilme genero = new("Terror");

        List<GeneroFilme> generoExistentes = new()
        {
            genero,
            new("Animação")
        };

        repositorioGeneroFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro inesperado"));

        Result<List<GeneroFilme>> resultado = generoAppService.SelecionarTodos();
        List<GeneroFilme> generoSelecionado = resultado.ValueOrDefault;                                                                     

        repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }
}
