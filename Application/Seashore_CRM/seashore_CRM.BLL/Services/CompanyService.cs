using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _uow;

        public CompanyService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(Company entity)
        {
            await _uow.Companies.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Companies.GetByIdAsync(id);
            if (e == null) return;
            _uow.Companies.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _uow.Companies.GetAllAsync();
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _uow.Companies.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Company entity)
        {
            _uow.Companies.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
