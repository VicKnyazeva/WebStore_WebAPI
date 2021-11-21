using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using WebStore.Domain;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Services.Mapping;

namespace WebStore.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductData _ProductData;
        private readonly IConfiguration _Configuration;

        public CatalogController(IProductData ProductData, IConfiguration Configuration)
        {
            _ProductData = ProductData;
            _Configuration = Configuration;
        }

        public IActionResult Index(int? BrandId, int? SectionId, int Page = 1, int? PageSize = null)
        {
            var pageSize = PageSize ?? (int.TryParse(_Configuration["CatalogPageSize"], out var value) ? value : null);

            var filter = new ProductFilter
            {
                BrandId = BrandId,
                SectionId = SectionId,
                Page = Page,
                PageSize = pageSize,
            };

            var (products, totalCount) = _ProductData.GetProducts(filter);

            var viewModel = new CatalogViewModel
            {
                BrandId = BrandId,
                SectionId = SectionId,
                Products = products.OrderBy(p => p.Order).ToView(),
                PageViewModel = new()
                {
                    Page = Page,
                    PageSize = pageSize ?? 0,
                    TotalItems = totalCount
                }
            };

            return View(viewModel);
        }

        public IActionResult Details(int Id)
        {
            var product = _ProductData.GetProductById(Id);

            if (product is null)
                return NotFound();

            return View(product.ToView());
        }
    }
}