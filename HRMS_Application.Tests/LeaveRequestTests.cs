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
    public class LeaveRequestTests
    {
        private LeaveRequestService GetService(out ApplicationDbContext context)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new ApplicationDbContext(options);

            var repository = new LeaveRequestRepository(context);

            return new LeaveRequestService(repository);
        }

        [Fact]
        public async Task CreateLeaveRequest_ValidRequest_ReturnsLeaveRequest()
        {
            var service = GetService(out var context);

            // Employee
            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "Ayush",
                LastName = "Mamoriya",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            // Leave Type
            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = false
            });

            // Leave Balance
            context.LeaveBalances.Add(new LeaveBalance
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                TotalAllotted = 12,
                Used = 0,
                Reserved = 0
            });

            await context.SaveChangesAsync();

            // Leave Request
            var request = new LeaveRequest
            {
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29),
                Reason = "Personal work"
            };

            // Act
            var result = await service.CreateLeaveRequest(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("PENDING", result.Status);
            Assert.Equal(2, context.LeaveBalances
                .First().Reserved);
        }

        [Fact]
        public async Task CreateLeaveRequest_InvalidEmployee_ReturnsNull()
        {
            var service = GetService(out var context);

            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = false
            });

            await context.SaveChangesAsync();

            var request = new LeaveRequest
            {
                EmployeeProfileId = 999,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29)
            };

            var result = await service.CreateLeaveRequest(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLeaveRequest_InvalidLeaveType_ReturnsNull()
        {
            var service = GetService(out var context);

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "Ayush",
                LastName = "Mamoriya",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            await context.SaveChangesAsync();

            var request = new LeaveRequest
            {
                EmployeeProfileId = 1,
                LeaveTypeId = 999,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29)
            };

            var result = await service.CreateLeaveRequest(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLeaveRequest_EndDateBeforeStartDate_ReturnsNull()
        {
            var service = GetService(out var context);

            var request = new LeaveRequest
            {
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 29),
                EndDate = new DateTime(2026, 9, 28)
            };

            var result = await service.CreateLeaveRequest(request);

            Assert.Null(result);
        }


        [Fact]
        public async Task CreateLeaveRequest_NoLeaveBalance_ReturnsNull()
        {
            var service = GetService(out var context);

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "Ayush",
                LastName = "Mamoriya",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = false
            });

            await context.SaveChangesAsync();

            var request = new LeaveRequest
            {
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29)
            };

            var result = await service.CreateLeaveRequest(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLeaveRequest_InsufficientBalance_ReturnsNull()
        {
            var service = GetService(out var context);

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "Ayush",
                LastName = "Mamoriya",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 2,
                CarryForward = false
            });

            context.LeaveBalances.Add(new LeaveBalance
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                TotalAllotted = 2,
                Used = 0,
                Reserved = 0
            });

            await context.SaveChangesAsync();

            var request = new LeaveRequest
            {
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 30)
            };

            var result = await service.CreateLeaveRequest(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLeaveRequest_OverlappingLeave_ReturnsNull()
        {
            var service = GetService(out var context);

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "Ayush",
                LastName = "Mamoriya",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = false
            });

            context.LeaveBalances.Add(new LeaveBalance
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                TotalAllotted = 12,
                Used = 0,
                Reserved = 2
            });

            context.LeaveRequests.Add(new LeaveRequest
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29),
                Status = "PENDING",
                CreatedAt = DateTime.Now
            });

            await context.SaveChangesAsync();

            var request = new LeaveRequest
            {
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 29),
                EndDate = new DateTime(2026, 9, 30)
            };

            var result = await service.CreateLeaveRequest(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLeaveRequest_WeekendExcluded_ReservesCorrectDays()
        {
            var service = GetService(out var context);

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "Ayush",
                LastName = "Mamoriya",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = false
            });

            context.LeaveBalances.Add(new LeaveBalance
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                TotalAllotted = 12,
                Used = 0,
                Reserved = 0
            });

            await context.SaveChangesAsync();

            var request = new LeaveRequest
            {
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 10, 2), // Friday
                EndDate = new DateTime(2026, 10, 5)   // Monday
            };

            var result = await service.CreateLeaveRequest(request);

            Assert.NotNull(result);
            Assert.Equal("PENDING", result.Status);

            var balance = await context.LeaveBalances.FirstAsync();

            Assert.Equal(2, balance.Reserved);
        }


        [Fact]
        public async Task ApproveLeaveRequest_ValidRequest_ApprovesLeave()
        {
            var service = GetService(out var context);

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "Ayush",
                LastName = "Mamoriya",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = false
            });

            context.LeaveBalances.Add(new LeaveBalance
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                TotalAllotted = 12,
                Used = 0,
                Reserved = 2
            });

            context.LeaveRequests.Add(new LeaveRequest
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29),
                Status = "PENDING",
                CreatedAt = DateTime.Now
            });

            await context.SaveChangesAsync();

            // Act
            var result = await service.ApproveLeaveRequest(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("APPROVED", result.Status);

            var balance = await context.LeaveBalances.FirstAsync();

            Assert.Equal(2, balance.Used);
            Assert.Equal(0, balance.Reserved);
        }
        [Fact]
        public async Task ApproveLeaveRequest_InvalidId_ReturnsNull()
        {
            var service = GetService(out var context);

            var result = await service.ApproveLeaveRequest(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task ApproveLeaveRequest_AlreadyApproved_ReturnsNull()
        {
            var service = GetService(out var context);

            context.LeaveRequests.Add(new LeaveRequest
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29),
                Status = "APPROVED",
                CreatedAt = DateTime.Now
            });

            await context.SaveChangesAsync();

            var result = await service.ApproveLeaveRequest(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task ApproveLeaveRequest_RejectedRequest_ReturnsNull()
        {
            var service = GetService(out var context);

            context.LeaveRequests.Add(new LeaveRequest
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29),
                Status = "REJECTED",
                RejectionReason = "Leave not approved",
                CreatedAt = DateTime.Now
            });

            await context.SaveChangesAsync();

            var result = await service.ApproveLeaveRequest(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task RejectLeaveRequest_ValidRequest_RejectsLeave()
        {
            var service = GetService(out var context);

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "Ayush",
                LastName = "Mamoriya",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            context.LeaveTypes.Add(new LeaveType
            {
                Id = 1,
                Name = "Casual Leave",
                AnnualQuota = 12,
                CarryForward = false
            });

            context.LeaveBalances.Add(new LeaveBalance
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                TotalAllotted = 12,
                Used = 0,
                Reserved = 2
            });

            context.LeaveRequests.Add(new LeaveRequest
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29),
                Status = "PENDING",
                CreatedAt = DateTime.Now
            });

            await context.SaveChangesAsync();

            var result = await service.RejectLeaveRequest(
                1,
                "Leave is not approved due to project requirements");

            Assert.NotNull(result);
            Assert.Equal("REJECTED", result.Status);
            Assert.Equal(
                "Leave is not approved due to project requirements",
                result.RejectionReason);

            var balance = await context.LeaveBalances.FirstAsync();

            Assert.Equal(0, balance.Reserved);
            Assert.Equal(0, balance.Used);
        }

        [Fact]
        public async Task RejectLeaveRequest_InvalidId_ReturnsNull()
        {
            var service = GetService(out var context);

            var result = await service.RejectLeaveRequest(
                999,
                "Not approved");

            Assert.Null(result);
        }

        [Fact]
        public async Task RejectLeaveRequest_EmptyReason_ReturnsNull()
        {
            var service = GetService(out var context);

            context.LeaveRequests.Add(new LeaveRequest
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29),
                Status = "PENDING",
                CreatedAt = DateTime.Now
            });

            await context.SaveChangesAsync();

            var result = await service.RejectLeaveRequest(1, "");

            Assert.Null(result);
        }

        [Fact]
        public async Task RejectLeaveRequest_AlreadyApproved_ReturnsNull()
        {
            var service = GetService(out var context);

            context.LeaveRequests.Add(new LeaveRequest
            {
                Id = 1,
                EmployeeProfileId = 1,
                LeaveTypeId = 1,
                StartDate = new DateTime(2026, 9, 28),
                EndDate = new DateTime(2026, 9, 29),
                Status = "APPROVED",
                CreatedAt = DateTime.Now
            });

            await context.SaveChangesAsync();

            var result = await service.RejectLeaveRequest(
                1,
                "Not approved");

            Assert.Null(result);
        }
    }
}
