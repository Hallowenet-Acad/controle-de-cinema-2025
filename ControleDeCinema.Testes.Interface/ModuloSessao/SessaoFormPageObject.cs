using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

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

    public SessaoIndexPageObject Confirmar()
    {
        new Actions(driver).ScrollByAmount(0, 500).Perform();

        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        return new SessaoIndexPageObject(driver!);
    }
}
