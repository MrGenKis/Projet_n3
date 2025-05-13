using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    
    public class FakeLocalizer : IStringLocalizer<ProductService>
    {
        public LocalizedString this[string name]
            => new LocalizedString(name, name);

        public LocalizedString this[string name, params object[] arguments]
            => new LocalizedString(name, name);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => new List<LocalizedString>();

        public IStringLocalizer WithCulture(CultureInfo culture)
            => this;
    }

    public class ProductServiceTests
    {
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

        [Fact]
        public void testl()
        {
            var productService = CreateProductService();
            var productVm = new ProductViewModel
            {
                Name = null,
                Price = "10",
                Stock = "5"
            };

            // Act
            var errors = productService.CheckProductModelErrors(productVm);

            // Assert
            Assert.Contains("MissingName", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ShouldReturn_MissingPrice_WhenPriceIsEmpty()
        {
            // Arrange
            var productService = CreateProductService();
            var productVm = new ProductViewModel
            {
                Name = "TestProduct",
                Price = "", 
                Stock = "5"
            };

            // Act
            var errors = productService.CheckProductModelErrors(productVm);

            // Assert
            Assert.Contains("MissingPrice", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ShouldReturn_PriceNotANumber_WhenPriceIsInvalidString()
        {
            // Arrange
            var productService = CreateProductService();
            var productVm = new ProductViewModel
            {
                Name = "TestProduct",
                Price = "abc", 
                Stock = "5"
            };

            // Act
            var errors = productService.CheckProductModelErrors(productVm);

            // Assert
            Assert.Contains("PriceNotANumber", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ShouldReturn_PriceNotGreaterThanZero_WhenPriceIsZeroOrNegative()
        {
            // Arrange
            var productService = CreateProductService();
            var productVm = new ProductViewModel
            {
                Name = "TestProduct",
                Price = "0", 
                Stock = "5"
            };

            // Act
            var errors = productService.CheckProductModelErrors(productVm);

            // Assert
            Assert.Contains("PriceNotGreaterThanZero", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ShouldReturn_MissingQuantity_WhenStockIsEmpty()
        {
            // Arrange
            var productService = CreateProductService();
            var productVm = new ProductViewModel
            {
                Name = "TestProduct",
                Price = "10",
                Stock = "" 
            };

            // Act
            var errors = productService.CheckProductModelErrors(productVm);

            // Assert
            Assert.Contains("MissingQuantity", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ShouldReturn_StockNotAnInteger_WhenStockIsInvalidString()
        {
            // Arrange
            var productService = CreateProductService();
            var productVm = new ProductViewModel
            {
                Name = "TestProduct",
                Price = "10",
                Stock = "abc" 
            };

            // Act
            var errors = productService.CheckProductModelErrors(productVm);

            // Assert
            Assert.Contains("StockNotAnInteger", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ShouldReturn_StockNotGreaterThanZero_WhenStockIsZeroOrNegative()
        {
            // Arrange
            var productService = CreateProductService();
            var productVm = new ProductViewModel
            {
                Name = "TestProduct",
                Price = "10",
                Stock = "0" 
            };

            // Act
            var errors = productService.CheckProductModelErrors(productVm);

            // Assert
            Assert.Contains("StockNotGreaterThanZero", errors);
        }
    }
}
