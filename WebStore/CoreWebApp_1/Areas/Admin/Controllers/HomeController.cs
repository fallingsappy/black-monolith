using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.DomainNew.Entities.Base.Filters;
using WebStore.Infrastructure.Interfaces;

namespace WebStore.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Administrator" )]
    public class HomeController : Controller
    {
        private readonly IProductData _productData;
        public HomeController(IProductData productData)
        {
            _productData = productData;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductList()
        {
            var products = _productData.GetProduct(new ProductFilter());
            return View(products);
        }
    }
}