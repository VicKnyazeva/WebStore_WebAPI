using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Services.Services;

using Assert = Xunit.Assert;

namespace WebStore.Services.Tests.Services
{
    [TestClass]
    public class CartServiceTests
    {
        private Cart _Cart;

        private Mock<IProductData> _ProductDataMock;
        private Mock<ICartStore> _CartStoreMock;

        private ICartService _CartService;

        [TestInitialize]
        public void TestInitialize()
        {
            _Cart = new Cart
            {
                Items = new List<CartItem>
                {
                    new() { ProductId = 1, Quantity = 1 },
                    new() { ProductId = 2, Quantity = 3 },
                }
            };

            _ProductDataMock = new Mock<IProductData>();
            _ProductDataMock
               .Setup(c => c.GetProducts(It.IsAny<ProductFilter>()))
               .Returns(new[]
                {
                    new Product
                    {
                        Id = 1,
                        Name = "Product 1",
                        Price = 1.1m,
                        Order = 1,
                        ImageUrl = "img_1.png",
                        Brand = new Brand { Id = 1, Name = "Brand 1", Order = 1},
                        SectionId = 1,
                        Section = new Section{ Id = 1, Name = "Section 1", Order = 1 },
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Product 2",
                        Price = 2.2m,
                        Order = 2,
                        ImageUrl = "img_2.png",
                        Brand = new Brand { Id = 2, Name = "Brand 2", Order = 2},
                        SectionId = 2,
                        Section = new Section{ Id = 2, Name = "Section 2", Order = 2 },
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Product 3",
                        Price = 3.3m,
                        Order = 3,
                        ImageUrl = "img_3.png",
                        Brand = new Brand { Id = 3, Name = "Brand 3", Order = 3},
                        SectionId = 3,
                        Section = new Section{ Id = 3, Name = "Section 3", Order = 3 },
                    },
                });

            _CartStoreMock = new Mock<ICartStore>();
            _CartStoreMock.Setup(c => c.Cart).Returns(_Cart);

            _CartService = new CartService(_CartStoreMock.Object, _ProductDataMock.Object);
        }

        [TestMethod]
        public void Cart_Class_ItemsCount_returns_Correct_Quantity()
        {
            var cart = _Cart;

            var expectedItemsCount = cart.Items.Sum(i => i.Quantity);

            var actualItemsCount = cart.ItemsCount;

            Assert.Equal(expectedItemsCount, actualItemsCount);
        }

        [TestMethod]
        public void CartViewModel_Returns_Correct_ItemsCount()
        {
            var cart_view_model = new CartViewModel
            {
                Items = new[]
                {
                    ( new ProductViewModel { Id = 1, Name = "Product 1", Price = 0.5m }, 1 ),
                    ( new ProductViewModel { Id = 2, Name = "Product 2", Price = 1.5m }, 3 ),
                }
            };

            var expectedItemsCount = cart_view_model.Items.Sum(i => i.Quantity);

            var actualItemsCount = cart_view_model.ItemsCount;

            Assert.Equal(expectedItemsCount, actualItemsCount);
        }

        [TestMethod]
        public void CartViewModel_Returns_Correct_TotalPrice()
        {
            var cartViewModel = new CartViewModel
            {
                Items = new[]
                {
                    ( new ProductViewModel { Id = 1, Name = "Product 1", Price = 0.5m }, 1 ),
                    ( new ProductViewModel { Id = 2, Name = "Product 2", Price = 1.5m }, 3 ),
                }
            };

            var expectedTotalPrice = cartViewModel.Items.Sum(item => item.Quantity * item.Product.Price);

            var actualTotalPrice = cartViewModel.TotalPrice;

            Assert.Equal(expectedTotalPrice, actualTotalPrice);
        }

        [TestMethod]
        public void CartService_Add_WorkCorrect()
        {
            _Cart.Items.Clear();

            const int expectedId = 5;
            const int expectedItemsCount = 1;

            _CartService.Add(expectedId);

            var actualItemsCount = _Cart.ItemsCount;

            Assert.Equal(expectedItemsCount, actualItemsCount);

            Assert.Single(_Cart.Items);

            Assert.Equal(expectedId, _Cart.Items.Single().ProductId);
        }

        [TestMethod]
        public void CartService_Remove_Correct_Item()
        {
            const int itemId = 1;
            const int expectedProductId = 2;

            _CartService.Remove(itemId);

            Assert.Single(_Cart.Items);

            Assert.Equal(expectedProductId, _Cart.Items.Single().ProductId);
        }

        [TestMethod]
        public void CartService_Clear_ClearCart()
        {
            _CartService.Clear();

            Assert.Empty(_Cart.Items);
        }

        [TestMethod]
        public void CartService_Decrement_Correct()
        {
            const int itemId = 2;

            const int expectedQuantity = 2;
            const int expectedItemsCount = 3;
            const int expectedProductsCount = 2;

            _CartService.Decrement(itemId);

            Assert.Equal(expectedItemsCount, _Cart.ItemsCount);
            Assert.Equal(expectedProductsCount, _Cart.Items.Count);

            var items = _Cart.Items.ToArray();
            Assert.Equal(itemId, items[1].ProductId);
            Assert.Equal(expectedQuantity, items[1].Quantity);
        }

        [TestMethod]
        public void CartService_Remove_Item_When_Decrement_to_0()
        {
            const int itemId = 1;
            const int expectedItemsCount = 3;

            _CartService.Decrement(itemId);

            Assert.Equal(expectedItemsCount, _Cart.ItemsCount);
            Assert.Single(_Cart.Items);
        }

        [TestMethod]
        public void CartService_GetViewModel_WorkCorrect()
        {
            const int expectedItemsCount = 4;
            const decimal expectedFirstProductPrice = 1.1m;

            var result = _CartService.GetViewModel();

            Assert.Equal(expectedItemsCount, result.ItemsCount);

            Assert.Equal(expectedFirstProductPrice, result.Items.First().Product.Price);
        }
    }
}