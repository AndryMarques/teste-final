using System;
using System.Drawing;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Xunit;

// Desativa paralelismo: um navegador por vez, ideal para gravação em vídeo.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace GamePadAPI.SeleniumTests.Infra
{
    /// <summary>
    /// Base dos testes funcionais Selenium. Cuida do ciclo de vida do navegador (Edge)
    /// e oferece ações "lentas e visíveis" (pausas + destaque dos elementos) para a gravação.
    ///
    /// Variáveis de ambiente:
    ///   BASE_URL       (padrão http://localhost:5173) – URL do front-end
    ///   HEADLESS=1     – executa sem janela (use somente para validação rápida)
    ///   PAUSE_MS       (padrão 1000) – pausa entre as ações
    ///   TYPE_DELAY_MS  (padrão 70)   – atraso entre cada caractere digitado
    /// </summary>
    public abstract class SeleniumTestBase : IDisposable
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;
        protected readonly string BaseUrl;
        private readonly int _pauseMs;
        private readonly int _typeDelayMs;

        protected SeleniumTestBase()
        {
            BaseUrl = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5173").TrimEnd('/');
            _pauseMs = ReadInt("PAUSE_MS", 1000);
            _typeDelayMs = ReadInt("TYPE_DELAY_MS", 70);
            var headless = Environment.GetEnvironmentVariable("HEADLESS") == "1";

            var options = new EdgeOptions();
            options.AddArgument("--inprivate");
            options.AddArgument("--no-first-run");
            options.AddArgument("--window-size=1920,1080");
            options.AddExcludedArgument("enable-automation");
            if (headless)
                options.AddArgument("--headless=new");

            Driver = new EdgeDriver(options);
            // Garante largura >= 1280px para a navbar desktop (hidden xl:flex) aparecer,
            // tanto em headless quanto com janela visível.
            Driver.Manage().Window.Size = new Size(1920, 1080);
            if (!headless)
            {
                try { Driver.Manage().Window.Maximize(); } catch { /* opcional */ }
            }
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(25));
        }

        private static int ReadInt(string key, int def)
            => int.TryParse(Environment.GetEnvironmentVariable(key), out var v) ? v : def;

        // -------- Ações de apoio (lentas/visíveis) --------

        protected void Pause() => Thread.Sleep(_pauseMs);
        protected void Pause(int ms) => Thread.Sleep(ms);

        protected void Goto(string path)
        {
            Driver.Navigate().GoToUrl(BaseUrl + path);
            Pause();
        }

        /// <summary>Espera o elemento existir e estar visível, retornando-o.</summary>
        protected IWebElement WaitVisible(By by)
            => Wait.Until(d =>
            {
                try
                {
                    var el = d.FindElement(by);
                    return el.Displayed ? el : null;
                }
                catch (NoSuchElementException) { return null; }
                catch (StaleElementReferenceException) { return null; }
            });

        protected void WaitUrlContains(string fragment)
            => Wait.Until(d => d.Url.Contains(fragment));

        /// <summary>Espera existir ao menos um elemento visível para o seletor, tolerando re-renderizações (stale).</summary>
        protected void WaitForVisible(By by)
            => Wait.Until(d =>
            {
                try
                {
                    foreach (var e in d.FindElements(by))
                    {
                        try { if (e.Displayed) return true; }
                        catch (StaleElementReferenceException) { return false; }
                    }
                    return false;
                }
                catch (StaleElementReferenceException) { return false; }
            });

        protected void Highlight(IWebElement el)
        {
            try
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript(
                    "arguments[0].scrollIntoView({block:'center'});" +
                    "arguments[0].style.outline='3px solid #ff2d95';" +
                    "arguments[0].style.outlineOffset='2px';", el);
            }
            catch { /* destaque é cosmético */ }
        }

        protected void Click(By by)
        {
            var el = WaitVisible(by);
            Highlight(el);
            Pause();
            for (var i = 0; i < 3; i++)
            {
                try { el.Click(); Pause(); return; }
                catch (StaleElementReferenceException) { el = WaitVisible(by); }
                catch (ElementClickInterceptedException)
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", el);
                    Pause();
                    return;
                }
            }
            el.Click();
            Pause();
        }

        /// <summary>Digita caractere a caractere para o vídeo acompanhar.</summary>
        protected IWebElement Type(By by, string text)
        {
            var el = WaitVisible(by);
            Highlight(el);
            el.Click();
            el.Clear();
            foreach (var ch in text)
            {
                el.SendKeys(ch.ToString());
                Thread.Sleep(_typeDelayMs);
            }
            Pause();
            return el;
        }

        public void Dispose()
        {
            try
            {
                var dir = Environment.GetEnvironmentVariable("SHOT_DIR");
                if (!string.IsNullOrEmpty(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                    ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(System.IO.Path.Combine(dir, "last.png"));
                    System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "last.url.txt"), Driver.Url);
                    System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "last.html"), Driver.PageSource);
                }
            }
            catch { /* diagnóstico é opcional */ }

            Pause(600);
            try { Driver.Quit(); } catch { /* ignore */ }
            Driver?.Dispose();
        }
    }
}
