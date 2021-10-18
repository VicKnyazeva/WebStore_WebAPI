using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using WebStore.DAL.Context;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Services.Interfaces;

namespace WebStore.Services.InSQL
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

        public IEnumerable<Product> GetProducts(ProductFilter Filter = null)
        {
            IQueryable<Product> query = _db.Products.Include(p => p.Brand).Include(p => p.Section);

            if (Filter?.Ids?.Length > 0)
            {
                query = query.Where(product => Filter.Ids.Contains(product.Id));
            }
            else
            {
                if (Filter?.SectionId != null)
                    query = query.Where(p => p.SectionId == Filter.SectionId);

                if (Filter?.BrandId != null)
                    query = query.Where(p => p.BrandId == Filter.BrandId);
            }
            return query;
        }

        public IEnumerable<Section> GetSections()
        {
            return _db.Sections;
        }

        public Section GetSectionById(int id) => _db.Sections.FirstOrDefault(s => s.Id == id);

        public Brand GetBrandById(int id) => _db.Brands.Find(id);
    }
}
