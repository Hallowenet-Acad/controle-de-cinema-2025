using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Testes.Integracao.Compartilhado;

namespace ControleDeCinema.Testes.Integracao.ModuloGenero;

[TestClass]
[TestCategory("Testes de Integração de Genêro")]
public sealed class RepositorioGeneroEmOrmTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Genero_Corretamente()
    {
        GeneroFilme genero = new ("Horror");

        repositorioGenero.Cadastrar(genero);
        dbContext.SaveChanges();

        GeneroFilme generoSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);
        Assert.AreEqual(genero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_Genero_Corretamente()
    {
        GeneroFilme genero = new ("Terror");
        repositorioGenero.Cadastrar(genero);
        dbContext.SaveChanges();

        GeneroFilme generoEditada = new ("Suspense");

        bool conseguiuEditar = repositorioGenero.Editar(genero.Id, generoEditada);
        dbContext.SaveChanges();

        GeneroFilme generoSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(genero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Genero_Corretamente()
    {
        GeneroFilme genero = new GeneroFilme("Romance");
        repositorioGenero.Cadastrar(genero);
        dbContext.SaveChanges();

        var conseguiuExcluir = repositorioGenero.Excluir(genero.Id);
        dbContext.SaveChanges();

        GeneroFilme generoSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(generoSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Genero_Por_Id_Corretamente()
    {
        GeneroFilme genero = new("Comédia");
        repositorioGenero.Cadastrar(genero);
        dbContext.SaveChanges();

        GeneroFilme generoSelecionado = repositorioGenero.SelecionarRegistroPorId(genero.Id);

        Assert.AreEqual(genero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Genero_Corretamente()
    {
        List<GeneroFilme> novosGeneros = new();
        {
            GeneroFilme genero = new ("Terror");
            GeneroFilme genero2 = new ("Ação");
            GeneroFilme genero3 = new ("Suspense");
        }

        repositorioGenero.CadastrarEntidades(novosGeneros);
        dbContext.SaveChanges();

        List<GeneroFilme> generosExistentes = repositorioGenero.SelecionarRegistros();
        List<GeneroFilme> generosEsperados = novosGeneros;

        Assert.AreEqual(generosEsperados.Count, generosExistentes.Count);
        CollectionAssert.AreEquivalent(generosEsperados, generosExistentes);
    }
}
