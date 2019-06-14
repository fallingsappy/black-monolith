using WebStore.DomainNew.Entities.Base.Interfaces;

namespace WebStore.DomainNew.Entities.Base
{
    public class NamedEntity : INamedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}