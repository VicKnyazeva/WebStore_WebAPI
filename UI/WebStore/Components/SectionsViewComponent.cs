using System.Linq;

using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Components
{
    public class SectionsViewComponent : ViewComponent
    {
        private readonly IProductData _ProductData;
        public SectionsViewComponent(IProductData ProductData)
        {
            _ProductData = ProductData;
        }

        public IViewComponentResult Invoke() 
        {
            var sections = _ProductData.GetSections();

            var parentSections = sections.Where(s => s.ParentId is null);

            var parentSectionsViews = parentSections
                .Select(s => new SectionViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Order = s.Order
                }).ToList();

            foreach (var parent_section in parentSectionsViews)
            {
                var children = sections.Where(s => s.ParentId == parent_section.Id);

                foreach(var child_section in children)
                {
                    parent_section.ChildSections.Add(new SectionViewModel
                    {
                        Id = child_section.Id,
                        Name = child_section.Name,
                        Order = child_section.Order,
                        Parent = parent_section
                    });
                }
                parent_section.ChildSections.Sort((a, b) => a.Order.CompareTo(b.Order));

            }

            parentSectionsViews.Sort((a, b) => a.Order.CompareTo(b.Order));

            return View(parentSectionsViews); 
        }
    }
}
