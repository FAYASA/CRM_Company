using AutoMapper;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var e = await _unitOfWork.Categories.GetByIdAsync(id);
            if (e == null) return;
            _unitOfWork.Categories.Remove(e);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var e = await _unitOfWork.Categories.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException($"Category with id {id} not found");
            return e;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CommitAsync();
        }
    }
}
