using System.Threading.Tasks;
using Xunit;
using Moq;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.BLL.Services;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using AutoMapper;
using seashore_CRM.BLL.Mapping;

namespace seashore_CRM.Tests
{
    public class LeadServiceTests
    {
        private readonly IMapper _mapper;

        public LeadServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task GetAllLeadsAsync_ReturnsLeads()
        {
            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.Leads.GetAllAsync()).ReturnsAsync(new List<Lead> { new Lead { Id = 1, LeadType = "Corporate" } });

            var service = new LeadService(mockUow.Object, _mapper);
            var result = await service.GetAllLeadsAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }
    }
}
