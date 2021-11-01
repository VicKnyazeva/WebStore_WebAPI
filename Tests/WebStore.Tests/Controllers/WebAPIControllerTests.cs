using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using WebStore.Controllers;
using WebStore.Interfaces.TestAPI;

using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class WebAPIControllerTests
    {
        [TestMethod]
        public void Index_returns_with_DataValues()
        {
            var data = Enumerable.Range(1, 10)
               .Select(i => $"Value - {i}")
               .ToArray();

            Debug.WriteLine("Вывод данных в процессе тестирования " + data.Length);

            var valuesServiceMock = new Mock<IValuesService>();
            valuesServiceMock
               .Setup(c => c.GetAll())
               .Returns(data);

            var controller = new WebAPIController(valuesServiceMock.Object);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<IEnumerable<string>>(viewResult.Model);

            var i = 0;
            foreach (var actualValue in model)
            {
                var expectedValue = data[i++];
                Assert.Equal(expectedValue, actualValue);
            }

            valuesServiceMock.Verify(s => s.GetAll());
            valuesServiceMock.VerifyNoOtherCalls();
        }
    }
}