using HRMS_Application.Data;
using HRMS_Application.Models;
using HRMS_Application.Repositories;
using HRMS_Application.Services;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Application.Tests;

public class EmployeeServiceTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateEmployee_WithValidData_ShouldCreateEmployee()
    {
        // Arrange
        using var context = CreateContext();

        var repository = new EmployeeRepository(context);
        var service = new EmployeeService(repository);

        context.Departments.Add(new Department
        {
            Id = 1,
            Name = "IT"
        });

        await context.SaveChangesAsync();

        var employee = new EmployeeProfile
        {
            User = new User
            {
                Email = "john@example.com",
                Password = "Test123",
                Role = "Employee"
            },

            FirstName = "John",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(employee);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }

    [Fact]
    public async Task CreateEmployee_WithEmptyFirstName_ShouldNotCreateEmployee()
    {
        // Arrange
        using var context = CreateContext();

        var repository = new EmployeeRepository(context);
        var service = new EmployeeService(repository);

        var employee = new EmployeeProfile
        {
            UserId = 2,
            FirstName = "",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(employee);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEmployee_WithEmptyLastName_ShouldNotCreateEmployee()
    {
        // Arrange
        using var context = CreateContext();

        var repository = new EmployeeRepository(context);
        var service = new EmployeeService(repository);

        var employee = new EmployeeProfile
        {
            UserId = 3,
            FirstName = "John",
            LastName = "",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(employee);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEmployee_WithNullEmployee_ShouldNotCreateEmployee()
    {
        // Arrange
        using var context = CreateContext();

        var repository = new EmployeeRepository(context);
        var service = new EmployeeService(repository);

        // Act
        var result = await service.CreateEmployee(null);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task CreateEmployee_WithWhitespaceFirstName_ShouldNotCreateEmployee()
    {
        // Arrange
        using var context = CreateContext();

        var repository = new EmployeeRepository(context);
        var service = new EmployeeService(repository);

        var employee = new EmployeeProfile
        {
            UserId = 4,
            FirstName = "   ",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(employee);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task CreateEmployee_WithWhitespaceLastName_ShouldNotCreateEmployee()
    {
        // Arrange
        using var context = CreateContext();

        var repository = new EmployeeRepository(context);
        var service = new EmployeeService(repository);

        var employee = new EmployeeProfile
        {
            UserId = 5,
            FirstName = "John",
            LastName = "   ",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(employee);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEmployee_WithDuplicateEmail_ShouldNotCreateEmployee()
    {
        // Arrange
        using var context = CreateContext();

        var repository = new EmployeeRepository(context);
        var service = new EmployeeService(repository);

        context.Departments.Add(new Department
        {
            Id = 1,
            Name = "IT"
        });

        await context.SaveChangesAsync();

        var employee1 = new EmployeeProfile
        {
            User = new User
            {
                Email = "same@example.com",
                Password = "Test123",
                Role = "Employee"
            },
            FirstName = "John",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        var employee2 = new EmployeeProfile
        {
            User = new User
            {
                Email = "same@example.com",
                Password = "Test123",
                Role = "Employee"
            },
            FirstName = "Alex",
            LastName = "Smith",
            JobTitle = "Designer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        await service.CreateEmployee(employee1);
        var result = await service.CreateEmployee(employee2);

        // Assert
        Assert.Null(result);
    }
    [Fact]
    public async Task CreateEmployee_WithInvalidDepartment_ShouldNotCreateEmployee()
    {
        // Arrange
        using var context = CreateContext();

        var repository = new EmployeeRepository(context);
        var service = new EmployeeService(repository);

        var employee = new EmployeeProfile
        {
            User = new User
            {
                Email = "invaliddepartment@example.com",
                Password = "Test123",
                Role = "Employee"
            },

            FirstName = "John",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 999,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(employee);

        // Assert
        Assert.Null(result);
    }
}