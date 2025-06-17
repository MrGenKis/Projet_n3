using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
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
      
        private static ProductService BuildService()
        {
            const string connectionString =
                @"Server=(localdb)\mssqllocaldb;Database=P3ReferentialTest;Trusted_Connection=True;MultipleActiveResultSets=true";

            var options = new DbContextOptionsBuilder<P3Referential>()
                          .UseSqlServer(connectionString)
                          .Options;

            var config = new ConfigurationBuilder()
                         .AddInMemoryCollection(new Dictionary<string, string>
                         {
                             ["ConnectionStrings:P3Referential"] = connectionString
                         })
                         .Build();

            var context = new P3Referential(options, config);
            context.Database.EnsureCreated(); // crée la BDD si nécessaire

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

        [Fact]
        public void AjouterProduit_Et_Verifier_Presence_Dans_Liste()
        {
            // Arrange
            SetInvariantCulture();
            var service = BuildService();

            var produitTest = new ProductViewModel
            {
                Name = "ProduitIntegrationTestFinal",
                Price = "15.99",   // séparateur « . » accepté grâce à InvariantCulture
                Stock = "10",
                Description = "Produit d'intégration",
                Details = "Test automatique"
            };

            // Act
            service.SaveProduct(produitTest);

            var produit = service.GetAllProductsViewModel()
                                 .FirstOrDefault(p => p.Name == "ProduitIntegrationTestFinal");

            // Assert
            Assert.NotNull(produit);
            Assert.Equal("ProduitIntegrationTestFinal", produit.Name);
            Assert.Equal("10", produit.Stock);
            Assert.Equal("15.99", produit.Price);

            // Clean-up
            service.DeleteProduct(produit.Id);
        }

        [Fact]
        public void SupprimerProduit_Et_Verifier_Absence()
        {
            // Arrange
            SetInvariantCulture();
            var service = BuildService();

            var produitTest = new ProductViewModel
            {
                Name = "ProduitIntegrationASupprimer",
                Price = "9.99",
                Stock = "5",
                Description = "Produit à supprimer",
                Details = "Test suppression"
            };

            service.SaveProduct(produitTest);

            var produit = service.GetAllProductsViewModel()
                                 .FirstOrDefault(p => p.Name == "ProduitIntegrationASupprimer");
            Assert.NotNull(produit); // vérifie que l’ajout a réussi

            // Act
            service.DeleteProduct(produit.Id);

            // Assert
            var produitsRestants = service.GetAllProductsViewModel();
            Assert.DoesNotContain(produitsRestants, p => p.Id == produit.Id);
        }
    }
}
