using System.Linq;
using System.Net;
using System.Threading.Tasks;

using AngleSharp.Dom;
using AngleSharp.Html.Parser;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Interfaces.TestAPI;

using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class WebAPIIntegrationTest
    {
        private readonly string[] _ExpectedValues = Enumerable.Range(1, 10).Select(i => $"TestValue - {i}").ToArray();

        private WebApplicationFactory<Startup> _Host;

        [TestInitialize]
        public void Initialize()
        {
            var valuesServiceMock = new Mock<IValuesService>();
            valuesServiceMock.Setup(s => s.GetAll()).Returns(_ExpectedValues);

            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock.Setup(c => c.GetViewModel()).Returns(() => new CartViewModel { Items = Enumerable.Empty<(ProductViewModel, int)>() });

            _Host = new WebApplicationFactory<Startup>()
               .WithWebHostBuilder(host => host
                   .ConfigureServices(services => services
                       .AddSingleton(valuesServiceMock.Object)
                       .AddSingleton(cartServiceMock.Object)));
        }

        [TestMethod]
        public async Task GetValues()
        {
            var client = _Host.CreateClient();

            var response = await client.GetAsync("/WebAPI");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var parser = new HtmlParser();

            var contentStream = await response.Content.ReadAsStreamAsync();
            var html = parser.ParseDocument(contentStream);

            var items = html.QuerySelectorAll(".container table.table tbody tr td:last-child");

            var actualValues = items.Select(item => item.Text());

            Assert.Equal(_ExpectedValues, actualValues);
        }
    }
}