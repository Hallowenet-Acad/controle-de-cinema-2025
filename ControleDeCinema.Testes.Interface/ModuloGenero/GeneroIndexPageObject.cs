using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace ControleDeCinema.Testes.Interface.ModuloGenero;

public class GeneroIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public GeneroIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(NoSuchElementException));
    }

    public GeneroIndexPageObject IrPara(string enderecoBase)
    {
        driver.Navigate().GoToUrl($"{enderecoBase.TrimEnd('/')}/generos");

        wait.Until(d => d.Url.Contains("/generos", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return this;
    }
            
    public GeneroFormPageObject ClickCadastrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']"))).Click();

        return new(driver);
    }

    public GeneroFormPageObject ClickEditar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnEditar']"))).Click();

        return new(driver);
    }

    public GeneroFormPageObject ClickExcluir()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnExcluir']"))).Click();

        return new(driver);
    }

    public bool ContemGenero(string descricao)
    {
        return driver.PageSource.Contains(descricao);
    }
}
