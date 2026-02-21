using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class LeadItemService : ILeadItemService
    {
        private readonly IUnitOfWork _uow;

        public LeadItemService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(LeadItem entity)
        {
            await _uow.LeadItems.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.LeadItems.GetByIdAsync(id);
            if (e == null) return;
            _uow.LeadItems.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<LeadItem>> GetAllAsync()
        {
            return await _uow.LeadItems.GetAllAsync();
        }

        public async Task<LeadItem?> GetByIdAsync(int id)
        {
            return await _uow.LeadItems.GetByIdAsync(id);
        }

        public async Task UpdateAsync(LeadItem entity)
        {
            _uow.LeadItems.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
