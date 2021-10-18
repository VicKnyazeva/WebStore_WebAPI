
using System.Collections.Generic;
using System.Linq;

using WebStore.Data;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Services.Interfaces;

namespace WebStore.Services.InMemory
{
    public class InMemoryProductData : IProductData
    {
        public IEnumerable<Brand> GetBrands()
        {
            return TestData.Brands;
        }

        public IEnumerable<Section> GetSections()
        {
            return TestData.Sections;
        }

        public Section GetSectionById(int id) => TestData.Sections.FirstOrDefault(s => s.Id == id);

        public Brand GetBrandById(int id) => TestData.Brands.FirstOrDefault(b => b.Id == id);

        public IEnumerable<Product> GetProducts(ProductFilter Filter = null)
        {
            IEnumerable<Product> query = TestData.Products;

            if (Filter?.SectionId != null)
                query = query.Where(p => p.SectionId == Filter.SectionId);

            if (Filter?.BrandId != null)
                query = query.Where(p => p.BrandId == Filter.BrandId);

            return query;
        }

        public Product GetProductById(int id)
        {
            return TestData.Products.FirstOrDefault(p => p.Id == id);
        }
    }
}
