using DocumentStorage.Core;
using DocumentStorage.User;
using Moq;

namespace DocumentStorage.Test;

public class Tests
{
    UserService? _service;

    [SetUp]
    public void Setup()
    {
        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.InsertUser(It.IsAny<User.User>()))
            .ReturnsAsync(1);

        _service = new UserService(new Mock<IUserRepository>().Object);
    }

    [Test]
    public async Task ShouldInsertAndReturnNewUserId()
    {
        var id = await _service!.AddUser(new User.User() { 
            Name = "Test", 
            Password = "12345",
            Role = Role.Admin, Active = true 
        });

        Assert.IsTrue(id > 0);
    }

    [Test]
    public async Task ShouldThrowArgumentException()
    {
        try
        {
            await _service!.AddUser(new User.User() { 
                Name = "Test", 
                Role = Role.Admin, 
                Active = true 
            });
        }
        catch (ArgumentException) 
        {
            Assert.Pass();
        }

        Assert.Fail();
    }
}