using System.ComponentModel.DataAnnotations;

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Context;

namespace UsersAPI.Moq.NUnitTests
{
    public class UsersControllerTests
    {
        // ???-?????? ?????????
        [Mock]
        private Mock<APIContext> mockContext;

        // ????????? ???-?????? ???????????
        [Spy]
        private UsersController controller;

        [SetUp]
        public void Setup()
        {
            // ????????????? ???-???????? ?? ?????????? ????
            mockContext = new Mock<APIContext>();
            controller = new UsersController(mockContext.Object);
        }

        [Test]
        public async Task GetUsers_ReturnsAllUsers()
        {
            // Arrange
            var users = GenerateUserList(10);
            mockContext.Setup(c => c.Users).Returns(users.AsQueryable());

            // Act
            var result = await controller.GetUsers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<User>>(actionResult.Value);
            Assert.Equal(users.Count, model.Count());
        }

        [Test]
        public async Task GetUser_ReturnsNotFound()
        {
            // Arrange
            var users = GenerateUserList(0);
            mockContext.Setup(c => c.Users).Returns(users.AsQueryable());

            // Act
            var result = await controller.GetUser(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PostUser_ThrowsException()
        {
            // Arrange
            mockContext.Setup(c => c.Users).Throws(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => controller.PostUser(new User()));
        }

        private List<User> GenerateUserList(int count)
        {
            var faker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.IndexFaker + 1)
                .RuleFor(u => u.FName, f => f.Name.FirstName())
                .RuleFor(u => u.LName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email());

            return faker.Generate(count);
        }
    }
}