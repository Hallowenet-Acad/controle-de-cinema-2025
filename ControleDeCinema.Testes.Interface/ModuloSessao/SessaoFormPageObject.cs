using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;

public class SessaoFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public SessaoFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("[data-se='form']")).Displayed);
    }

    public SessaoFormPageObject PreencherDataHorarioInicio(DateTime dataHorario)
    {
        wait.Until(d =>
        d.FindElement(By.Id("Inicio")).Displayed &&
        d.FindElement(By.Id("Inicio")).Enabled
    );

        var input = driver.FindElement(By.Id("Inicio"));
        input.Clear();

        string valor = dataHorario.ToString("dd-MM-yyyy");
        string hora = dataHorario.ToString("HH:mm");

        input.SendKeys(valor);
        input.SendKeys(Keys.ArrowRight);
        input.SendKeys(hora);

        return this;
    }

    public SessaoFormPageObject PreencherNumeroMaximoIngressos(int numero)
    {
        wait.Until(d =>
            d.FindElement(By.Id("NumeroMaximoIngressos")).Displayed &&
            d.FindElement(By.Id("NumeroMaximoIngressos")).Enabled
        );

        var inputNome = driver?.FindElement(By.Id("NumeroMaximoIngressos"));
        inputNome?.Clear();
        inputNome?.SendKeys(numero.ToString());
        return this;
    }
    public SessaoFormPageObject ClickSubmitEsperandoErros()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d =>
        {
            bool segueNoCadastro = d.Url.Contains("/sessoes/cadastrar", StringComparison.OrdinalIgnoreCase) &&
                                   d.FindElement(By.CssSelector("form[data-se='form']")).Displayed;

            ReadOnlyCollection<IWebElement> spans = d.FindElements(By.CssSelector("span[data-valmsg-for]"));
            bool temMensagemValidacao = spans.Any(s => !string.IsNullOrWhiteSpace(s.Text));

            ReadOnlyCollection<IWebElement> alerts = d.FindElements(By.CssSelector("div.alert[role='alert']"));
            bool temMensagemAlerta = alerts.Any(a => a.Displayed && !string.IsNullOrWhiteSpace(a.Text));

            return segueNoCadastro && (temMensagemValidacao || temMensagemAlerta);
        });

        return this;
    }

    public SessaoFormPageObject SelecionarFilme(string filme)
    {
        wait.Until(d =>
            d.FindElement(By.Id("FilmeId")).Displayed &&
            d.FindElement(By.Id("FilmeId")).Enabled
        );

        var select = new SelectElement(driver.FindElement(By.Id("FilmeId")));
        select.SelectByText(filme);

        return this;
    }

    public SessaoFormPageObject SelecionarSala(int sala)
    {
        wait.Until(d =>
            d.FindElement(By.Id("SalaId")).Displayed &&
            d.FindElement(By.Id("SalaId")).Enabled
        );

        var select = new SelectElement(driver.FindElement(By.Id("SalaId")));
        select.SelectByText(sala.ToString());

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
    public SessaoIndexPageObject Confirmar()
    {
        new Actions(driver).ScrollByAmount(0, 500).Perform();

        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        return new SessaoIndexPageObject(driver!);
    }
    public SessaoIndexPageObject ClickSubmitEncerrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnEncerrar']"))).Click();
        wait.Until(d => d.Url.Contains("/sessoes/detalhes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnVoltar']"))).Click();
        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));

        return new(driver);
    }
    public SessaoIndexPageObject ClickSubmitExcluir(string tituloFilme)
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();
        wait.Until(d => d.Url.Contains("/sessoes", StringComparison.OrdinalIgnoreCase));
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);
        wait.Until(d => !d.PageSource.Contains(tituloFilme));

        return new(driver);
    }
}
