using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Components
{
    public class BreadCrumbsViewComponent : ViewComponent
    {
        private readonly IProductData _ProductData;

        public BreadCrumbsViewComponent(IProductData ProductData) => _ProductData = ProductData;

        public IViewComponentResult Invoke()
        {
            var model = new BreadCrumbsViewModel();

            if (int.TryParse(Request.Query["SectionId"], out var sectionId))
            {
                model.Section = _ProductData.GetSectionById(sectionId);
                if (model.Section.ParentId is { } parentSectionId)
                    model.Section.Parent = _ProductData.GetSectionById(parentSectionId);
            }

            if (int.TryParse(Request.Query["BrandId"], out var brandId))
                model.Brand = _ProductData.GetBrandById(brandId);

            if (int.TryParse(ViewContext.RouteData.Values["id"]?.ToString(), out var productId))
                model.Product = _ProductData.GetProductById(productId)?.Name;

            return View(model);
        }
    }
}