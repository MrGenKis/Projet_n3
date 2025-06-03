using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    // Classe FakeLocalizer qui simule le Localizer (très simple)
    // Elle permet de renvoyer un message d'erreur basique comme "MissingName"
    public class FakeLocalizer : IStringLocalizer<ProductService>
    {
        // Cette méthode retourne simplement la clé qu'on lui demande
        public LocalizedString this[string name] => new LocalizedString(name, name);

        // Variante avec arguments (pas utilisée ici, mais nécessaire pour l'interface)
        public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, name);

        // Méthode obligatoire mais inutile ici
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => new List<LocalizedString>();

        public IStringLocalizer WithCulture(CultureInfo culture) => this;
    }

    // Classe qui contient tous nos tests unitaires
    public class ProductServiceTests
    {
        // Méthode qui nous permet de créer facilement un ProductService pour les tests
        private ProductService CreateProductService()
        {
            var fakeLocalizer = new FakeLocalizer();
            return new ProductService(
                cart: null,
                productRepository: null,
                orderRepository: null,
                localizer: fakeLocalizer
            );
        }

        // Premier test : vérifier que le champ Name est obligatoire
        [Fact]
        public void Test_Nom()
        {
            var service = CreateProductService();
            var produit = new ProductViewModel { Name = null, Price = "10", Stock = "5" };

            var erreurs = service.CheckProductModelErrors(produit);

            Assert.Contains("MissingName", erreurs);
        }

        // Deuxième test : vérifier que le champ Prix est obligatoire
        [Fact]
        public void Test_Prix()
        {
            var service = CreateProductService();
            var produit = new ProductViewModel { Name = "ProduitTest", Price = "", Stock = "5" };

            var erreurs = service.CheckProductModelErrors(produit);

            Assert.Contains("MissingPrice", erreurs);
        }

        // Troisième test : le prix doit être un nombre
        [Fact]
        public void Test_PrixNombre()
        {
            var service = CreateProductService();
            var produit = new ProductViewModel { Name = "ProduitTest", Price = "abc", Stock = "5" };

            var erreurs = service.CheckProductModelErrors(produit);

            Assert.Contains("PriceNotANumber", erreurs);
        }

        //  le prix doit être supérieur à zéro
        [Fact]
        public void Test_PrixSuperieurAZero()
        {
            var service = CreateProductService();
            var produit = new ProductViewModel { Name = "ProduitTest", Price = "0", Stock = "5" };

            var erreurs = service.CheckProductModelErrors(produit);

            Assert.Contains("PriceNotGreaterThanZero", erreurs);
        }

        // quantité  obligatoire
        [Fact]
        public void Test_Quantite()
        {
            var service = CreateProductService();
            var produit = new ProductViewModel { Name = "ProduitTest", Price = "10", Stock = "" };

            var erreurs = service.CheckProductModelErrors(produit);

            Assert.Contains("MissingQuantity", erreurs);
        }

        // la quantité doit être chiffre un entier
        [Fact]
        public void Test_QuantiteNombre()
        {
            var service = CreateProductService();
            var produit = new ProductViewModel { Name = "ProduitTest", Price = "10", Stock = "abc" };

            var erreurs = service.CheckProductModelErrors(produit);

            Assert.Contains("StockNotAnInteger", erreurs);
        }

        // la quantité doit être supérieure à zéro
        [Fact]
        public void Test_QuantiteSuperieureAZero()
        {
            var service = CreateProductService();
            var produit = new ProductViewModel { Name = "ProduitTest", Price = "10", Stock = "0" };

            var erreurs = service.CheckProductModelErrors(produit);

            Assert.Contains("StockNotGreaterThanZero", erreurs);
        }
    }
}
