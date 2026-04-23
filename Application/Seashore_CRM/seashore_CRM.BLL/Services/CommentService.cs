using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using System;

namespace seashore_CRM.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<Comment> _validator;

        public CommentService(IUnitOfWork uow, IValidator<Comment> validator)
        {
            _uow = uow;
            _validator = validator;
        }

        public async Task<int> AddAsync(Comment entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var v = await _validator.ValidateAsync(entity);
            if (!v.IsValid) throw new ValidationException(v.Errors);

            await _uow.Comments.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Comments.GetByIdAsync(id);
            if (e == null) return;
            _uow.Comments.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _uow.Comments.GetAllAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _uow.Comments.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Comment>> GetByEntityIdAsync(int entityId)
        {
            return await _uow.Comments.GetByEntityIdAsync(entityId);
        }

        public async Task UpdateAsync(Comment entity)
        {
            var v = await _validator.ValidateAsync(entity);
            if (!v.IsValid) throw new ValidationException(v.Errors);
            _uow.Comments.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
