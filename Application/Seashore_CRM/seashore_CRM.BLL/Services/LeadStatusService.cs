using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class LeadStatusService : ILeadStatusService
    {
        private readonly IUnitOfWork _uow;

        public LeadStatusService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(LeadStatus entity)
        {
            await _uow.LeadStatuses.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.LeadStatuses.GetByIdAsync(id);
            if (e == null) return;
            _uow.LeadStatuses.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<LeadStatus>> GetAllAsync()
        {
            return await _uow.LeadStatuses.GetAllAsync();
        }

        public async Task<LeadStatus?> GetByIdAsync(int id)
        {
            return await _uow.LeadStatuses.GetByIdAsync(id);
        }

        public async Task UpdateAsync(LeadStatus entity)
        {
            _uow.LeadStatuses.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
