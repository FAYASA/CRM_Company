using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _uow;

        public ActivityService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(Activity entity)
        {
            await _uow.Activities.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Activities.GetByIdAsync(id);
            if (e == null) return;
            _uow.Activities.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Activity>> GetAllAsync()
        {
            return await _uow.Activities.GetAllAsync();
        }

        public async Task<Activity?> GetByIdAsync(int id)
        {
            return await _uow.Activities.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Activity entity)
        {
            _uow.Activities.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
