using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class LeadSourceService : ILeadSourceService
    {
        private readonly IUnitOfWork _uow;

        public LeadSourceService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(LeadSource entity)
        {
            await _uow.LeadSources.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.LeadSources.GetByIdAsync(id);
            if (e == null) return;
            _uow.LeadSources.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<LeadSource>> GetAllAsync()
        {
            return await _uow.LeadSources.GetAllAsync();
        }

        public async Task<LeadSource?> GetByIdAsync(int id)
        {
            return await _uow.LeadSources.GetByIdAsync(id);
        }

        public async Task UpdateAsync(LeadSource entity)
        {
            _uow.LeadSources.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
