using seashore_CRM.ApplicationLayer.DTOs;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IProductService
    {

        IQueryable<ProductListDto> GetAllAsync();
        IQueryable<ProductListDto> SearchAsync(string? query);
        Task<ProductDetailDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ProductCreateDto dto);
        Task<bool> UpdateAsync(ProductUpdateDto dto);
        Task DeleteAsync(int id);

        //Task DeleteAsync(int id);
        Task<IEnumerable<ProductDetailDto>> GetByCategoryIdAsync(int categoryId);
    }
}
