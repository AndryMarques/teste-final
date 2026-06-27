using System;
using System.Text.RegularExpressions;
using GamePadAPI.SeleniumTests.Infra;
using OpenQA.Selenium;
using Xunit;

namespace GamePadAPI.SeleniumTests
{
    /// <summary>
    /// 10 testes funcionais automatizados (CT11 a CT20) do GamePad, executados com Selenium
    /// sobre o front (Vite) integrado ao back local. Ordem controlada para a gravação.
    ///
    /// Pré-requisitos para rodar: back-end no ar (IGDB configurado) e front em http://localhost:5173.
    /// </summary>
    [TestCaseOrderer("GamePadAPI.SeleniumTests.Infra.AlphabeticalOrderer", "GamePadAPI.SeleniumTests")]
    public class FuncionaisSeleniumTests : SeleniumTestBase
    {
        // ---- Seletores reutilizados ----
        private static readonly By BotaoEntrarNavbar = By.XPath("//button[normalize-space()='Entrar']");
        private static readonly By BotaoRegistrarNavbar = By.XPath("//button[normalize-space()='Registrar']");
        private static readonly By BotaoSubmit = By.CssSelector("button[type='submit']");
        private static readonly By CampoEmail = By.CssSelector("input[name='email']");
        private static readonly By CampoSenha = By.CssSelector("input[name='senha']");
        private static readonly By BuscaNavbar = By.CssSelector("input[placeholder='Busque um jogo...']");
        private static readonly By PrimeiroCardJogo = By.XPath("//div[contains(@class,'grid')]//img[1]");

        // CT11 – A página inicial carrega com o logo e a navbar visíveis.
        [Fact]
        public void CT11_Home_CarregaComLogoENavbar()
        {
            Goto("/");

            var logo = WaitVisible(By.CssSelector("img[alt='Logo do site']"));
            var home = WaitVisible(By.XPath("//a[normalize-space()='Home']"));

            Assert.True(logo.Displayed, "O logo deveria estar visível na home.");
            Assert.True(home.Displayed, "O link 'Home' da navbar deveria estar visível.");
        }

        // CT12 – Navegar para a página de Jogos pela navbar e listar jogos.
        [Fact]
        public void CT12_NavegarParaJogos_ListaJogos()
        {
            Goto("/");

            Click(By.XPath("//a[normalize-space()='Jogos']"));

            WaitUrlContains("/games");
            WaitForVisible(PrimeiroCardJogo);

            Assert.Contains("/games", Driver.Url);
        }

        // CT13 – Buscar um jogo retorna resultados.
        [Fact]
        public void CT13_BuscarJogo_RetornaResultados()
        {
            Goto("/");

            Type(BuscaNavbar, "Zelda");
            Click(By.CssSelector("button[aria-label='Buscar']"));

            WaitUrlContains("/games/search/");
            WaitForVisible(By.CssSelector("button[aria-label^='Ver detalhes de']"));

            Assert.Contains("/games/search/", Driver.Url);
        }

        // CT14 – Abrir o detalhe de um jogo a partir da listagem.
        [Fact]
        public void CT14_AbrirDetalheDoJogo()
        {
            Goto("/games");

            WaitForVisible(PrimeiroCardJogo);
            Click(PrimeiroCardJogo);

            Wait.Until(d => Regex.IsMatch(d.Url, @"/games/\d+"));
            Assert.Matches(@"/games/\d+", Driver.Url);
        }

        // CT15 – Abrir o modal de login pela navbar.
        [Fact]
        public void CT15_AbrirModalDeLogin()
        {
            Goto("/");

            Click(BotaoEntrarNavbar);

            var titulo = WaitVisible(By.XPath("//h2[normalize-space()='Entrar']"));
            var email = WaitVisible(CampoEmail);

            Assert.True(titulo.Displayed, "O título 'Entrar' do modal deveria aparecer.");
            Assert.True(email.Displayed, "O campo de e-mail do login deveria aparecer.");
        }

