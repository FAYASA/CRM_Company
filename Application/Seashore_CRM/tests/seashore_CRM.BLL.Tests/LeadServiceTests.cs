using System.Threading.Tasks;
using Xunit;
using Moq;
using seashore_CRM.BLL.Services;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using FluentValidation;
using seashore_CRM.BLL.Services.Service_Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace seashore_CRM.BLL.Tests
{
    public class LeadServiceTests
    {
        [Fact]
        public async Task CreateLeadAsync_Saves_ActivityId_On_Lead()
        {
            // Arrange
            var uowMock = new Mock<IUnitOfWork>();
            var leadItemsRepo = new Mock<ILeadItemRepository>();
            var validatorMock = new Mock<IValidator<LeadDto>>();
            var activityServiceMock = new Mock<IUserActivityService>();
            var httpAccessorMock = new Mock<IHttpContextAccessor>();

            // validator returns success
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<LeadDto>(), default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // setup Leads repository to capture added entity
            var leadsRepoMock = new Mock<ILeadRepository>();
            Lead capturedLead = null;
            leadsRepoMock.Setup(r => r.AddAsync(It.IsAny<Lead>())).Returns<Lead>(l => {
                capturedLead = l;
                l.Id = 123;
                return Task.CompletedTask;
            });

            // setup LeadStatusActivities repo GetById - return a template activity
            var statusActRepo = new Mock<ILeadStatusActivityRepository>();
            statusActRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => new seashore_CRM.Models.Entities.LeadStatusActivity { Id = id, ActivityName = "Demo", LeadStatusId = 1 });

            uowMock.Setup(u => u.Leads).Returns(leadsRepoMock.Object);
            uowMock.Setup(u => u.LeadItems).Returns(leadItemsRepo.Object);
            uowMock.Setup(u => u.LeadStatusActivities).Returns(statusActRepo.Object);

            var svc = new LeadService(uowMock.Object, leadItemsRepo.Object, validatorMock.Object, activityServiceMock.Object, httpAccessorMock.Object);

            var dto = new LeadDto
            {
                LeadType = "Corporate",
                Priority = "Hot",
                ActivityId = 42
            };

            // Act
            var (result, id) = await svc.CreateLeadAsync(dto);

            // Assert
            Assert.Equal("Success", result);
            Assert.Equal(123, id);
            Assert.NotNull(capturedLead);
            Assert.Equal(42, capturedLead.ActivityId);
        }
    }
}
