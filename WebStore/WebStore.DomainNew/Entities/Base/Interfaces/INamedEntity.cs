namespace WebStore.DomainNew.Entities.Base.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Entity with name
    /// </summary>
    public interface INamedEntity : IBaseEntity
    {
        /// <summary>
        /// Наименование
        /// </summary>
        string Name { get; set; }
    }
}