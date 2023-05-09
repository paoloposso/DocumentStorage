using DocumentStorage.User;
using Moq;

namespace DocumentStorage.Test;

public class UserGroupServiceTest
{
    GroupService? _service;

    [SetUp]
    public void Setup()
    {
        var repository = new Mock<IGroupRepository>();

        repository
            .Setup(x => x.AddGroup(It.Is<string>(p => p.Equals("Test")), It.IsAny<string>()));

        repository
            .Setup(x => x.GetGroupById(It.Is<int>(p => p == 1)))
            .ReturnsAsync(() => new UserGroup { Id = 1, Name = "Test", Description = "Test Group" });

        _service = new GroupService(repository.Object);
    }

    [Test]
    public async Task ShouldNotErorInsertingGroup()
    {
        await _service!.AddGroup("ValidGroupName", "Group Description");

        Assert.Pass();
    }

    [Test]
    public async Task ShouldThrowArgumentExceptionWithInvalidData()
    {
        try
        {
            await _service!.AddGroup(string.Empty, "Group Description");
        }
        catch (ArgumentException) 
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    [Test]
    public async Task ShouldReturnGroupById()
    {
        var user = await _service!.GetById(1);

        Assert.IsNotNull(user);
        Assert.IsTrue(user?.Name.Equals("Test"));
    }

    [Test]
    public async Task ShouldReturnNullInvalidId()
    {
        try
        {
            _ = await _service!.GetById(99);
            Assert.Fail("Should Throw Argument Exception");
        }
        catch (ArgumentException)
        {
            Assert.Pass();
        }
    }
}