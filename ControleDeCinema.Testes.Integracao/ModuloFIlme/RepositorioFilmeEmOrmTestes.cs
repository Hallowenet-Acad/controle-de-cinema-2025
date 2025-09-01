using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao.ModuloFIlme;

[TestClass]
[TestCategory("Testes de Integração de Filme")]
public sealed class RepositorioFilmeEmOrmTestes : TestFixture
{
    private GeneroFilme genero = null!;

    [TestInitialize]
    public override void ConfigurarTestes()
    {
        base.ConfigurarTestes();

        genero = Builder<GeneroFilme>.CreateNew()
            .With(d => d.Descricao = "Terror").Persist();
    }


    [TestMethod]
    public void Deve_Cadastrar_Filme_Corretamente()
    {
        Filme filme = new ("Bring Her Back", 105, true, genero);

        repositorioFilme.Cadastrar(filme);
        dbContext.SaveChanges();

        Filme filmeSelecionado = repositorioFilme.SelecionarRegistroPorId(filme.Id);
        Assert.AreEqual(filme, filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_Filme_Corretamente()
    {
        Filme filme = new ("Weapons", 105, true, genero);
        repositorioFilme.Cadastrar(filme);

        GeneroFilme novoGenero = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Id = Guid.NewGuid())
            .With(d => d.Descricao = "Animação").Persist();

        dbContext.SaveChanges();

        Filme filmeEditado = new("Nimona", 110, true, novoGenero);
        bool conseguiuEditar = repositorioFilme.Editar(filme.Id, filmeEditado);
        dbContext.SaveChanges();

        Filme filmeSelecionado = repositorioFilme.SelecionarRegistroPorId(filme.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(filme, filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Filme_Corretamente()
    {
        Filme filme = new("Brinquedo Assasino", 105, true, genero);
        repositorioFilme.Cadastrar(filme);
        dbContext.SaveChanges();

        bool conseguiuExcluir = repositorioFilme.Excluir(filme.Id);
        dbContext.SaveChanges();

        Filme filmeSelecionado = repositorioFilme.SelecionarRegistroPorId(filme.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Filme_Por_Id_Corretamente()
    {
        Filme filme = new("LongLegs", 120, true, genero);
        repositorioFilme.Cadastrar(filme);
        dbContext.SaveChanges();

        Filme filmeSelecionado = repositorioFilme.SelecionarRegistroPorId(filme.Id);

        Assert.AreEqual(filme, filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Filme_Corretamente()
    {

        List<GeneroFilme> novosGeneros = Builder<GeneroFilme>.CreateListOfSize(3)
            .All().With(g => g.Id = Guid.NewGuid()).Persist().ToList();

        List<Filme> novosFilmes = new()
        {
            new ("Sexta Feira 13", 105, true, genero),
            new ("Panico", 115, true, genero),
            new ("Halloween", 125, true, genero)
        };

        repositorioFilme.CadastrarEntidades(novosFilmes);
        dbContext.SaveChanges();

        List<Filme> filmesExistentes = repositorioFilme.SelecionarRegistros();
        List<Filme> filmesEsperados = novosFilmes;

        Assert.AreEqual(filmesEsperados.Count, filmesExistentes.Count);
        CollectionAssert.AreEquivalent(filmesEsperados, filmesExistentes);
    }
}
