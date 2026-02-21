using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(User entity)
        {
            await _uow.Users.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Users.GetByIdAsync(id);
            if (e == null) return;
            _uow.Users.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _uow.Users.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _uow.Users.GetByIdAsync(id);
        }

        public async Task UpdateAsync(User entity)
        {
            _uow.Users.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
