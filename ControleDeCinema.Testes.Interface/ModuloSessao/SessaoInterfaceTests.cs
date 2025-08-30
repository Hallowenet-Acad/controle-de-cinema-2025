using ControleDeCinema.Testes.Interface.ModuloSala;
using ControleDeCinema.Testes.Interface.Compartilhado;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;

[TestClass]
[TestCategory("Testes de interface de Sessão")]
public sealed class SessaoInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sessao()
    {
        // Arrange
        //var authIndex = new AutentificacaoIndexPageObject(driver!)
        //    .IrParaLogin(enderecoBase!)
        //    .ClickCriarConta()
        //    .PreencherEmail("cinema@gmail.com")
        //    .PreencherSenha("Senha12345")
        //    .PreencherConfirmarSenha("Senha12345")
        //    .SelecionarTipoUsuario("Empresa")
        //    .Confirmar();

        //var generoFilmeIndex = new GeneroFilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //generoFilmeIndex
        //    .ClickCadastrar()
        //    .PreencherDescricao("Ação")
        //    .Confirmar();

        //var filmeIndex = new FilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //filmeIndex
        //    .ClickCadastrar()
        //    .PreencherTitulo("John Wick")
        //    .PreencherDuracao(120)
        //    .SelecionarGenero("Ação")
        //    .Confirmar();

        //var salaIndex = new SalaIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //salaIndex
        //    .ClickCadastrar()
        //    .PreencherNumeroSala(1)
        //    .PreencherCapacidade(100)
        //    .Confirmar();

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        // Act
        var horario = DateTime.Now.AddHours(5);

        sessaoIndex
            .ClickCadastrar()
            .PreencherDataHorarioInicio(horario)
            .PreencherNumeroMaximoIngressos(50)
            .SelecionarFilme("John Wick")
            .SelecionarSala(1)
            .Confirmar();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemSessao("John Wick", horario));
    }

    [TestMethod]
    public void Deve_Editar_Sessao()
    {
        // Arrange
        //var authIndex = new AutentificacaoIndexPageObject(driver!)
        //    .IrParaLogin(enderecoBase!)
        //    .ClickCriarConta()
        //    .PreencherEmail("cinema@gmail.com")
        //    .PreencherSenha("Senha12345")
        //    .PreencherConfirmarSenha("Senha12345")
        //    .SelecionarTipoUsuario("Empresa")
        //    .Confirmar();

        //var generoFilmeIndex = new GeneroFilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //generoFilmeIndex
        //    .ClickCadastrar()
        //    .PreencherDescricao("Ação")
        //    .Confirmar();

        //var filmeIndex = new FilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //filmeIndex
        //    .ClickCadastrar()
        //    .PreencherTitulo("John Wick")
        //    .PreencherDuracao(120)
        //    .SelecionarGenero("Ação")
        //    .Confirmar();

        //var salaIndex = new SalaIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //salaIndex
        //    .ClickCadastrar()
        //    .PreencherNumeroSala(1)
        //    .PreencherCapacidade(100)
        //    .Confirmar();

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        sessaoIndex
            .ClickCadastrar()
            .PreencherDataHorarioInicio(DateTime.Now.AddHours(5))
            .PreencherNumeroMaximoIngressos(100)
            .SelecionarFilme("John Wick")
            .SelecionarSala(1)
            .Confirmar();

        // Act
        var horario = DateTime.Now.AddHours(6);

        sessaoIndex
            .ClickEditar()
            .PreencherDataHorarioInicio(horario)
            .PreencherNumeroMaximoIngressos(90)
            .SelecionarFilme("John Wick")
            .SelecionarSala(1)
            .Confirmar();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemSessao("John Wick", horario));
    }

    
}