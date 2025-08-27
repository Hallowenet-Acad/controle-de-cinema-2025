using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ControleDeCinema.Testes.Integracao.ModuloSala;

[TestClass]
[TestCategory("Testes de Integração de Sala")]
public sealed class RepositorioSalaEmOrmTests : TestFixture
{
   
    [TestMethod]
    public void Deve_Cadastrar_Registro_Corretamente()
    {
        // Arrange
        var sala = new Sala(3, 60);
        
        // Act
        repositorioSala?.Cadastrar(sala);

        dbContext?.SaveChanges();

        // Assert
        var registroSelecionado = repositorioSala?.SelecionarRegistroPorId(sala.Id);

        Assert.AreEqual(sala, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Registros_Corretamente()
    {
        // Arrange
        var sala = new Sala(3, 60);
        var sala2 = new Sala(1, 70);
        var sala3 = new Sala(2, 58);

        repositorioSala.Cadastrar(sala);
        repositorioSala.Cadastrar(sala2);
        repositorioSala.Cadastrar(sala3);

        dbContext.SaveChanges();

        List<Sala> salasEsperadas = [sala, sala2, sala3];

        var salasEsperadasOrdenadas = salasEsperadas
            .OrderBy(s => s.Numero)
            .ToList();

        // Act
        var salasRecebidas = repositorioSala
            .SelecionarRegistros();

        var salasRecebidasOrdenadas = salasRecebidas
            .OrderBy(s => s.Numero)
            .ToList();

        // Assert
        CollectionAssert.AreEqual(salasEsperadasOrdenadas, salasRecebidas);
    }
    [TestMethod]
    public void Deve_Editar_Registros_Corretamente()
    {
        // Arrange
        var sala = new Sala(3, 60);
        repositorioSala.Cadastrar(sala);
        dbContext.SaveChanges();

        var salaEditada = new Sala(4, 30);
        // Act
        var conseguiuEditar = repositorioSala.Editar(sala.Id, salaEditada);
        dbContext.SaveChanges();

        // Assert
        var registroSelecionado = repositorioSala.SelecionarRegistroPorId(sala.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(sala, registroSelecionado);

    }
    [TestMethod]
    public void Deve_Excluir_Registros_Corretamente()
    {
        // Arrange
        var sala = new Sala(3, 60);
        repositorioSala.Cadastrar(sala);
        dbContext.SaveChanges();

        // Act
        var conseguiuExcluir = repositorioSala.Excluir(sala.Id);
        dbContext.SaveChanges();

        // Assert 
        var registroSelecionado = repositorioSala.SelecionarRegistroPorId(sala.Id);

        Debug.WriteLine(registroSelecionado);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(registroSelecionado);
    }
}
