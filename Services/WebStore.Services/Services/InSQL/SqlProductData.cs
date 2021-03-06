using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using WebStore.DAL.Context;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.Services.Services.InSQL
{
    public class SqlProductData : IProductData
    {
        private readonly WebStoreDB _db;
        public SqlProductData(WebStoreDB db)
        {
            _db = db;
        }

        public IEnumerable<Brand> GetBrands()
        {
            return _db.Brands;
        }

        public Product GetProductById(int id)
        {
            return _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Section)
                .FirstOrDefault(p => p.Id == id);
        }

        public ProductsPage GetProducts(ProductFilter Filter = null)
        {
            IQueryable<Product> query = _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Section);

            if (Filter?.Ids?.Length > 0)
                query = query.Where(product => Filter.Ids.Contains(product.Id));
            else
            {
                if (Filter?.SectionId is { } sectionId)
                    query = query.Where(p => p.SectionId == sectionId);

                if (Filter?.BrandId is { } brandId)
                    query = query.Where(p => p.BrandId == brandId);
            }

            var totalCount = query.Count();

            if (Filter is { PageSize: > 0 and var pageSize, Page: > 0 and var pageNumber })
                query = query
                    .OrderBy(p => p.Order)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

            return new(query.AsEnumerable(), totalCount);
        }

        public IEnumerable<Section> GetSections()
        {
            return _db.Sections;
        }

        public Section GetSectionById(int id) => _db.Sections.FirstOrDefault(s => s.Id == id);

        public Brand GetBrandById(int id) => _db.Brands.Find(id);
    }
}
