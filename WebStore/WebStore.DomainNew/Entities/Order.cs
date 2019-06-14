using System;
using System.Collections.ObjectModel;
using WebStore.DomainNew.Entities.Base;

namespace WebStore.DomainNew.Entities
{
    public class Order : NamedEntity
    {
        public virtual User User { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public virtual Collection<OrderItem> OrderItems { get; set; }
    }
}
