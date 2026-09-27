using HRMS_Application.Data;
using HRMS_Application.DTOs;
using HRMS_Application.Models;
using HRMS_Application.Repositories;
using HRMS_Application.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRMS_Application.Tests
{
    public class AuthTests
    {
        private AuthService GetService(out ApplicationDbContext context)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new ApplicationDbContext(options);

            var repository = new AuthRepository(context);

            return new AuthService(repository);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsUser()
        {
            var service = GetService(out var context);

            context.Users.Add(new User
            {
                Id = 1,
                Email = "ayush@gmail.com",
                Password = "12345",
                Role = "Employee"
            });

            await context.SaveChangesAsync();

            var request = new LoginRequest
            {
                Email = "ayush@gmail.com",
                Password = "12345"
            };

            var result = await service.Login(request);

            Assert.NotNull(result);
            Assert.Equal("ayush@gmail.com", result.Email);
            Assert.Equal("Employee", result.Role);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsNull()
        {
            var service = GetService(out var context);

            context.Users.Add(new User
            {
                Id = 1,
                Email = "ayush@gmail.com",
                Password = "12345",
                Role = "Employee"
            });

            await context.SaveChangesAsync();

            var request = new LoginRequest
            {
                Email = "ayush@gmail.com",
                Password = "wrong"
            };

            var result = await service.Login(request);

            Assert.Null(result);
        }
        [Fact]
        public async Task Login_UserNotFound_ReturnsNull()
        {
            var service = GetService(out var context);

            var request = new LoginRequest
            {
                Email = "unknown@gmail.com",
                Password = "12345"
            };

            var result = await service.Login(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_EmptyEmail_ReturnsNull()
        {
            var service = GetService(out var context);

            var request = new LoginRequest
            {
                Email = "",
                Password = "12345"
            };

            var result = await service.Login(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_EmptyPassword_ReturnsNull()
        {
            var service = GetService(out var context);

            var request = new LoginRequest
            {
                Email = "ayush@gmail.com",
                Password = ""
            };

            var result = await service.Login(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task SetupPassword_ValidRequest_UpdatesPassword()
        {
            var service = GetService(out var context);

            context.Users.Add(new User
            {
                Id = 1,
                Email = "ayush@gmail.com",
                Password = "old123",
                Role = "Employee"
            });

            await context.SaveChangesAsync();

            var request = new SetupPasswordRequest
            {
                Email = "ayush@gmail.com",
                Password = "new123"
            };

            var result = await service.SetupPassword(request);

            Assert.NotNull(result);
            Assert.Equal("new123", result.Password);

            var user = await context.Users.FirstAsync();

            Assert.Equal("new123", user.Password);
        }
    }
}
