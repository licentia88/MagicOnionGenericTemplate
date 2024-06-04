using MagicOnion;
using MagicT.Server.Services;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Moq;

namespace MagicT.UnitTest.Services;

[TestFixture]
[TestOf(typeof(UserService))]
public class UsersServiceTest
{
    Mock<IUserService> Service;
    public UsersServiceTest()
    {
        Service = new Mock<IUserService>();
        Service.Setup(x => x.CreateAsync(new USERS { U_NAME = "Test" })).Returns(UnaryResult.FromResult(new USERS()));
    }

    [Test]
    public void METHOD()
    {
        //Service.Object.
    }
}