using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Testes.Interface.Compartilhado;
using FizzWare.NBuilder;
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
        //    .PreencherDescricao("Ficção Científica")
        //    .Confirmar();

        //var filmeIndex = new FilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //filmeIndex
        //    .ClickCadastrar()
        //    .PreencherTitulo("Interestelar")
        //    .PreencherDuracao(120)
        //    .SelecionarGenero("Ficção Científica")
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
            .SelecionarFilme("Interestelar")
            .SelecionarSala(1)
            .Confirmar();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemSessao("Interestelar", horario));
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
        //    .PreencherDescricao("Ficção Científica")
        //    .Confirmar();

        //var filmeIndex = new FilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //filmeIndex
        //    .ClickCadastrar()
        //    .PreencherTitulo("Interestelar")
        //    .PreencherDuracao(120)
        //    .SelecionarGenero("Ficção Científica")
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
            .SelecionarFilme("Interestelar")
            .SelecionarSala(1)
            .Confirmar();

        // Act
        var horario = DateTime.Now.AddHours(6);

        sessaoIndex
            .ClickEditar()
            .PreencherDataHorarioInicio(horario)
            .PreencherNumeroMaximoIngressos(90)
            .SelecionarFilme("Interestelar")
            .SelecionarSala(1)
            .Confirmar();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemSessao("Interestelar", horario));
    }

    [TestMethod]
    public void Deve_Excluir_Sessao()
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
        //    .PreencherDescricao("Ficção Científica")
        //    .Confirmar();

        //var filmeIndex = new FilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //filmeIndex
        //    .ClickCadastrar()
        //    .PreencherTitulo("Interestelar")
        //    .PreencherDuracao(120)
        //    .SelecionarGenero("Ficção Científica")
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

        var horario = DateTime.Now.AddHours(5);

        sessaoIndex
            .ClickCadastrar()
            .PreencherDataHorarioInicio(horario)
            .PreencherNumeroMaximoIngressos(50)
            .SelecionarFilme("Interestelar")
            .SelecionarSala(1)
            .Confirmar();

        // Act
        sessaoIndex
            .ClickEncerrar()
            .Confirmar()
            .IrPara(enderecoBase!);

        sessaoIndex
            .ClickExcluir()
            .Confirmar();

        // Assert
        Assert.IsFalse(sessaoIndex.ContemSessao("Interestelar", horario));
    }

    [TestMethod]
    public void Deve_Visualizar_Detalhes_Sessao()
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
        //    .PreencherDescricao("Ficção Científica")
        //    .Confirmar();

        //var filmeIndex = new FilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //filmeIndex
        //    .ClickCadastrar()
        //    .PreencherTitulo("Interestelar")
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

        var horario = DateTime.Now.AddHours(5);

        sessaoIndex
            .ClickCadastrar()
            .PreencherDataHorarioInicio(horario)
            .PreencherNumeroMaximoIngressos(50)
            .SelecionarFilme("Interestelar")
            .SelecionarSala(1)
            .Confirmar();

        // Act
        sessaoIndex
            .ClickDetalhes();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemDetalhes("Interestelar", horario, 50));
    }

    [TestMethod]
    public void Deve_Validar_Campos_Obrigatorios_Sessao()
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
        //    .PreencherDescricao("Ficção Científica")
        //    .Confirmar();

        //var filmeIndex = new FilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //filmeIndex
        //    .ClickCadastrar()
        //    .PreencherTitulo("Interestelar")
        //    .PreencherDuracao(120)
        //    .SelecionarGenero("Ficção Científica")
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
            .SelecionarFilme("Interestelar")
            .SelecionarSala(1)
            .Confirmar();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemErroSpan("NumeroMaximoIngressos"));
    }

    [TestMethod]
    public void Deve_Validar_Horario_Já_Existente_Sessao()
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
        //    .PreencherDescricao("Ficção Científica")
        //    .Confirmar();

        //var filmeIndex = new FilmeIndexPageObject(driver!)
        //    .IrPara(enderecoBase!);

        //filmeIndex
        //    .ClickCadastrar()
        //    .PreencherTitulo("Interestelar")
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

        var horario = DateTime.Now.AddHours(5);

        sessaoIndex
            .ClickCadastrar()
            .PreencherDataHorarioInicio(horario)
            .PreencherNumeroMaximoIngressos(50)
            .SelecionarFilme("Interestelar")
            .SelecionarSala(1)
            .Confirmar();

        // Act
        sessaoIndex
            .ClickCadastrar()
            .PreencherDataHorarioInicio(horario.AddHours(1))
            .PreencherNumeroMaximoIngressos(50)
            .SelecionarFilme("Interestelar")
            .SelecionarSala(1)
            .Confirmar();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemErroAlert("Já existe uma sessão nesta sala que conflita com o horário informado."));
    }

    [TestMethod]
    public void Deve_Comprar_Ingresso_Sessao()
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
        //    .PreencherCapacidade(2)
        //    .Confirmar();

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        var horario = DateTime.Now.AddHours(5);

        sessaoIndex
            .ClickCadastrar()
            .PreencherDataHorarioInicio(horario)
            .PreencherNumeroMaximoIngressos(2)
            .SelecionarFilme("Interestelar")
            .SelecionarSala(1)
            .Confirmar();

        // Act
        //authIndex
        //    .ClickUsuario()
        //    .ClickLoggout();

        //authIndex
        //    .ClickCriarConta()
        //    .PreencherEmail("cliente@gmail.com")
        //    .PreencherSenha("Senha12345")
        //    .PreencherConfirmarSenha("Senha12345")
        //    .SelecionarTipoUsuario("Cliente")
        //    .Confirmar();

        sessaoIndex
            .IrPara(enderecoBase!)
            .ClickComprarIngresso()
            .Confirmar();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemIngresso("Interestelar"));
    }

    [TestMethod]
    public void Deve_Validar_Ingresso_Sessao_Lotada()
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
        //    .PreencherCapacidade(2)
        //    .Confirmar();

        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        var horario = DateTime.Now.AddHours(5);

        sessaoIndex
            .ClickCadastrar()
            .PreencherDataHorarioInicio(horario)
            .PreencherNumeroMaximoIngressos(2)
            .SelecionarFilme("John Wick")
            .SelecionarSala(1)
            .Confirmar();

        //authIndex
        //    .ClickUsuario()
        //    .ClickLoggout();

        //authIndex
        //    .ClickCriarConta()
        //    .PreencherEmail("cliente@gmail.com")
        //    .PreencherSenha("Senha12345")
        //    .PreencherConfirmarSenha("Senha12345")
        //    .SelecionarTipoUsuario("Cliente")
        //    .Confirmar();

        sessaoIndex
            .IrPara(enderecoBase!);

        // Act
        sessaoIndex
            .ClickComprarIngresso()
            .Confirmar();

        sessaoIndex
            .ClickComprarIngresso()
            .Confirmar();

        sessaoIndex
            .ClickComprarIngresso()
            .Confirmar();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemErroSpan("Assento"));
    }

    [TestMethod]
    public void Deve_Exibir_Contagem_Correta_De_Ingressos_Vendidos_Nos_Detalhes()
    {
        // Arrange
        var sessaoIndex = new SessaoIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        var horario = DateTime.Now.AddHours(3);
        var nomeFilme = "Duna"; 

        sessaoIndex
            .ClickCadastrar()
            .PreencherDataHorarioInicio(horario)
            .PreencherNumeroMaximoIngressos(10)
            .SelecionarFilme(nomeFilme) 
            .SelecionarSala(1)         
            .Confirmar();

        sessaoIndex
            .ClickComprarIngressoPorFilme(nomeFilme) 
            .Confirmar();

        sessaoIndex
            .IrPara(enderecoBase!) 
            .ClickComprarIngressoPorFilme(nomeFilme)
            .Confirmar();

        // Act
        sessaoIndex
            .IrPara(enderecoBase!)
            .ClickDetalhesPorFilme(nomeFilme); 


        // Assert
        Assert.IsTrue(sessaoIndex.ContemInformacaoDeIngressos("8 / 10"));
    }

    [TestMethod]
    public void Deve_Exibir_Contagem_Correta_De_Sessoes_Por_Filme_Na_Lista()
    {

        var filmeDuna = Builder<Filme>.CreateNew()
            .With(f => f.Titulo = "Duna: Parte 2")
            .Build();

        var filmeOppenheimer = Builder<Filme>.CreateNew()
            .With(f => f.Titulo = "Oppenheimer")
            .Build();

        var sala = Builder<Sala>.CreateNew()
            .With(s => s.Numero = 1)
            .Build();

        var sessoes = Builder<Sessao>.CreateListOfSize(3)
            .TheFirst(1)
                .With(s => s.Filme = filmeDuna)
                .With(s => s.Sala = sala)
                .With(s => s.Inicio = DateTime.Now.AddDays(1).Date.AddHours(18))

            .TheNext(1)
                .With(s => s.Filme = filmeDuna)
                .With(s => s.Sala = sala)
                .With(s => s.Inicio = DateTime.Now.AddDays(1).Date.AddHours(21))

            .TheNext(1)
                .With(s => s.Filme = filmeOppenheimer)
                .With(s => s.Sala = sala)
                .With(s => s.Inicio = DateTime.Now.AddDays(1).Date.AddHours(20))
            .Build();

        dbContext?.Filmes.Add(filmeDuna);
        dbContext?.Filmes.Add(filmeOppenheimer);
        dbContext?.Salas.Add(sala);
        dbContext?.Sessoes.AddRange(sessoes);
        dbContext?.SaveChanges();

        var sessaoIndex = new SessaoIndexPageObject(driver!);

        sessaoIndex.IrPara(enderecoBase!);

        Assert.AreEqual(2, sessaoIndex.ContarCardsDeSessaoParaFilme(filmeDuna.Titulo));
        Assert.AreEqual(1, sessaoIndex.ContarCardsDeSessaoParaFilme(filmeOppenheimer.Titulo));
    }
}