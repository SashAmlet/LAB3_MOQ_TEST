using Moq;
using Xunit.Sdk;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private Mock<IExtendedRepository<User, CounterAPIContext>> _userRepository;
        private Fixture _fixture;
        private UsersController _controller;
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}