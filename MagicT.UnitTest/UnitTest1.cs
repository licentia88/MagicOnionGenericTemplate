using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.UnitTest;

public class RolesService : UnitTestBase<ROLES, IRolesService>
{

    [SetUp]
    public void Setup()
    {
        //this
    }

    //[test]
    public override UnaryResult<ROLES> CreateAsync(ROLES model)
    {
        return base.CreateAsync(model);
    }
}
public class Tests
{
    [SetUp]
    public void Setup()
    {
        //this
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}