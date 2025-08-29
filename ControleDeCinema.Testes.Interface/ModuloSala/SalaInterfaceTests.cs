using ControleDeCinema.Testes.Interface.Compartilhado;
using OpenQA.Selenium;

namespace ControleDeCinema.Testes.Interface.ModuloSala;

//dotnet run --project ./ControleDeCinema.WebApp --launch-profile "https [Dev]"

[TestClass]
[TestCategory("Tests de Interface de Sala")]
public sealed class SalaInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sala_Corretamente()
    {

        // Arrange
        var salaIndex = new SalaIndexPageObject(driver!)
            .IrPara(enderecoBase!); 

        salaIndex
            .ClickCadastrar()
            .PreencherNumero("1")
            .PreencherCapacidade("100")
            .Confirmar();

        // Assert
        Assert.IsTrue(salaIndex.ContemSala("1"));
    }

    [TestMethod]
    public void Deve_Editar_Sala_Corretamente()
    {
        // Arrange
        var salaIndex = new SalaIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        salaIndex
            .ClickCadastrar()
            .PreencherNumero("1")
            .PreencherCapacidade("100")
            .Confirmar();

        // Act
        salaIndex
            .ClickEditar()
            .PreencherNumero("2")
            .PreencherCapacidade("50")
            .Confirmar();

        // Assert
        Assert.IsTrue(salaIndex.ContemSala("2"));
    }
}

/* 
 
[TestClass]
[TestCategory("Tests de Interface de Sessão")]
public sealed class SessaoInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sessao_Corretamente()
    {
        //Arrange
        driver?.Navigate().GoToUrl(Path.Combine(enderecoBase, "salas", "cadastrar"));

        driver?.FindElement(By.Id("Numero")).SendKeys("2");
        driver?.FindElement(By.Id("Capacidade")).SendKeys("4");

        driver?.FindElement(By.CssSelector("button[type='submit']")).Click();

        driver?.Navigate().GoToUrl(Path.Combine(enderecoBase, "salas", "cadastrar"));

        //Act
        driver?.FindElement(By.Id("Numero")).SendKeys("2");


        //Assert
    }
 */
