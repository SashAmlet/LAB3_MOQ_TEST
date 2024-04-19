using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using WebApplication1.Context;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Repository;

namespace UsersAPI.Moq
{
    public class UsersTests
    {
        private readonly Mock<IRepository> _userRepository;
        private UsersController _controller;
        public UsersTests()
        {
            // Arrange
            _userRepository = new Mock<IRepository>();
        }
        #region GetUsers Tests

        [Fact]
        public async Task GetAllAsync_WhenUsersExist_ReturnsAllUsers()
        {
            var users = GenerateUserList(10);
            // creating mock user repository based on the fake data
            _userRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);
            // creating user controller based on the mock user repository
            _controller = new UsersController(_userRepository.Object);
            // calling a controller method
            var result = await _controller.GetUsers();

            // checking if the result is of type ActionResult<IEnumerable<User>>
            Assert.IsType<ActionResult<IEnumerable<User>>>(result);

            // getting the status code from the ActionResult
            var statusCode = (result as ActionResult<IEnumerable<User>>)?.Result as ObjectResult;

            // comparing results
            Assert.Equal(200, statusCode?.StatusCode);
            Assert.Equal(users, statusCode?.Value);
            // verifying that GetAllAsync method was called once
            _userRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        
        [Fact]
        public async Task GetAllAsync_WhenNoUsersExist_ThrowsException()
        {
            _userRepository.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception());

            _controller = new UsersController(_userRepository.Object);

            var result = await _controller.GetUsers();

            Assert.IsType<ActionResult<IEnumerable<User>>>(result);


            var statusCode = (result as ActionResult<IEnumerable<User>>)?.Result as ObjectResult;


            Assert.Equal(400, statusCode?.StatusCode);
        }
        #endregion
        
        #region PostUser Tests

        [Fact]
        public async Task AddAsync_WhenUsersExist_AddsUserToContextAndSavesChanges()
        {
            var users = GenerateUserList(10);
            _userRepository.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.FromResult(users));

            _controller = new UsersController(_userRepository.Object);

            var result = await _controller.PostUser(GenerateUserList(1).First());
            Assert.IsAssignableFrom<ActionResult>(result);

            var statusCode = result as StatusCodeResult;

            Assert.Equal(200, statusCode?.StatusCode);

        }

        [Fact]
        public async Task AddAsync_WithNoValidUser_ThrowsException()
        {
            _userRepository.Setup(repo => repo.AddAsync(It.IsAny<User>())).ThrowsAsync(new Exception());

            _controller = new UsersController(_userRepository.Object);

            var result = await _controller.PostUser(null);
            Assert.IsAssignableFrom<ActionResult>(result);

            var statusCode = result as ObjectResult;

            Assert.Equal(400, statusCode?.StatusCode);
        }
        #endregion

        #region PutUser Tests

        [Fact]
        public async Task UpdateAsync_WhenUsersExist_UpdateUserAndSavesChanges()
        {
            var user = GenerateUserList(10).First();

            _userRepository.Setup(repo => repo.UpdateAsync(It.IsAny<int>(), It.IsAny<User>())).Returns((int id, User updatedUser) => Task.FromResult(user));

            _controller = new UsersController(_userRepository.Object);

            var result = await _controller.PutUser(user.Id, user);
            Assert.IsAssignableFrom<ActionResult>(result);

            var obj = result as StatusCodeResult;

            Assert.Equal(200, obj?.StatusCode);

        }

        [Fact]
        public async Task UpdateAsync_WithNonMatchingIdAndUser_ThrowsException()
        {
            var user = GenerateUserList(10).First();

            _userRepository.Setup(repo => repo.UpdateAsync(It.IsAny<int>(), It.IsAny<User>())).ThrowsAsync(new Exception());

            _controller = new UsersController(_userRepository.Object);

            var result = await _controller.PutUser(user.Id, user);
            Assert.IsAssignableFrom<ActionResult>(result);

            var obj = result as ObjectResult;

            Assert.Equal(400, obj?.StatusCode);
        }
        #endregion

        #region DeleteUser Tests

        [Fact]
        public async Task DeleteAsync_WhenUserExists_DeleteUserAndSavesChanges()
        {
            _userRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).Returns((int id) => Task.FromResult(true));

            _controller = new UsersController(_userRepository.Object);

            var result = await _controller.DeleteUser(It.IsAny<int>());
            var obj = result as ObjectResult;

            Assert.True(obj?.StatusCode == 200 && (bool)obj?.Value);

        }

        [Fact]
        public async Task DeleteAsync_WithNoUser_ThrowsException()
        {
            _userRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            _controller = new UsersController(_userRepository.Object);

            var result = await _controller.DeleteUser(It.IsAny<int>());
            var obj = result as ObjectResult;

            Assert.True(obj?.StatusCode == 400 || (bool)obj.Value);

        }
        #endregion
        
        #region HELP
        private List<User> GenerateUserList(int count)
        {
            // Initializing Faker to create fake data
            var faker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.IndexFaker + 1)
                .RuleFor(u => u.FName, f => f.Name.FirstName())
                .RuleFor(u => u.LName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email());

            // Creating a list of users
            List<User> users = faker.Generate(10);

            return users;
        }
        #endregion
    }


}