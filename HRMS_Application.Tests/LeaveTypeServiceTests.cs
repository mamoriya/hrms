using HRMS_Application.Data;
using HRMS_Application.Models;
using HRMS_Application.Repositories;
using HRMS_Application.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRMS_Application.Tests
{
    public class LeaveTypeServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateLeaveType_WithValidData_ShouldCreateLeaveType()
        {
            using var context = CreateContext();

            var repository = new LeaveTypeRepository(context);
            var service = new LeaveTypeService(repository);

            var leaveType = new LeaveType
            {
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = true
            };

            var result = await service.CreateLeaveType(leaveType);

            Assert.NotNull(result);
            Assert.Equal("Casual Leave", result.Name);
            Assert.Equal(12, result.AnnualQuota);
            Assert.True(result.CarryForward);
        }

        [Fact]
        public async Task CreateLeaveType_WithEmptyName_ShouldNotCreateLeaveType()
        {
            using var context = CreateContext();

            var repository = new LeaveTypeRepository(context);
            var service = new LeaveTypeService(repository);

            var leaveType = new LeaveType
            {
                Name = "",
                AnnualQuota = 12,
                CarryForward = true
            };

            var result = await service.CreateLeaveType(leaveType);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLeaveType_WithInvalidAnnualQuota_ShouldNotCreateLeaveType()
        {
            using var context = CreateContext();

            var repository = new LeaveTypeRepository(context);
            var service = new LeaveTypeService(repository);

            var leaveType = new LeaveType
            {
                Name = "Casual Leave",
                AnnualQuota = 0,
                CarryForward = true
            };

            var result = await service.CreateLeaveType(leaveType);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLeaveType_WithDuplicateName_ShouldNotCreateLeaveType()
        {
            using var context = CreateContext();

            context.LeaveTypes.Add(new LeaveType
            {
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = true
            });

            await context.SaveChangesAsync();

            var repository = new LeaveTypeRepository(context);
            var service = new LeaveTypeService(repository);

            var leaveType = new LeaveType
            {
                Name = "Casual Leave",
                AnnualQuota = 10,
                CarryForward = false
            };

            var result = await service.CreateLeaveType(leaveType);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLeaveType_WithNullData_ShouldNotCreateLeaveType()
        {
            using var context = CreateContext();

            var repository = new LeaveTypeRepository(context);
            var service = new LeaveTypeService(repository);

            var result = await service.CreateLeaveType(null);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateLeaveType_WithValidData_ShouldUpdateLeaveType()
        {
            using var context = CreateContext();

            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = true
            });

            await context.SaveChangesAsync();

            var repository = new LeaveTypeRepository(context);
            var service = new LeaveTypeService(repository);

            var updatedLeaveType = new LeaveType
            {
                Name = "Updated Leave",
                AnnualQuota = 15,
                CarryForward = false
            };

            var result =
                await service.UpdateLeaveType(1, updatedLeaveType);

            Assert.NotNull(result);
            Assert.Equal("Updated Leave", result.Name);
            Assert.Equal(15, result.AnnualQuota);
            Assert.False(result.CarryForward);
        }

        [Fact]
        public async Task UpdateLeaveType_WithInvalidId_ShouldNotUpdateLeaveType()
        {
            using var context = CreateContext();

            var repository = new LeaveTypeRepository(context);
            var service = new LeaveTypeService(repository);

            var leaveType = new LeaveType
            {
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = true
            };

            var result =
                await service.UpdateLeaveType(999, leaveType);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteLeaveType_WithValidId_ShouldDeleteLeaveType()
        {
            using var context = CreateContext();

            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = true
            });

            await context.SaveChangesAsync();

            var repository = new LeaveTypeRepository(context);
            var service = new LeaveTypeService(repository);

            var result = await service.DeleteLeaveType(1);

            Assert.True(result);

            var deletedLeaveType =
                await context.LeaveTypes
                    .FirstOrDefaultAsync(l => l.Id == 1);

            Assert.Null(deletedLeaveType);
        }

        [Fact]
        public async Task DeleteLeaveType_WithInvalidId_ShouldReturnFalse()
        {
            using var context = CreateContext();

            var repository = new LeaveTypeRepository(context);
            var service = new LeaveTypeService(repository);

            var result = await service.DeleteLeaveType(999);

            Assert.False(result);
        }
    }
}
