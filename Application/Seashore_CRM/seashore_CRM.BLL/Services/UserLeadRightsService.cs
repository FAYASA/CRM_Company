using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class UserLeadRightsService : IUserLeadRightsService
    {
        private readonly IUnitOfWork _uow;

        public UserLeadRightsService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<UserLeadRights>> ListAsync(int? leadId = null, int? userId = null)
        {
            var q = _uow.UserLeadRights; // repository returns IQueryable via GetAllAsync
            var items = await q.GetAllAsync();
            var list = items.AsQueryable();
            if (leadId.HasValue) list = list.Where(x => x.LeadId == leadId.Value);
            if (userId.HasValue) list = list.Where(x => x.UserId == userId.Value);
            return list.ToList();
        }

        public async Task<UserLeadRights?> GetAsync(int id)
        {
            return await _uow.UserLeadRights.GetByIdAsync(id);
        }

        public async Task<int> SaveAsync(UserLeadRights entity)
        {
            if (entity.Id > 0)
            {
                _uow.UserLeadRights.Update(entity);
                await _uow.CommitAsync();
                return entity.Id;
            }
            else
            {
                await _uow.UserLeadRights.AddAsync(entity);
                await _uow.CommitAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _uow.UserLeadRights.GetByIdAsync(id);
            if (e == null) return false;
            _uow.UserLeadRights.Remove(e);
            await _uow.CommitAsync();
            return true;
        }
    }
}