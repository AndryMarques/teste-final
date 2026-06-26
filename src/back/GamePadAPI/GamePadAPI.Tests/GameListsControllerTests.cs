using System.Threading.Tasks;
using GamePad_TIDAI_2025.Models;
using GamePadAPI.Controllers;
using GamePadAPI.Models;
using GamePadAPI.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GamePadAPI.Tests
{
    /// <summary>
    /// Testes unitários das listas de jogos (CT10).
    /// </summary>
    public class GameListsControllerTests
    {
        // CT10 – Adicionar à lista um jogo já presente nela.
        [Fact]
        public async Task CT10_AddGameToList_ComJogoJaPresente_RetornaConflict()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            context.GameLists.Add(new GameList { Id = 1, Title = "Favoritos", UsuarioId = 1 });
            context.GameListItems.Add(new GameListItem { Id = 1, GameListId = 1, IgdbGameId = 500, GameTitle = "Jogo X" });
            await context.SaveChangesAsync();
            var controller = new GameListsController(context);

            var item = new GameListItem { IgdbGameId = 500, GameTitle = "Jogo X" };

            // Act
            var resultado = await controller.AddGameToList(listId: 1, item: item);

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(resultado);
            var mensagem = ObjectReader.Get<string>(conflict.Value, "message");
            Assert.Equal("Este jogo já está na lista.", mensagem);
        }
    }
}
