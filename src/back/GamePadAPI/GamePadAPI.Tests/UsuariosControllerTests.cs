using System.Threading.Tasks;
using GamePad_TIDAI_2025.Models;
using GamePadAPI.Controllers;
using GamePadAPI.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GamePadAPI.Tests
{
    /// <summary>
    /// Testes unitários do cadastro e autenticação de usuários (CT01 a CT05).
    /// </summary>
    public class UsuariosControllerTests
    {
        // CT01 – Cadastro de usuário com e-mail já existente.
        [Fact]
        public async Task CT01_PostUsuario_ComEmailDuplicado_RetornaBadRequest()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            context.Usuarios.Add(new Usuario
            {
                Nome = "usuarioExistente",
                Email = "user@teste.com",
                Senha = "hash"
            });
            await context.SaveChangesAsync();
            var controller = new UsuariosController(context);

            var novo = new Usuario { Nome = "OutroNome", Email = "user@teste.com", Senha = "123456" };

            // Act
            var resultado = await controller.PostUsuario(novo);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var mensagem = ObjectReader.Get<string>(badRequest.Value, "message");
            Assert.Equal("Já existe um usuário cadastrado com este e-mail.", mensagem);
        }

        // CT02 – Cadastro de usuário com nome de usuário já existente.
        [Fact]
        public async Task CT02_PostUsuario_ComNomeDuplicado_RetornaBadRequest()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            context.Usuarios.Add(new Usuario
            {
                Nome = "jogador1",
                Email = "existente@teste.com",
                Senha = "hash"
            });
            await context.SaveChangesAsync();
            var controller = new UsuariosController(context);

            var novo = new Usuario { Nome = "jogador1", Email = "novo@teste.com", Senha = "123456" };

            // Act
            var resultado = await controller.PostUsuario(novo);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var mensagem = ObjectReader.Get<string>(badRequest.Value, "message");
            Assert.Equal("Já existe um usuário cadastrado com este nome de usuário.", mensagem);
        }

        // CT03 – Cadastro de usuário válido: senha com hash BCrypt e imagem padrão.
        [Fact]
        public async Task CT03_PostUsuario_Valido_CriaComSenhaHasheadaEImagemPadrao()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var controller = new UsuariosController(context);

            var novo = new Usuario { Nome = "novoUser", Email = "novo@teste.com", Senha = "senhaForte1" };

            // Act
            var resultado = await controller.PostUsuario(novo);

            // Assert
            Assert.IsType<CreatedAtActionResult>(resultado.Result);

            var salvo = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "novo@teste.com");
            Assert.NotNull(salvo);
            Assert.NotEqual("senhaForte1", salvo.Senha); // senha não fica em texto puro
            Assert.True(BCrypt.Net.BCrypt.Verify("senhaForte1", salvo.Senha)); // hash válido
            Assert.Equal("/profile-images/default-profile.png", salvo.ImgUser); // imagem padrão
        }

        // CT04 – Autenticação com credenciais válidas: gera token JWT.
        [Fact]
        public async Task CT04_Authenticate_ComCredenciaisValidas_RetornaJwt()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            context.Usuarios.Add(new Usuario
            {
                Nome = "login",
                Email = "login@teste.com",
                Senha = BCrypt.Net.BCrypt.HashPassword("senha123")
            });
            await context.SaveChangesAsync();
            var controller = new UsuariosController(context);

            var login = new UsuariosController.LoginDto { Email = "login@teste.com", Senha = "senha123" };

            // Act
            var resultado = await controller.Authenticate(login);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(resultado);
            var jwt = ObjectReader.Get<string>(ok.Value, "jwt");
            Assert.False(string.IsNullOrWhiteSpace(jwt));
        }

        // CT05 – Autenticação com senha incorreta.
        [Fact]
        public async Task CT05_Authenticate_ComSenhaIncorreta_RetornaNotFound()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            context.Usuarios.Add(new Usuario
            {
                Nome = "login",
                Email = "login@teste.com",
                Senha = BCrypt.Net.BCrypt.HashPassword("senha123")
            });
            await context.SaveChangesAsync();
            var controller = new UsuariosController(context);

            var login = new UsuariosController.LoginDto { Email = "login@teste.com", Senha = "senhaErrada" };

            // Act
            var resultado = await controller.Authenticate(login);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado);
        }
    }
}
