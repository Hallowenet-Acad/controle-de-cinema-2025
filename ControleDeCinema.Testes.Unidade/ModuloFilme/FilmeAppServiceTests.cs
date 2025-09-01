using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FluentResults;
using Moq;
using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Dominio.ModuloFilme;
using Microsoft.Extensions.Logging;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Unidade.ModuloFilme;

[TestClass]
[TestCategory("Testes de Unidade de Filme")]
public sealed class FilmeAppServiceTests
{
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioFilme> repositorioFilmeMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<FilmeAppService>>? loggerMock;

    private FilmeAppService filmeAppService;

    private static GeneroFilme genero = Builder<GeneroFilme>.CreateNew()
    .With(d => d.Id = Guid.NewGuid())
    .With(d => d.Descricao = "Terror")
    .Build();

    [TestInitialize]
    public void setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioFilmeMock = new Mock<IRepositorioFilme>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<FilmeAppService>>();

        filmeAppService = new FilmeAppService(
            tenantProviderMock.Object,
            repositorioFilmeMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
            );
    }

    [TestMethod]
    public void Cadastrar_Filme_Retorna_Correto()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>());

        Result resultado = filmeAppService.Cadastrar(filme);

        repositorioFilmeMock?.Verify(r => r.Cadastrar(filme), Times.Once);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }


    [TestMethod]
    public void Cadastrar_Filme_Retorna_Falha_Quando_Duplicado()
    {
        Filme filme = new("LongLegs", 125, true, genero);
        Filme filmeDuplicado = new("LongLegs", 125, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { filme });

        Result resultado = filmeAppService.Cadastrar(filme);

        repositorioFilmeMock?.Verify(r => r.Cadastrar(filme), Times.Never);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro duplicado", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Cadastrar_Filme_Retorna_Falha_Quando_Houver_Excecao()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>());

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro no cadastro"));

        Result resultado = filmeAppService.Cadastrar(filme);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        Assert.IsNotNull(resultado);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_Filme_Retorna_Correto()
    {
        GeneroFilme novoGenero = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Id = Guid.NewGuid())
            .With(d => d.Descricao = "Thriller").Build();

        Filme filme = new("LongLegs", 105, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { filme });

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        Filme filmeEditado = new("LONGLEGS-VÍNCULO MORTAL", 125, true, novoGenero);

        Result resultado = filmeAppService.Editar(filme.Id, filmeEditado);

        repositorioFilmeMock.Verify(r => r.Editar(filme.Id, filmeEditado), Times.Once);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Editar_Filme_Retorna_Falha_Quando_Duplicado()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        List<Filme> filmeExistentes = new()
        {
            filme,
            new("Nosferatu", 135, true, genero)
        };

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(filmeExistentes);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        Filme filmeEditado = new("Nosferatu", 125, true, genero);

        Result resultado = filmeAppService.Editar(filme.Id, filmeEditado);

        repositorioFilmeMock.Verify(r => r.Editar(filme.Id, filmeEditado), Times.Never);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro duplicado", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_Filme_Retorna_Falha_Quando_Houver_Excecao()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { filme });

        Filme filmeEditado = new("ShortLegs", 125, true, genero);

        repositorioFilmeMock
         .Setup(r => r.Editar(genero.Id, filmeEditado))
         .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro na edição"));

        Result resultadoEdicao = filmeAppService.Editar(filme.Id, filmeEditado);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        Assert.IsNotNull(resultadoEdicao);

        string mensagemErro = resultadoEdicao.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultadoEdicao.IsFailed);
    }

    [TestMethod]
    public void Excluir_Filme_Retorna_Correto()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { filme });

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        repositorioFilmeMock
            .Setup(r => r.Excluir(filme.Id))
            .Returns(true);

        Result resultado = filmeAppService.Excluir(filme.Id);

        repositorioFilmeMock.Verify(r => r.Excluir(filme.Id), Times.Once);
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }
    [TestMethod]
    public void Excluir_Filme_Inexistente_Retorna_Falha()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { filme });

        repositorioFilmeMock
            .Setup(r => r.Excluir(Guid.NewGuid()))
            .Returns(false);

        Result resultado = filmeAppService.Excluir(Guid.NewGuid());

        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro não encontrado", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Excluir_Filme_Retorna_Falha_Quando_Houver_Excecao()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { filme });

        repositorioFilmeMock
            .Setup(r => r.Excluir(filme.Id))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro na exclusão"));

        Result resultadoExclusao = filmeAppService.Excluir(filme.Id);

        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
        Assert.IsNotNull(resultadoExclusao);

        string mensagemErro = resultadoExclusao.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultadoExclusao.IsFailed);
    }

    [TestMethod]
    public void Selecionar_Filme_Por_Id_Retorna_Correto()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        Result<Filme> resultado = filmeAppService.SelecionarPorId(filme.Id);
        Filme filmeSelecionado = resultado.ValueOrDefault;

        repositorioFilmeMock.Verify(r => r.SelecionarRegistroPorId(filme.Id), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(filme, filmeSelecionado);
    }

    [TestMethod]
    public void Selecionar_Filme_Por_Id_Inexistente_Retorna_Falha()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        Result<Filme> resultado = filmeAppService.SelecionarPorId(Guid.NewGuid());
        Filme filmeSelecionado = resultado.ValueOrDefault;

        repositorioFilmeMock.Verify(r => r.SelecionarRegistroPorId(filme.Id), Times.Never);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Registro não encontrado", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreNotEqual(filme, filmeSelecionado);
    }


    [TestMethod]
    public void Selecionar_Filme_Por_Id_Retorna_Falha_Quando_Houver_Excecao()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Throws(new Exception("Erro inesperado"));

        Result<Filme> resultado = filmeAppService.SelecionarPorId(filme.Id);
        Filme generoSelecionado = resultado.ValueOrDefault;

        repositorioFilmeMock.Verify(r => r.SelecionarRegistroPorId(filme.Id), Times.Once);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Selecionar_Todos_Filme_Retorna_Correto()
    {
        Filme filme = new("Bebê de Rosemary", 125, true, genero);

        List<Filme> filmeExistentes = new()
        {
            filme,
            new("Nosferatu", 135, true, genero)
        };

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(filmeExistentes);

        Result<List<Filme>> resultado = filmeAppService.SelecionarTodos();
        List<Filme> filmeSelecionado = resultado.ValueOrDefault;

        repositorioFilmeMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
        CollectionAssert.AreEquivalent(filmeExistentes, filmeSelecionado);
    }

    [TestMethod]
    public void Selecionar_Todos_Filme_Retorna_Falha_Quando_Houver_Excecao()
    {
        Filme filme = new("LongLegs", 125, true, genero);

        List<Filme> filmeExistentes = new()
        {
            filme,
            new("Nosferatu", 135, true, genero)
        };

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro inesperado"));

        Result<List<Filme>> resultado = filmeAppService.SelecionarTodos();
        List<Filme> generoSelecionado = resultado.ValueOrDefault;

        repositorioFilmeMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        string mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }
}
