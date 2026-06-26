using System;
using GamePad_TIDAI_2025.Models;
using Microsoft.EntityFrameworkCore;

namespace GamePadAPI.Tests.Helpers
{
    /// <summary>
    /// Cria um AppDbContext apoiado no provedor EF Core InMemory.
    /// Cada chamada usa um nome de banco único, garantindo total isolamento entre os testes.
    /// </summary>
    public static class TestDbContextFactory
    {
        public static AppDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            return new AppDbContext(options);
        }
    }

    /// <summary>
    /// Lê propriedades de objetos anônimos retornados pelos controllers
    /// (ex.: new { message = "..." }), já que esses tipos são internos à API.
    /// </summary>
    public static class ObjectReader
    {
        public static T Get<T>(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop == null)
                throw new ArgumentException($"Propriedade '{propertyName}' não encontrada em {obj.GetType().Name}.");
            return (T)prop.GetValue(obj);
        }
    }
}
