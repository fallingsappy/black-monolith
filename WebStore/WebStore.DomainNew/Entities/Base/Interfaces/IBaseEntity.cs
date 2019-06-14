namespace WebStore.DomainNew.Entities.Base.Interfaces
{
    /// <summary>
    /// Base entity with Id
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        int Id { get; set; }
    }
}