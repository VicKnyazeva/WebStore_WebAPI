using System.Collections.Generic;

using WebStore.Domain;
using WebStore.Domain.Entities;

namespace WebStore.Interfaces.Services
{
    public interface IProductData
    {
        IEnumerable<Section> GetSections();
        IEnumerable<Brand> GetBrands();
        ProductsPage GetProducts(ProductFilter Filter = null);
        Product GetProductById(int id);
        Section GetSectionById(int id);
        Brand GetBrandById(int id);
    }
}
