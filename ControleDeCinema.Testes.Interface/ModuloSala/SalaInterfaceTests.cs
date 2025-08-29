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

        //Arrange
        driver?.Navigate().GoToUrl(Path.Combine(enderecoBase, "salas"));

        var elemento = driver?.FindElement(By.CssSelector("a[data-se='btnCadastrar']"));

        elemento?.Click();

        //Act
        driver?.FindElement(By.Id("Numero")).SendKeys("2");
        driver?.FindElement(By.Id("Capacidade")).SendKeys("4");

        driver?.FindElement(By.CssSelector("button[type='submit']")).Click();

        //Assert
        var elementosCard = driver?.FindElements(By.CssSelector(".cards"));

        Assert.AreEqual(1, elementosCard?.Count);
    }

    [TestMethod]
    public void Deve_Editar_Sala_Corretamente()
    {
        //Arrange
        driver?.Navigate().GoToUrl(Path.Combine(enderecoBase, "salas"));

        var elemento = driver?.FindElement(By.CssSelector("a[data-se='btnCadastrar']"));

        elemento?.Click();

        driver?.FindElement(By.Id("Numero")).SendKeys("2");
        driver?.FindElement(By.Id("Capacidade")).SendKeys("4");

        driver?.FindElement(By.CssSelector("button[type='submit']")).Click();

        driver?.FindElement(By.CssSelector(".card"))
            .FindElement(By.CssSelector("a[title='Edição']")).Click();

        //Act
        driver?.FindElement(By.Id("Numero")).SendKeys("5");
        driver?.FindElement(By.Id("Capacidade")).SendKeys("10");

        //Assert
        Assert.IsTrue(driver?.PageSource.Contains("25"));

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
