using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _uow;

        public ContactService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(Contact entity)
        {
            await _uow.Contacts.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Contacts.GetByIdAsync(id);
            if (e == null) return;
            _uow.Contacts.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await _uow.Contacts.GetAllAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _uow.Contacts.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId)
        {
            return await _uow.Contacts.GetByCompanyIdAsync(companyId);
        }

        public async Task UpdateAsync(Contact entity)
        {
            _uow.Contacts.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
