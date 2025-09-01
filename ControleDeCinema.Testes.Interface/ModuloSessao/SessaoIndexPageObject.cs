using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;

    public class SessaoIndexPageObject
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public SessaoIndexPageObject(IWebDriver driver)
        {
            this.driver = driver;

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        public SessaoIndexPageObject IrPara(string enderecoBase)
        {
            driver?.Navigate().GoToUrl(Path.Combine(enderecoBase, "sessoes"));

            return this;
        }

        public SessaoFormPageObject ClickCadastrar()
        {
            wait.Until(d => d?.FindElement(By.CssSelector("[data-se='btnCadastrar']"))).Click();

            wait.Until(d => d.Url.Contains("/cadastrar"));

            return new SessaoFormPageObject(driver!);
        }

        public SessaoFormPageObject ClickEditar()
        {
            wait.Until(d => d?.FindElement(By.CssSelector(".card a[title='Edição']"))).Click();

            return new SessaoFormPageObject(driver!);
        }

        public SessaoFormPageObject ClickEncerrar()
        {
            wait.Until(d => d?.FindElement(By.CssSelector(".card a[title='Encerrar']"))).Click();

            return new SessaoFormPageObject(driver!);
        }

        public SessaoIndexPageObject ClickDetalhes()
        {
            wait.Until(d => d?.FindElement(By.CssSelector(".card a[title='Detalhes']"))).Click();

            return this;
        }

        public SessaoFormPageObject ClickExcluir()
        {
            wait.Until(d => d?.FindElement(By.CssSelector(".card a[title='Exclusão']"))).Click();

            return new SessaoFormPageObject(driver!);
        }

        public SessaoFormPageObject ClickComprarIngresso()
        {
            wait.Until(d => d?.FindElement(By.CssSelector(".card a[title='Comprar Ingresso']"))).Click();

            return new SessaoFormPageObject(driver!);
        }
        
        public SessaoFormPageObject ClickComprarIngressoPorFilme(string nomeFilme)
    {
        var xPath = $"//div[contains(., 'Sessão para {nomeFilme}')]//a[@title='Comprar Ingresso']";
        wait.Until(d => d.FindElement(By.XPath(xPath))).Click();
        return new SessaoFormPageObject(driver!);
    }

        public SessaoIndexPageObject ClickDetalhesPorFilme(string nomeFilme)
    {
        var xPath = $"//div[contains(., 'Sessão para {nomeFilme}')]//a[@title='Detalhes']";
        wait.Until(d => d.FindElement(By.XPath(xPath))).Click();
        return this;
    }

        public bool ContemSessao(string sessao, DateTime horario)
        {
            wait.Until(d => d.FindElement(By.CssSelector("[data-se='btnCadastrar']")).Displayed);

            var sessaoVerificada = driver.PageSource.Contains($"Sessão para {sessao}");
            var horarioVerificado = driver.PageSource.Contains($"{horario:dd/MM/yyyy HH:mm}:00");

            return sessaoVerificada && horarioVerificado;
        }

        public bool ContemIngresso(string filme)
        {
            wait.Until(d => d.PageSource.Contains(filme));

            return driver.PageSource.Contains(filme);
        }

        public bool ContemInformacaoDeIngressos(string contagemEsperada)
        {
            wait.Until(d => d.PageSource.Contains("Detalhes da Sessão"));

            try
            {
                var elementoContagem = driver.FindElement(By.CssSelector("[data-se='contagem-ingressos']"));
                return elementoContagem.Text.Contains(contagemEsperada);
            }
            catch (NoSuchElementException)
            {
                return driver.PageSource.Contains(contagemEsperada);
            }
        }

        public int ContarCardsDeSessaoParaFilme(string nomeFilme)
        {
            var seletor = By.XPath($"//div[contains(@class, 'card') and .//h5[contains(text(), 'Sessão para {nomeFilme}')]]");

            try
            {
                // Espera um pouco para garantir que os cards carregaram
                wait.Until(d => d.FindElement(By.CssSelector("[data-se='btnCadastrar']")).Displayed);

                var cardsEncontrados = driver.FindElements(seletor);
                return cardsEncontrados.Count;
            }
            catch (WebDriverTimeoutException)
            {
                return 0;
            }
        }
        public bool ContemDetalhes(string sessao, DateTime horario, int ingressos)
        {
            wait.Until(d => d.PageSource.Contains("Detalhes da Sessão"));

            var sessaoVerificada = driver.PageSource.Contains($"{sessao}");
            var horarioVerificado = driver.PageSource.Contains($"{horario:dd/MM/yyyy HH:mm}:00");
            var salaVerificada = driver.PageSource.Contains($"{ingressos}");

            return sessaoVerificada && horarioVerificado && salaVerificada;
        }

        public bool ContemErroSpan(string erro)
        {
            return wait.Until(d => d.FindElement(By.CssSelector($"span[id='{erro}-error']")).Displayed);
        }

        public bool ContemErroAlert(string erroMsg)
        {
            wait.Until(d => d.FindElement(By.CssSelector($"div[data-se='alertCard']")).Displayed);

            var alert = driver.FindElement(By.CssSelector($"div[data-se='alertCard']")).Text;

            return alert.Contains(erroMsg);
        }
}
