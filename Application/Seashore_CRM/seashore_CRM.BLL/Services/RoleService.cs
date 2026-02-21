using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _uow;

        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(Role entity)
        {
            await _uow.Roles.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Roles.GetByIdAsync(id);
            if (e == null) return;
            _uow.Roles.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _uow.Roles.GetAllAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _uow.Roles.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Role entity)
        {
            _uow.Roles.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
