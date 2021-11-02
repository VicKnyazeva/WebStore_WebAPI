using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WebStore.Controllers;

using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void Index_Returns_View()
        {
            var controller = new HomeController();

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void ContactUs_Returns_View()
        {
            var controller = new HomeController();

            var result = controller.Contacts();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void Status_with_id_404_Returns_View()
        {
            #region Arrange

            const string id = "404";
            const string expectedViewName = "Error404";
            var controller = new HomeController();

            #endregion

            #region Act

            var result = controller.Status(id);

            #endregion

            #region Assert

            var viewResult = Assert.IsType<ViewResult>(result);

            var actualViewName = viewResult.ViewName;

            Assert.Equal(expectedViewName, actualViewName);

            #endregion
        }

        [TestMethod]
        [DataRow("123")]
        [DataRow("QWE")]
        public void Status_with_id_123_Returns_View(string id)
        {
            var expectedContent = "Status --- " + id;
            var controller = new HomeController();

            var result = controller.Status(id);

            var contentResult = Assert.IsType<ContentResult>(result);

            var actualContent = contentResult.Content;

            Assert.Equal(expectedContent, actualContent);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Status_thrown_ArgumentNullException_when_id_is_null_1()
        {
            var controller = new HomeController();

            _ = controller.Status(null);
        }

        [TestMethod]
        public void Status_thrown_ArgumentNullException_when_id_is_null_2()
        {
            const string expectedParameterName = "id";
            var controller = new HomeController();

            Exception exception = null;

            try
            {
                _ = controller.Status(null);
            }
            catch (ArgumentNullException e)
            {
                exception = e;
            }

            if (exception is null)
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();

            var actualException = Assert.IsType<ArgumentNullException>(exception);
            var actualParameterName = actualException.ParamName;
            Assert.Equal(expectedParameterName, actualParameterName);
        }

        [TestMethod]
        public void Status_thrown_ArgumentNullException_when_id_is_null_3()
        {
            const string expectedParameterName = "id";
            var controller = new HomeController();

            var actualException = Assert.Throws<ArgumentNullException>(() => controller.Status(null));
            var actualParameterName = actualException.ParamName;
            Assert.Equal(expectedParameterName, actualParameterName);
        }
    }
}