        // CT16 – Login com credenciais inválidas exibe mensagem de erro.
        [Fact]
        public void CT16_LoginInvalido_ExibeErro()
        {
            Goto("/");

            Click(BotaoEntrarNavbar);
            Type(CampoEmail, "naoexiste_" + Sufixo() + "@teste.com");
            Type(CampoSenha, "senhaErrada");
            Click(BotaoSubmit);

            var alerta = WaitVisible(By.XPath("//*[contains(text(),'Inválidos')]"));
            Assert.Contains("Inválidos", alerta.Text);
        }

        // CT17 – Validação do cadastro: enviar formulário vazio exibe erro de campo obrigatório.
        [Fact]
        public void CT17_CadastroVazio_ExibeValidacao()
        {
            Goto("/");

            Click(BotaoRegistrarNavbar);
            WaitVisible(By.XPath("//h2[normalize-space()='Crie sua conta']"));
            Click(BotaoSubmit);

            var erro = WaitVisible(By.XPath("//span[contains(text(),'obrigatório')]"));
            Assert.Contains("obrigatório", erro.Text);
        }

        // CT18 – Cadastrar um novo usuário com sucesso.
        [Fact]
        public void CT18_CadastrarNovoUsuario()
        {
            Goto("/");
            var (nome, email, senha) = GerarUsuario();

            RegistrarUsuario(nome, email, senha);

            var sucesso = WaitVisible(By.XPath("//*[contains(text(),'Cadastro realizado com sucesso')]"));
            Assert.Contains("sucesso", sucesso.Text);
        }

        // CT19 – Login com um usuário recém-cadastrado exibe a saudação.
        [Fact]
        public void CT19_LoginValido_ExibeBoasVindas()
        {
            Goto("/");
            var (nome, email, senha) = GerarUsuario();

            RegistrarUsuario(nome, email, senha);   // após o sucesso, a modal troca para login automaticamente
            LogarPelaModal(email, senha);

            var boasVindas = WaitVisible(By.XPath("//*[contains(text(),'Bem-vindo')]"));
            Assert.Contains("Bem-vindo", boasVindas.Text);
        }

        // CT20 – Usuário logado cria uma nova lista de jogos.
        [Fact]
        public void CT20_UsuarioLogado_CriaLista()
        {
            Goto("/");
            var (nome, email, senha) = GerarUsuario();

            RegistrarUsuario(nome, email, senha);
            LogarPelaModal(email, senha);
            WaitVisible(By.XPath("//*[contains(text(),'Bem-vindo')]")); // garante login + persistência no localStorage
            Pause(1500);

            Goto("/list/create");
            Type(By.CssSelector("input[placeholder='Ex: Meus favoritos']"), "Lista Selenium " + Sufixo());
            Click(By.XPath("//button[normalize-space()='Salvar Lista']"));

            WaitUrlContains("/list/");
            Assert.Contains("/list/", Driver.Url);
        }

        // ---------------- Apoio ----------------

        private static string Sufixo() => Guid.NewGuid().ToString("N").Substring(0, 6);

        private static (string nome, string email, string senha) GerarUsuario()
        {
            var nome = "sel" + Sufixo();                 // 9 chars, dentro de [a-zA-Z0-9 _-]{3,20}
            return (nome, nome + "@teste.com", "senha123");
        }

        private void RegistrarUsuario(string nome, string email, string senha)
        {
            Click(BotaoRegistrarNavbar);
            WaitVisible(By.XPath("//h2[normalize-space()='Crie sua conta']"));
            Type(By.CssSelector("input[name='nome']"), nome);
            Type(CampoEmail, email);
            Type(CampoSenha, senha);
            Type(By.CssSelector("input[name='confirmarSenha']"), senha);
            Click(BotaoSubmit);
            WaitVisible(By.XPath("//*[contains(text(),'Cadastro realizado com sucesso')]"));
        }

        private void LogarPelaModal(string email, string senha)
        {
            // a modal de login abre automaticamente após o cadastro
            WaitVisible(By.XPath("//h2[normalize-space()='Entrar']"));
            Type(CampoEmail, email);
            Type(CampoSenha, senha);
            Click(BotaoSubmit);
        }
    }
}
