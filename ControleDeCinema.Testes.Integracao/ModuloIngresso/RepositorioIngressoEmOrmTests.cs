using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ControleDeCinema.Testes.Integracao.ModuloIngresso;

[TestClass]
[TestCategory("Testes de Integração de Ingresso")]
public sealed class RepositorioIngressoEmOrmTests : TestFixture
{
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

    //    var registroSelecionado = repositorioIngresso?.SelecionarRegistros(ingresso.Id);

    //    Ingresso ingressoEncontrado = registroSelecionado.FirstOrDefault()!;

    //    Assert.AreEqual(ingresso, ingressoEncontrado);

    //}

}