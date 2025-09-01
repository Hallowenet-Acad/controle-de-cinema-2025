using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloIngresso;
using ControleDeCinema.Testes.Interface.ModuloSala;
using FizzWare.NBuilder;
namespace ControleDeCinema.Testes.Interface.ModuloSessao;

[TestClass]
[TestCategory("Testes de interface de Sessão")]
public sealed class SessaoInterfaceTests : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Cadastrar_Sessao()
    {
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ficção Científica")
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDataHorarioInicio(DateTime.UtcNow)
            .PreencherNumeroMaximoIngressos(100)
            .SelecionarFilme(filme.Titulo)
            .SelecionarSala(sala.Numero)
            .Confirmar();

        Assert.IsTrue(sessaoIndex.ContemSessao(filme.Titulo, DateTime.UtcNow));
    }

    [TestMethod]
    public void Deve_Editar_Sessao()
    {
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ficção Científica")
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        SessaoIndexPageObject sessaoIndex = new(driver!);
        
        sessaoIndex.IrPara(enderecoBase!)
            .ClickCadastrar()
            .PreencherDataHorarioInicio(DateTime.UtcNow)
            .PreencherNumeroMaximoIngressos(50)
            .SelecionarFilme(filme.Titulo)
            .SelecionarSala(sala.Numero)
            .Confirmar();

        SessaoFormPageObject sessaoForm = sessaoIndex
            .IrPara(enderecoBase)
            .ClickEditar();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemSessao(filme.Titulo, DateTime.UtcNow));
    }

    [TestMethod]
    public void Deve_Excluir_Sessao()
    {
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ficção Científica")
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        SessaoIndexPageObject sessaoIndex = new(driver!);
        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDataHorarioInicio(DateTime.UtcNow)
            .PreencherNumeroMaximoIngressos(50)
            .SelecionarFilme(filme.Titulo)
            .SelecionarSala(sala.Numero)
            .Confirmar();

        // Act
        SessaoFormPageObject sessaoForm = sessaoIndex
            .IrPara(enderecoBase)
            .ClickEncerrar()
            .ClickSubmitEncerrar()
            .ClickExcluir();

        sessaoForm
            .ClickSubmitExcluir(filme.Titulo);

        // Assert
        Assert.IsFalse(sessaoIndex.ContemSessao(filme.Titulo, DateTime.UtcNow));
    }

    [TestMethod]
    public void Deve_Visualizar_Detalhes_Sessao()
    {
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ficção Científica")
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        SessaoIndexPageObject sessaoIndex = new(driver!);
            
        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDataHorarioInicio(DateTime.UtcNow)
            .PreencherNumeroMaximoIngressos(50)
            .SelecionarFilme(filme.Titulo)
            .SelecionarSala(sala.Numero)
            .Confirmar();

        // Act
        sessaoIndex
            .IrPara(enderecoBase)
            .ClickDetalhes();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemDetalhes(filme.Titulo, DateTime.UtcNow, 50));
    }

    [TestMethod]
    public void Deve_Validar_Campos_Obrigatorios_Sessao()
    {
        SessaoFormPageObject sessaoForm = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase)
            .ClickCadastrar();

        sessaoForm
            .ClickSubmitEsperandoErros();

        Assert.IsTrue(sessaoForm.EstourouValidacao("NumeroMaximoIngressos"));
        Assert.IsTrue(sessaoForm.EstourouValidacao("FilmeId"));
        Assert.IsTrue(sessaoForm.EstourouValidacao("SalaId"));
    }

    [TestMethod]
    public void Deve_Validar_Horario_Já_Existente_Sessao()
    {
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ficção Científica")
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDataHorarioInicio(DateTime.UtcNow)
            .PreencherNumeroMaximoIngressos(100)
            .SelecionarFilme(filme.Titulo)
            .SelecionarSala(sala.Numero)
            .Confirmar();

        SessaoFormPageObject sessaoForm = sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        sessaoForm
            .PreencherDataHorarioInicio(DateTime.UtcNow)
            .PreencherNumeroMaximoIngressos(50)
            .SelecionarFilme(filme.Titulo)
            .SelecionarSala(5)
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(sessaoForm.EstourouValidacao());
    }

    [TestMethod]
    public void Deve_Comprar_Ingresso_Sessao()
    {
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ficção Científica")
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDataHorarioInicio(DateTime.UtcNow)
            .PreencherNumeroMaximoIngressos(100)
            .SelecionarFilme(filme.Titulo)
            .SelecionarSala(sala.Numero)
            .Confirmar();

        FazerLogout();
        RegistrarContaCliente();

        SessaoFormPageObject _ = sessaoIndex
            .IrPara(enderecoBase)
            .ClickComprarIngresso();

        IngressoFormPageObject ingressoPage = new(driver);
        ingressoPage
            .SelecionarAssento(1)
            .MarcarMeiaEntrada()
            .ClickSubmitComoCliente();

        FazerLogout();
        FazerLogin("Empresa");

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickDetalhes();

        Assert.IsTrue(sessaoIndex.ContemIngressosVendidos(1));
        Assert.IsTrue(sessaoIndex.ContemIngressosDisponiveis(9));
        
    }

    [TestMethod]
    public void Deve_Validar_Ingresso_Sessao_Lotada()
    {
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ficção Científica")
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDataHorarioInicio(DateTime.UtcNow)
            .PreencherNumeroMaximoIngressos(100)
            .SelecionarFilme(filme.Titulo)
            .SelecionarSala(sala.Numero)
            .Confirmar();

        FazerLogout();
        RegistrarContaCliente();

        SessaoFormPageObject _ = sessaoIndex
            .IrPara(enderecoBase)
            .ClickComprarIngresso();

        sessaoIndex = new IngressoFormPageObject(driver)
            .SelecionarAssento(1)
            .ClickSubmitComoCliente();

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickDetalhes();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemIngressosDisponiveis(1));
    }

    [TestMethod]
    public void Deve_Exibir_Contagem_Correta_De_Sessoes_Por_Filme_Na_Lista()
    {
        var generoFilme = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ficção Científica")
            .Persist();

        var generoFilme2 = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Descricao = "Ação")
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme)
            .Persist();

        var filme2 = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilme2)
            .Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDataHorarioInicio(DateTime.UtcNow)
            .PreencherNumeroMaximoIngressos(100)
            .SelecionarFilme(filme.Titulo)
            .SelecionarSala(sala.Numero)
            .Confirmar();

        FazerLogout();
        RegistrarContaCliente();

        IngressoIndexPageObject ingressoIndex = new IngressoIndexPageObject(driver)
            .IrPara(enderecoBase);

        Assert.IsTrue(ingressoIndex.ContemFilme(filme.Titulo));
        Assert.IsTrue(ingressoIndex.ContemFilme(filme2.Titulo));
    }
}