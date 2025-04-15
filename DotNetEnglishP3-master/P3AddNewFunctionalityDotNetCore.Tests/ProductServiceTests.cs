using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;
using P3AddNewFunctionalityDotNetCore.Models.Services;
namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        /// <summary>
        /// Take this test method as a template to write your test method.
        /// A test method must check if a definite method does its job:
        /// returns an expected value from a particular set of parameters
        /// </summary>
        [Fact]
        public void CheckProductModelErrors_ShouldReturn_MissingName_WhenNameIsNull()
        {
            // Arrange
            var fakeLocalizer = new FakeLocalizer();
            var productService = new ProductService(
                cart: null,
                productRepository: null,
                orderRepository: null,
                localizer: fakeLocalizer
            );
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
    }
}



