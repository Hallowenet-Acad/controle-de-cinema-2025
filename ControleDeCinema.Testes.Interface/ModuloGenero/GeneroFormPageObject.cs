using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace ControleDeCinema.Testes.Interface.ModuloGenero;

public class GeneroFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public GeneroFormPageObject(IWebDriver driver)
    {
        this.driver = driver; string url = driver.Url;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(NoSuchElementException));
    }

    public GeneroFormPageObject PreencherDescricao(string descricao)
    {
        wait.Until(d =>
            d.FindElement(By.CssSelector("input[data-se='inputDescricao']")).Displayed &&
            d.FindElement(By.CssSelector("input[data-se='inputDescricao']")).Enabled
        );

        IWebElement inputDescricao = driver.FindElement(By.CssSelector("input[data-se='inputDescricao']"));
        inputDescricao.Clear();
        inputDescricao.SendKeys(descricao);

        return this;
    }

    public GeneroIndexPageObject ClickSubmit()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d => d.Url.Contains("/generos", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new(driver);
    }

    public GeneroIndexPageObject ClickSubmitExcluir(string descricao)
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d => d.Url.Contains("/generos", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);
        wait.Until(d => !d.PageSource.Contains(descricao));

        return new(driver);
    }

    public GeneroFormPageObject ClickSubmitEsperandoErros()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d =>
        {
            // permanece no formulário de cadastro/edição e aparecem mensagens de validação
            bool segueNoCadastro = d.FindElement(By.CssSelector("form[data-se='form']")).Displayed;

            ReadOnlyCollection<IWebElement> spans = d.FindElements(By.CssSelector("span[data-valmsg-for]"));
            bool temMensagemValidacao = spans.Any(s => !string.IsNullOrWhiteSpace(s.Text));

            ReadOnlyCollection<IWebElement> alerts = d.FindElements(By.CssSelector("div.alert[role='alert']"));
            bool temMensagemAlerta = alerts.Any(a => a.Displayed && !string.IsNullOrWhiteSpace(a.Text));

            return segueNoCadastro && (temMensagemValidacao || temMensagemAlerta);
        });

        return this;
    }

    public bool EstourouValidacao(string nomeCampo = "")
    {
        if (!string.IsNullOrWhiteSpace(nomeCampo))
        {
            IWebElement span = driver.FindElement(By.CssSelector($"span[data-valmsg-for='{nomeCampo}']"));
            if (!string.IsNullOrWhiteSpace(span.Text?.Trim()))
                return true;
        }

        ReadOnlyCollection<IWebElement> alerts = driver.FindElements(By.CssSelector("div.alert[role='alert']"));
        return alerts.Any(a => a.Displayed && !string.IsNullOrWhiteSpace(a.Text));
    }
}