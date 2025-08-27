using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ControleDeCinema.Testes.Integracao.ModuloIngresso;

[TestClass]
[TestCategory("Testes de Integração de Ingresso")]
public sealed class RepositorioIngressoEmOrmTests
{
    private ControleDeCinemaDbContext dbContext;
    private RepositorioIngressoEmOrm repositorioIngresso;
    private RepositorioSessaoEmOrm repositorioSessao;
    private RepositorioSalaEmOrm repositorioSala;
    private RepositorioFilmeEmOrm repositorioFilme;
    private RepositorioGeneroFilmeEmOrm repositorioGeneroFilme;

    [TestInitialize]
    public void ConfigurarTestes()
    {
        var assembly = typeof(RepositorioIngressoEmOrmTests).Assembly;

        var configuracao = new ConfigurationBuilder()
            .AddUserSecrets(assembly)
            .Build();

        var connectionString = configuracao["SQL_CONNECTION_STRING"];

        var options = new DbContextOptionsBuilder<ControleDeCinemaDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        dbContext = new ControleDeCinemaDbContext(options);
        repositorioSala = new RepositorioSalaEmOrm(dbContext);
        repositorioFilme = new RepositorioFilmeEmOrm(dbContext);
        repositorioSessao = new RepositorioSessaoEmOrm(dbContext);
        repositorioGeneroFilme = new RepositorioGeneroFilmeEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Sala>(repositorioSala.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<Filme>(repositorioFilme.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<Sessao>(repositorioSessao.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<GeneroFilme>(repositorioGeneroFilme.Cadastrar);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }
    //[TestMethod]
    //public void Deve_Cadastrar_Registro_Corretamente()
    //{
    //    var generoFilme = Builder<GeneroFilme>.CreateNew()
    //        .With(g => g.Descricao = "Ação")
    //        .Persist();
    //    var filme = Builder<Filme>.CreateNew()
    //        .With(f => f.Genero = generoFilme)
    //        .Persist();
    //    var sala = Builder<Sala>.CreateNew().Persist();
    //    var sessao = Builder<Sessao>.CreateNew()
    //        .With(s => s.Filme = filme)
    //        .Persist();

    //    Ingresso ingresso = sessao.GerarIngresso(50, true);
    //    dbContext.SaveChanges();

    //    var registroSelecionado = repositorioIngresso.SelecionarRegistros(ingresso.Id);

    //    Ingresso ingressoEncontrado = registroSelecionado.FirstOrDefault()!;

    //    Assert.AreEqual(ingresso, ingressoEncontrado);

    //}

}