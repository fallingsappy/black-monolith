using System.Threading.Tasks;
using DDrop.Db.DbEntities;

namespace DDrop.DAL
{
    public interface IDDropRepository
    {
        Task UpdateUserAsync(DbUser user);
        Task<DbUser> GetUserByLoginAndPassword(string email);
    }
}