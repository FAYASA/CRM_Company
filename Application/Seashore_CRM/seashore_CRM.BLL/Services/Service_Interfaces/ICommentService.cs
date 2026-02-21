using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetAllAsync();
        Task<Comment?> GetByIdAsync(int id);
        Task<int> AddAsync(Comment entity);
        Task UpdateAsync(Comment entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<Comment>> GetByEntityIdAsync(int entityId);
    }
}
