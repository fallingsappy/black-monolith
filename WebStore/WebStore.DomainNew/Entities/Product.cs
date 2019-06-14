using System.ComponentModel.DataAnnotations.Schema;
using WebStore.DomainNew.Entities.Base;
using WebStore.DomainNew.Entities.Base.Interfaces;

namespace WebStore.DomainNew.Entities
{
    /// <inheritdoc cref="NamedEntity" />
    /// <summary>
    /// Сущность продукт
    /// </summary>
    [Table("Products")]
    public class Product : NamedEntity, IOrderedEntity
    {
        public int Order { get; set; }

        /// <summary>
        /// Секция, к которой принадлежит товар
        /// </summary>
        public int SectionId { get; set; }

        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }

        /// <summary>
        /// Бренд товара
        /// </summary>
        public int? BrandId { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }

        /// <summary>
        /// Ссылка на картинку
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }
    }
}