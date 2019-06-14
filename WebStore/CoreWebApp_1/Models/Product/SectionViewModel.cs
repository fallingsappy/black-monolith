using System.Collections.Generic;
using WebStore.DomainNew.Entities.Base.Interfaces;

namespace WebStore.Models.Product
{
    public class SectionViewModel : INamedEntity, IOrderedEntity
    {
        public SectionViewModel()
        {
            ChildSections = new List<SectionViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        /// <summary>
        /// Дочерние секции
        /// </summary>
        public List<SectionViewModel> ChildSections { get; set; }
        /// <summary>
        /// Родительская секция
        /// </summary>
        public SectionViewModel ParentSection { get; set; }
    }
}