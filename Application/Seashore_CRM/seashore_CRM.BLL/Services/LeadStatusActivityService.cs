using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class LeadStatusActivityService : ILeadStatusActivityService
    {
        private readonly IUnitOfWork _uow;

        public LeadStatusActivityService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(LeadStatusActivity entity)
        {
            await _uow.LeadStatusActivities.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.LeadStatusActivities.GetByIdAsync(id);
            if (e == null) return;
            _uow.LeadStatusActivities.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<LeadStatusActivity>> GetAllAsync()
        {
            return await _uow.LeadStatusActivities.GetAllAsync();
        }

        public async Task<LeadStatusActivity?> GetByIdAsync(int id)
        {
            return await _uow.LeadStatusActivities.GetByIdAsync(id);
        }

        public async Task UpdateAsync(LeadStatusActivity entity)
        {
            _uow.LeadStatusActivities.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
