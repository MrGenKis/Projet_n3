using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests.Integration
{
    public class FakeLocalizer : IStringLocalizer<ProductService>
    {
        public LocalizedString this[string name] => new LocalizedString(name, name);
        public LocalizedString this[string name, params object[] args] => new LocalizedString(name, name);
        public IEnumerable<LocalizedString> GetAllStrings(bool _) => new List<LocalizedString>();
        public IStringLocalizer WithCulture(CultureInfo culture) => this;
    }

    public class ProductIntegrationTests
    {
        private const string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=P3ReferentialTest;Trusted_Connection=True;MultipleActiveResultSets=true";

        private static ProductService BuildService()
        {
            var options = new DbContextOptionsBuilder<P3Referential>()
                          .UseSqlServer(ConnectionString)
                          .Options;

            var config = new ConfigurationBuilder()
                         .AddInMemoryCollection(new Dictionary<string, string>
                         {
                             ["ConnectionStrings:P3Referential"] = ConnectionString
                         })
                         .Build();

            var context = new P3Referential(options, config);
            context.Database.EnsureCreated();

            var cart = new Cart();
            var productRepo = new ProductRepository(context);
            var orderRepo = new OrderRepository(context);
            var localizer = new FakeLocalizer();

            return new ProductService(cart, productRepo, orderRepo, localizer);
        }

        private static void SetInvariantCulture()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        [Fact(DisplayName = "Ajout côté admin : le produit est visible côté client")]
        public void AjouterProduit_Et_Verifier_Presence_CoteClient()
        {
            // Arrange
            SetInvariantCulture();
            var service = BuildService();
            var uniqueName = "Produit_" + Guid.NewGuid().ToString("N");

            var produitTest = new ProductViewModel
            {
                Name = uniqueName,
                Price = "15.99",
                Stock = "10",
                Description = "Ajout test",
                Details = "Ajout d’un produit via test"
            };

            // Act
            service.SaveProduct(produitTest);

            // Assert
            var produit = service.GetAllProductsViewModel()
                                 .FirstOrDefault(p => p.Name == uniqueName);

            Assert.NotNull(produit);
            Assert.Equal("10", produit.Stock);
            Assert.Equal("15.99", produit.Price);

            // Clean-up
            service.DeleteProduct(produit.Id);
        }

        [Fact(DisplayName = "Suppression côté admin : le produit n’est plus visible côté client")]
        public void SupprimerProduit_Et_Verifier_Absence_CoteClient()
        {
            // Arrange
            SetInvariantCulture();
            var service = BuildService();
            var uniqueName = "Produit_" + Guid.NewGuid().ToString("N");

            var produitTest = new ProductViewModel
            {
                Name = uniqueName,
                Price = "9.99",
                Stock = "5",
                Description = "Suppression test",
                Details = "Produit à supprimer"
            };

            service.SaveProduct(produitTest);

            var produit = service.GetAllProductsViewModel()
                                 .FirstOrDefault(p => p.Name == uniqueName);

            Assert.NotNull(produit);

            // Act
            service.DeleteProduct(produit.Id);

            // Assert
            var produitsRestants = service.GetAllProductsViewModel();
            Assert.DoesNotContain(produitsRestants, p => p.Id == produit.Id);
        }
    }
}
