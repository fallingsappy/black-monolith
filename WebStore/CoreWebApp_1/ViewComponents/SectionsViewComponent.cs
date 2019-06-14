using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebStore.Infrastructure.Interfaces;
using WebStore.Models.Product;

namespace WebStore.ViewComponents
{
    public class SectionsViewComponent : ViewComponent
    {
        private readonly IProductData _productData;

        public SectionsViewComponent(IProductData productData)
        {
            _productData = productData;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sections = GetSections();
            return View(sections);
        }
        private List<SectionViewModel> GetSections()
        {
            var categories = _productData.GetSections();
            var parentCategories = categories.Where(p =>
                !p.ParentId.HasValue).ToArray();
            var parentSections = new List<SectionViewModel>();
            foreach (var parentCategory in parentCategories)
            {
                parentSections.Add(new SectionViewModel()
                {
                    Id = parentCategory.Id,
                    Name = parentCategory.Name,
                    Order = parentCategory.Order,
                    ParentSection = null
                });
            }
            foreach (var sectionViewModel in parentSections)
            {
                var childCategories = categories.Where(c =>
                    c.ParentId.Equals(sectionViewModel.Id));
                foreach (var childCategory in childCategories)
                {
                    sectionViewModel.ChildSections.Add(new SectionViewModel()
                    {
                        Id = childCategory.Id,
                        Name = childCategory.Name,
                        Order = childCategory.Order,
                        ParentSection = sectionViewModel
                    });
                }
                sectionViewModel.ChildSections =
                    sectionViewModel.ChildSections.OrderBy(c => c.Order).ToList();
            }
            parentSections = parentSections.OrderBy(c => c.Order).ToList();
            return parentSections;
        }
    }
}