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
    public class DepartmentServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateDepartment_WithValidData_ShouldCreateDepartment()
        {
            // Arrange
            using var context = CreateContext();

            var repository = new DepartmentRepository(context);
            var service = new DepartmentService(repository);

            var department = new Department
            {
                Name = "IT"
            };

            // Act
            var result = await service.CreateDepartment(department);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("IT", result.Name);
        }

        [Fact]
        public async Task CreateDepartment_WithEmptyName_ShouldNotCreateDepartment()
        {
            // Arrange
            using var context = CreateContext();

            var repository = new DepartmentRepository(context);
            var service = new DepartmentService(repository);

            var department = new Department
            {
                Name = ""
            };

            // Act
            var result = await service.CreateDepartment(department);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateDepartment_WithDuplicateName_ShouldNotCreateDepartment()
        {
            // Arrange
            using var context = CreateContext();

            context.Departments.Add(new Department
            {
                Name = "IT"
            });

            await context.SaveChangesAsync();

            var repository = new DepartmentRepository(context);
            var service = new DepartmentService(repository);

            var department = new Department
            {
                Name = "IT"
            };

            // Act
            var result = await service.CreateDepartment(department);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateDepartment_WithNullDepartment_ShouldNotCreateDepartment()
        {
            // Arrange
            using var context = CreateContext();

            var repository = new DepartmentRepository(context);
            var service = new DepartmentService(repository);

            // Act
            var result = await service.CreateDepartment(null);

            // Assert
            Assert.Null(result);
        }
    }
}
