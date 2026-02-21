using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class OpportunityService : IOpportunityService
    {
        private readonly IUnitOfWork _uow;

        public OpportunityService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(Opportunity entity)
        {
            await _uow.Opportunities.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Opportunities.GetByIdAsync(id);
            if (e == null) return;
            _uow.Opportunities.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Opportunity>> GetAllAsync()
        {
            return await _uow.Opportunities.GetAllAsync();
        }

        public async Task<Opportunity?> GetByIdAsync(int id)
        {
            return await _uow.Opportunities.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Opportunity entity)
        {
            _uow.Opportunities.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
