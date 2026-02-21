using seashore_CRM.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ICategoryService
    {
         Task<IEnumerable<Category>> GetAllCategoriesAsync();
         Task<Category> GetCategoryByIdAsync(int id);
         Task AddCategoryAsync(Category category);
         Task UpdateCategoryAsync(Category category);
         Task DeleteCategoryAsync(int id);  
    }
}
