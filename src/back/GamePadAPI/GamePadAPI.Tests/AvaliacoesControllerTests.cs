using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamePad_TIDAI_2025.Models;
using GamePadAPI.Controllers;
using GamePadAPI.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GamePadAPI.Tests
{
    /// <summary>
    /// Testes unitários das avaliações de jogos (CT06 a CT09).
    /// </summary>
    public class AvaliacoesControllerTests
    {
        // CT06 – Avaliação: comentário enviado sem nota.
        [Fact]
        public async Task CT06_PostAvaliacao_ComComentarioSemNota_RetornaBadRequest()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var controller = new AvaliacoesApiController(context);

            var avaliacao = new Avaliacao { Comentario = "Jogo ótimo", Nota = "" };

            // Act
            var resultado = await controller.PostAvaliacao(avaliacao);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Equal("Para comentar, é necessário dar uma nota ao jogo.", badRequest.Value);
        }

        // CT07 – Avaliação: nota sem comentário (caso válido).
        [Fact]
        public async Task CT07_PostAvaliacao_ComNotaSemComentario_CriaAvaliacao()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var controller = new AvaliacoesApiController(context);

            var avaliacao = new Avaliacao
            {
                Nota = "5",
                Comentario = null,
                IgdbGameId = 100,
                Data = DateTime.UtcNow,
                UsuarioId = 1
            };

            // Act
            var resultado = await controller.PostAvaliacao(avaliacao);

            // Assert
            Assert.IsType<CreatedAtActionResult>(resultado.Result);
            Assert.Equal(1, context.Avaliacoes.Count());
        }

        // CT08 – Cálculo da média de avaliações por jogo com filtro mínimo.
        [Fact]
        public async Task CT08_GetMedias_ComFiltroMinimo_RetornaApenasJogosAcimaDaMedia()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            // Jogo A (Id 1): notas 4 e 2 -> média 3
            context.Avaliacoes.Add(new Avaliacao { Nota = "4", IgdbGameId = 1, UsuarioId = 1 });
            context.Avaliacoes.Add(new Avaliacao { Nota = "2", IgdbGameId = 1, UsuarioId = 2 });
            // Jogo B (Id 2): notas 5 e 5 -> média 5
            context.Avaliacoes.Add(new Avaliacao { Nota = "5", IgdbGameId = 2, UsuarioId = 1 });
            context.Avaliacoes.Add(new Avaliacao { Nota = "5", IgdbGameId = 2, UsuarioId = 2 });
            await context.SaveChangesAsync();
            var controller = new AvaliacoesApiController(context);

            // Act
            var resultado = await controller.GetMedias(minMedia: 4);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(resultado.Result);
            var itens = ((IEnumerable)ok.Value).Cast<object>().ToList();

            Assert.Single(itens);
            var item = itens[0];
            Assert.Equal(2L, ObjectReader.Get<long?>(item, "IgdbGameId"));
            Assert.Equal(5d, ObjectReader.Get<double>(item, "Media"));
        }

        // CT09 – Curtir o próprio comentário (não permitido).
        [Fact]
        public async Task CT09_LikeAvaliacao_NoProprioComentario_RetornaBadRequest()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            context.Avaliacoes.Add(new Avaliacao { Id = 1, Nota = "5", UsuarioId = 10, IgdbGameId = 1 });
            await context.SaveChangesAsync();
            var controller = new AvaliacoesApiController(context);

            // Act – usuário 10 tenta curtir a própria avaliação
            var resultado = await controller.LikeAvaliacao(id: 1, usuarioId: 10);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal("Não pode curtir o próprio comentário.", badRequest.Value);
        }
    }
}
