using seashore_CRM.ApplicationLayer.DTOs;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq;
using System;

namespace seashore_CRM.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductUpdateDto> _update_validator;

        public ProductService(IUnitOfWork uow, IValidator<ProductCreateDto> createValidator, IValidator<ProductUpdateDto> updateValidator)
        {
            _uow = uow;
            _createValidator = createValidator;
            _update_validator = updateValidator;
        }
        IQueryable<ProductListDto> IProductService.GetAllExceptInactive()
        {
            var products = _uow.Products.GetAllExceptInactive();
            var dto = products.Select(p => new ProductListDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                CategoryName = p.Category.CategoryName,
                ProductGroupName = p.ProductGroup.GroupName,
                Cost = p.Cost,
                TaxPercentage = p.TaxPercentage,
                IsActive = p.IsActive
            });
            return dto;
        }
        IQueryable<ProductListDto> IProductService.GetAllAsync()
        {
            var products = _uow.Products.GetAllAsync();
            var dto = products.Select(p => new ProductListDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                CategoryName = p.Category.CategoryName,
                ProductGroupName = p.ProductGroup.GroupName,
                Cost = p.Cost,
                TaxPercentage = p.TaxPercentage,
                IsActive = p.IsActive
            });
            return dto;
        }

        public IQueryable<ProductListDto> SearchAsync(string? query)
        {
            var products = _uow.Products.GetAllAsync();
            if (!string.IsNullOrEmpty(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                products = products.Where(p =>
                    p.ProductName.ToLower().Contains(normalizedQuery) ||
                    p.Category.CategoryName.ToLower().Contains(normalizedQuery) ||
                    p.ProductGroup.GroupName.ToLower().Contains(normalizedQuery));
            }
            var dto = products.Select(p => new ProductListDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                CategoryName = p.Category.CategoryName,
                ProductGroupName = p.ProductGroup.GroupName,
                Cost = p.Cost,
                TaxPercentage = p.TaxPercentage,
                IsActive = p.IsActive
            });
            return dto;
        }

        public async Task<ProductDetailDto?> GetByIdAsync(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);

            if (product == null)
                return null;

            var dto = new ProductDetailDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName ?? string.Empty,
                ProductGroupId = product.ProductGroupId,
                ProductGroupName = product.ProductGroup?.GroupName ?? string.Empty,
                Cost = product.Cost,
                TaxPercentage = product.TaxPercentage,
                IsActive = product.IsActive,
                CreatedDate = product.CreatedDate,
                CreatedBy = product.CreatedBy,
                UpdatedDate = product.UpdatedDate,
                UpdatedBy = product.UpdatedBy
            };

            return dto;
        }

        public async Task<int> CreateAsync(ProductCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var vResult = await _createValidator.ValidateAsync(dto);
            if (!vResult.IsValid) throw new ValidationException(vResult.Errors);

            var product = new Product
            {
                ProductName = dto.ProductName,
                CategoryId = dto.CategoryId,
                ProductGroupId = dto.ProductGroupId,
                Cost = dto.Cost,
                TaxPercentage = dto.TaxPercentage,
                IsActive = true
            };

            await _uow.Products.AddAsync(product);
            await _uow.CommitAsync();

            return product.Id;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var vResult = await _update_validator.ValidateAsync(dto);
            if (!vResult.IsValid) throw new ValidationException(vResult.Errors);

            var product = await _uow.Products.GetByIdAsync(dto.Id);

            if (product == null)
                return false;

            product.ProductName = dto.ProductName;
            product.CategoryId = dto.CategoryId;
            product.ProductGroupId = dto.ProductGroupId;
            product.Cost = dto.Cost;
            product.TaxPercentage = dto.TaxPercentage;
            product.IsActive = dto.IsActive;

            _uow.Products.Update(product);
            await _uow.CommitAsync();

            return true;
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);

            if (product == null)
                return;

            // Soft delete
            product.IsActive = false;

            _uow.Products.Update(product);
            await _uow.CommitAsync();
        }

        async Task<IEnumerable<ProductDetailDto>> IProductService.GetByCategoryIdAsync(int categoryId)
        {
            var products = await _uow.Products.GetByCategoryIdAsync(categoryId);
            var Entity = products.Select(p => new ProductDetailDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                ProductGroupId = p.ProductGroupId,
                Cost = p.Cost,
                TaxPercentage = p.TaxPercentage,
                IsActive = p.IsActive
            });
            return Entity;
        }
    }
}
