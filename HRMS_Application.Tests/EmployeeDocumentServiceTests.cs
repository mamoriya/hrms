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
    public class EmployeeDocumentServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddDocument_WithValidData_ShouldCreateDocument()
        {
            using var context = CreateContext();

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            await context.SaveChangesAsync();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var document = new EmployeeDocument
            {
                EmployeeProfileId = 1,
                FileName = "resume.pdf",
                FileType = "pdf",
                FilePath = "uploads/resume.pdf",
                FileSize = 1024
            };

            var result = await service.AddDocument(document);

            Assert.NotNull(result);
            Assert.Equal("resume.pdf", result.FileName);
            Assert.Equal(1, result.EmployeeProfileId);
        }

        [Fact]
        public async Task AddDocument_WithInvalidEmployee_ShouldNotCreateDocument()
        {
            using var context = CreateContext();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var document = new EmployeeDocument
            {
                EmployeeProfileId = 999,
                FileName = "resume.pdf",
                FileType = "pdf",
                FilePath = "uploads/resume.pdf",
                FileSize = 1024
            };

            var result = await service.AddDocument(document);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddDocument_WithEmptyFileName_ShouldNotCreateDocument()
        {
            using var context = CreateContext();

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            await context.SaveChangesAsync();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var document = new EmployeeDocument
            {
                EmployeeProfileId = 1,
                FileName = "",
                FileType = "pdf",
                FilePath = "uploads/resume.pdf",
                FileSize = 1024
            };

            var result = await service.AddDocument(document);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddDocument_WithZeroFileSize_ShouldNotCreateDocument()
        {
            using var context = CreateContext();

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            await context.SaveChangesAsync();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var document = new EmployeeDocument
            {
                EmployeeProfileId = 1,
                FileName = "resume.pdf",
                FileType = "pdf",
                FilePath = "uploads/resume.pdf",
                FileSize = 0
            };

            var result = await service.AddDocument(document);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetDocumentsByEmployeeId_WithValidEmployee_ShouldReturnDocuments()
        {
            using var context = CreateContext();

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            context.EmployeeDocuments.Add(new EmployeeDocument
            {
                Id = 1,
                EmployeeProfileId = 1,
                FileName = "resume.pdf",
                FileType = "pdf",
                FilePath = "uploads/resume.pdf",
                FileSize = 1024
            });

            await context.SaveChangesAsync();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var result = await service.GetDocumentsByEmployeeId(1);

            Assert.Single(result);
            Assert.Equal("resume.pdf", result[0].FileName);
        }

        [Fact]
        public async Task GetDocumentsByEmployeeId_WithInvalidEmployee_ShouldReturnEmptyList()
        {
            using var context = CreateContext();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var result = await service.GetDocumentsByEmployeeId(999);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDocumentById_WithValidData_ShouldReturnDocument()
        {
            using var context = CreateContext();

            context.EmployeeDocuments.Add(new EmployeeDocument
            {
                Id = 1,
                EmployeeProfileId = 1,
                FileName = "resume.pdf",
                FileType = "pdf",
                FilePath = "uploads/resume.pdf",
                FileSize = 1024
            });

            await context.SaveChangesAsync();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var result = await service.GetDocumentById(1, 1);

            Assert.NotNull(result);
            Assert.Equal("resume.pdf", result.FileName);
        }

        [Fact]
        public async Task DeleteDocument_WithValidData_ShouldDeleteDocument()
        {
            using var context = CreateContext();

            context.EmployeeDocuments.Add(new EmployeeDocument
            {
                Id = 1,
                EmployeeProfileId = 1,
                FileName = "resume.pdf",
                FileType = "pdf",
                FilePath = "uploads/resume.pdf",
                FileSize = 1024
            });

            await context.SaveChangesAsync();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var result = await service.DeleteDocument(1, 1);

            Assert.True(result);

            var document = await context.EmployeeDocuments
                .FirstOrDefaultAsync(d => d.Id == 1);

            Assert.Null(document);
        }

        [Fact]
        public async Task AddDocument_WithInvalidFileType_ShouldNotCreateDocument()
        {
            using var context = CreateContext();

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            await context.SaveChangesAsync();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var document = new EmployeeDocument
            {
                EmployeeProfileId = 1,
                FileName = "resume.exe",
                FileType = "exe",
                FilePath = "uploads/resume.exe",
                FileSize = 1024
            };

            var result = await service.AddDocument(document);

            Assert.Null(result);
        }


        [Fact]
        public async Task AddDocument_WithFileSizeGreaterThan5MB_ShouldNotCreateDocument()
        {
            using var context = CreateContext();

            context.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                JobTitle = "Developer",
                DepartmentId = 1,
                JoiningDate = DateTime.Now
            });

            await context.SaveChangesAsync();

            var repository = new EmployeeDocumentRepository(context);
            var employeeRepository = new EmployeeRepository(context);

            var service = new EmployeeDocumentService(
                repository,
                employeeRepository);

            var document = new EmployeeDocument
            {
                EmployeeProfileId = 1,
                FileName = "large.pdf",
                FileType = "pdf",
                FilePath = "uploads/large.pdf",
                FileSize = (5 * 1024 * 1024) + 1
            };

            var result = await service.AddDocument(document);

            Assert.Null(result);
        }
    }
}
