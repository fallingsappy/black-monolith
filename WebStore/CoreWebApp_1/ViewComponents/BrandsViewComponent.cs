using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebStore.DomainNew.Entities.Base.Filters;
using WebStore.Infrastructure.Interfaces;
using WebStore.Models.Product;

namespace WebStore.ViewComponents
{
    public class BrandsViewComponent : ViewComponent
    {
        private readonly IProductData _productData;

        private int GetProductCount(int id)
        {
            var products = _productData.GetProduct(new ProductFilter
            {
                BrandId = id,
            });
            return products.Count();
        }

        public BrandsViewComponent(IProductData productData)
        {
            _productData = productData;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var brands = GetBrands();
            return View(brands);
        }
        private IEnumerable<BrandViewModel> GetBrands()
        {
            var dbBrands = _productData.GetBrands();
            return dbBrands.Select(b => new BrandViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Order = b.Order,
                ProductsCount = GetProductCount(b.Id)
            }).OrderBy(b => b.Order).ToList();
        }
    }
}