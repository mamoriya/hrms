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

        var request = new CreateEmployeeRequest
        {
            Email = "john@example.com",
            Password = "Test123",
            FirstName = "John",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(request);

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

        var request = new CreateEmployeeRequest
        {
            Email = "emptyfirstname@example.com",
            Password = "Test123",
            FirstName = "",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(request);

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

        var request = new CreateEmployeeRequest
        {
            Email = "emptylastname@example.com",
            Password = "Test123",
            FirstName = "John",
            LastName = "",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(request);

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

        var request = new CreateEmployeeRequest
        {
            Email = "whitespacefirstname@example.com",
            Password = "Test123",
            FirstName = "   ",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(request);

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

        var request = new CreateEmployeeRequest
        {
            Email = "whitespacelastname@example.com",
            Password = "Test123",
            FirstName = "John",
            LastName = "   ",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(request);

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

        var request1 = new CreateEmployeeRequest
        {
            Email = "same@example.com",
            Password = "Test123",
            FirstName = "John",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        var request2 = new CreateEmployeeRequest
        {
            Email = "same@example.com",
            Password = "Test123",
            FirstName = "Alex",
            LastName = "Smith",
            JobTitle = "Designer",
            DepartmentId = 1,
            JoiningDate = DateTime.Now
        };

        // Act
        await service.CreateEmployee(request1);

        var result = await service.CreateEmployee(request2);

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

        var request = new CreateEmployeeRequest
        {
            Email = "invaliddepartment@example.com",
            Password = "Test123",
            FirstName = "John",
            LastName = "Doe",
            JobTitle = "Developer",
            DepartmentId = 999,
            JoiningDate = DateTime.Now
        };

        // Act
        var result = await service.CreateEmployee(request);

        // Assert
        Assert.Null(result);
    }
}