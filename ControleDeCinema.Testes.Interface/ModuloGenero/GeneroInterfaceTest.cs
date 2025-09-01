using ControleDeCinema.Testes.Interface.Compartilhado;

namespace ControleDeCinema.Testes.Interface.ModuloGenero;

[TestClass]
[TestCategory("Teste de Interface de Gênero")]
public sealed class GeneroInterfaceTest : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Cadastrar_Genero_Correto()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase);

        GeneroFormPageObject generoFormPage = generoIndexPage
            .ClickCadastrar();

        generoFormPage
            .PreencherDescricao("Terror")
            .ClickSubmit();

        Assert.IsTrue(generoIndexPage.ContemGenero("Terror"));
    }

    [TestMethod]
    public void Deve_Editar_Genero_Correto()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
            .ClickSubmit();

        GeneroFormPageObject generoFormPage = generoIndexPage
            .IrPara(enderecoBase)
            .ClickEditar();

        generoFormPage
            .PreencherDescricao("Comédia")
            .ClickSubmit();

        Assert.IsTrue(generoIndexPage.ContemGenero("Comédia"));
    }

    [TestMethod]
    public void Deve_Excluir_GeneroFilme_Corretamente()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
            .ClickSubmit();

        GeneroFormPageObject generoFormPage = generoIndexPage
            .IrPara(enderecoBase)
            .ClickExcluir();

        generoFormPage
            .ClickSubmitExcluir("Românce");

        Assert.IsFalse(generoIndexPage.ContemGenero("Românce"));
    }

    [TestMethod]
    public void Deve_Visualizar_GenerosFilme_Cadastrados_Corretamente()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
            .ClickSubmit();

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Românce")
            .ClickSubmit();

        generoIndexPage
            .IrPara(enderecoBase);

        Assert.IsTrue(generoIndexPage.ContemGenero("Terror"));
        Assert.IsTrue(generoIndexPage.ContemGenero("Românce"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_GeneroFilme_Com_Campos_Vazios()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase);

        GeneroFormPageObject generoFormPage = generoIndexPage
            .ClickCadastrar();

        generoFormPage
            .ClickSubmitEsperandoErros();

        Assert.IsTrue(generoFormPage.EstourouValidacao("Descricao"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_GeneroFilme_Com_Descricao_Duplicada()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
            .ClickSubmit();

        GeneroFormPageObject generoFormPage = generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar();

        generoFormPage
            .PreencherDescricao("Terror")
            .ClickSubmitEsperandoErros();

        Assert.IsTrue(generoFormPage.EstourouValidacao());
    }
}
