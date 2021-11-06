using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.ViewModels;

namespace WebStore.Components
{
    public class SectionsViewComponent : ViewComponent
    {
        private readonly IProductData _ProductData;

        public SectionsViewComponent(IProductData ProductData) => _ProductData = ProductData;

        public IViewComponentResult Invoke(string SectionId)
        {
            var sectionId = int.TryParse(SectionId, out var id) ? id : (int?)null;

            var sections = GetSections(sectionId, out var parentSectionId);

            return View(new SelectableSectionsViewModel
            {
                Sections = sections,
                SectionId = sectionId,
                ParentSectionId = parentSectionId,
            });
        }

        private IEnumerable<SectionViewModel> GetSections(int? SectionId, out int? ParentSectionId)
        {
            ParentSectionId = null;

            var sections = _ProductData.GetSections();

            var parentSections = sections.Where(s => s.ParentId is null);

            var parentSectionsViews = parentSections
               .Select(s => new SectionViewModel
               {
                   Id = s.Id,
                   Name = s.Name,
                   Order = s.Order,
               })
               .ToList();

            foreach (var parentSection in parentSectionsViews)
            {
                var childElements = sections.Where(s => s.ParentId == parentSection.Id);

                foreach (var childSection in childElements)
                {
                    if (childSection.Id == SectionId)
                        ParentSectionId = childSection.ParentId;

                    parentSection.ChildSections.Add(new SectionViewModel
                    {
                        Id = childSection.Id,
                        Name = childSection.Name,
                        Order = childSection.Order,
                        Parent = parentSection
                    });
                }

                parentSection.ChildSections.Sort((a, b) => Comparer<int>.Default.Compare(a.Order, b.Order));
            }

            parentSectionsViews.Sort((a, b) => Comparer<int>.Default.Compare(a.Order, b.Order));

            return parentSectionsViews;
        }
    }
}