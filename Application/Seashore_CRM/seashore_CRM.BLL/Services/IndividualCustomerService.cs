using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.DomainModelLayer.Entities;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace seashore_CRM.BLL.Services
{
    public class IndividualCustomerService : IIndividualCustomerService
    {
        private readonly IUnitOfWork _uow;

        public IndividualCustomerService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IQueryable<IndividualCustomerListDto> GetAllExceptInactive()
        {
            var customers = _uow.IndividualCustomers.GetAllExceptInactive();
            return customers.Select(c => new IndividualCustomerListDto
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,
                Phone = c.Phone,
                Email = c.Email
            });
        }
        public IQueryable<IndividualCustomerListDto> GetAll()
        {
            var customers = _uow.IndividualCustomers.GetAllAsync();
            return customers.Select(c => new IndividualCustomerListDto
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,
                Phone = c.Phone,
                Email = c.Email
            });
        }

        public async Task<IndividualCustomerDetailDto?> GetByIdAsync(int id)
        {
            var c = await _uow.IndividualCustomers.GetByIdAsync(id);
            if (c == null) return null;
            return new IndividualCustomerDetailDto
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,
                Phone = c.Phone,
                Email = c.Email
            };
        }

        public async Task<int> CreateAsync(IndividualCustomerCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = new IndividualCustomer
            {
                Name = dto.Name,
                Location = dto.Location,
                Phone = dto.Phone,
                Email = dto.Email
            };

            await _uow.IndividualCustomers.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(IndividualCustomerUpdateDto dto)
        {
            var entity = await _uow.IndividualCustomers.GetByIdAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Individual customer not found");

            entity.Name = dto.Name;
            entity.Location = dto.Location;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;

            _uow.IndividualCustomers.Update(entity);
            await _uow.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _uow.IndividualCustomers.GetByIdAsync(id);
            if (entity == null) return;
            _uow.IndividualCustomers.Remove(entity);
            await _uow.CommitAsync();
        }
    }
}
