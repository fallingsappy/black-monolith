using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL;
using WebStore.DomainNew.Entities;
using WebStore.DomainNew.Entities.Base.Filters;
using WebStore.Infrastructure.Interfaces;

namespace WebStore.Infrastructure.Implementation
{
    public class SqlProductData : IProductData
    {
        private readonly WebStoreContext _context;

        public SqlProductData(WebStoreContext context)
        {
            _context = context;
        }
        public IEnumerable<Section> GetSections()
        {
            return _context.Sections;
        }

        public IEnumerable<Brand> GetBrands()
        {
            return _context.Brands;
        }

        public IEnumerable<Product> GetProduct(ProductFilter filter)
        {
            var products = _context.Products.AsQueryable();

            if (filter.SectionId.HasValue)
                products = products.Where(p => p.SectionId == filter.SectionId.Value);
            if (filter.BrandId.HasValue)
                products = products.Where(p => p.BrandId == filter.BrandId.Value);

            return products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Include("Brand").Include("Section").FirstOrDefault(p => p.Id.Equals(id));
        }
    }
}