using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloGenero;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;

[TestClass]
[TestCategory("Testes de Interface de Filme")]
public sealed class FilmeInterfaceTestes : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Cadastrar_Filme_Corretamente()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
        .ClickSubmit();

        FilmeIndexPageObject filmeIndexPage = new(driver);

        FilmeFormPageObject filmeFormPage = filmeIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar();

        filmeFormPage
            .PreencherTitulo("A Hora do Mal")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Terror")
            .ClickSubmit();

        Assert.IsTrue(filmeIndexPage.ContemFilme("A Hora do Mal"));
    }

    [TestMethod]
    public void Deve_Editar_Filme_Corretamente()
    {

        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
        .ClickSubmit();

        FilmeIndexPageObject filmeIndexPage = new(driver);

        filmeIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherTitulo("A Hora do Mal")
            .PreencherDuracao(125)
            .MarcarLancamento()
            .SelecionarGenero("Terror")
            .ClickSubmit();

        FilmeFormPageObject filmeFormPage = filmeIndexPage
            .IrPara(enderecoBase)
            .ClickEditar();

        filmeFormPage
            .PreencherTitulo("A Hora do Bem")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Religião")
            .ClickSubmit();

        Assert.IsTrue(filmeIndexPage.ContemFilme("A Hora do Bem"));
    }

    [TestMethod]
    public void Deve_Excluir_Filme_Corretamente()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
        .ClickSubmit();

        FilmeIndexPageObject filmeIndexPage = new(driver);

        filmeIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherTitulo("Longlegs")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Terror")
            .ClickSubmit();

        FilmeFormPageObject filmeFormPage = filmeIndexPage
            .IrPara(enderecoBase)
            .ClickExcluir();

        filmeFormPage
            .ClickSubmitExcluir("Longlegs");

        Assert.IsFalse(filmeIndexPage.ContemFilme("Longlegs"));
    }

    [TestMethod]
    public void Deve_Visualizar_Filmes_Cadastrados_Corretamente()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
            .ClickSubmit();

        FilmeIndexPageObject filmeIndexPage = new(driver);

        filmeIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherTitulo("Pânico")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Terror")
            .ClickSubmit();

        filmeIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherTitulo("Nosferatu")
            .PreencherDuracao(135)
            .MarcarLancamento()
            .SelecionarGenero("Romance")
            .ClickSubmit();

        filmeIndexPage
            .IrPara(enderecoBase);

        Assert.IsTrue(filmeIndexPage.ContemFilme("Pânico"));
        Assert.IsTrue(filmeIndexPage.ContemFilme("Nosferatu"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Filme_Com_Campos_Vazios()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
            .ClickSubmit();

        FilmeIndexPageObject filmeIndexPage = new(driver);

        FilmeFormPageObject filmeFormPage = filmeIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar();

        filmeFormPage
            .ClickSubmitEsperandoErros();

        Assert.IsTrue(filmeFormPage.EstourouValidacao("Titulo"));
        Assert.IsTrue(filmeFormPage.EstourouValidacao("Duracao"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Filme_Com_Duracao_Invalida()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
            .ClickSubmit();

        FilmeIndexPageObject filmeIndexPage = new(driver);

        FilmeFormPageObject filmeFormPage = filmeIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar();

        filmeFormPage
            .PreencherTitulo("Esposa de Mentirinha")
            .PreencherDuracao(0)
            .MarcarLancamento()
            .SelecionarGenero("Comédia")
            .ClickSubmitEsperandoErros();

        Assert.IsTrue(filmeFormPage.EstourouValidacao("Duracao"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Filme_Com_Titulo_Duplicado()
    {
        GeneroIndexPageObject generoIndexPage = new(driver);

        generoIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Terror")
            .ClickSubmit();

        FilmeIndexPageObject filmeIndexPage = new(driver);

        filmeIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherTitulo("Faça Ela Voltar")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Terror")
            .ClickSubmit();

        FilmeFormPageObject filmeFormPage = filmeIndexPage
            .IrPara(enderecoBase)
            .ClickCadastrar();

        filmeFormPage
            .PreencherTitulo("Faça Ela Voltar")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Terror")
            .ClickSubmitEsperandoErros();

        Assert.IsTrue(filmeFormPage.EstourouValidacao());
    }
}
