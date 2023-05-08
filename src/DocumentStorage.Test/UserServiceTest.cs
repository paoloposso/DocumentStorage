using DocumentStorage.Core;
using DocumentStorage.User;
using Moq;

namespace DocumentStorage.Test;

public class UserServiceTest
{
    UserService? _service;

    [SetUp]
    public void Setup()
    {
        var repository = new Mock<IUserRepository>();

        repository
            .Setup(x => x.InsertUser(It.IsAny<User.User>()))
            .ReturnsAsync(1);

        repository
            .Setup(x => x.GetUserById(It.Is<int>(p => p == 1)))
            .ReturnsAsync(() => new User.User() { 
                Id = 1,
                Name = "Test", 
                Password = "12345",
                Role = Role.Admin, 
                Active = true 
            });

        _service = new UserService(repository.Object);
    }

    [Test]
    public async Task ShouldInsertAndReturnNewUserId()
    {
        var id = await _service!.AddUser(new User.User() { 
            Name = "Test", 
            Password = "12345",
            Email = "test@a123.com",
            Role = Role.Admin, Active = true 
        });

        Assert.IsTrue(id > 0);
    }

    [Test]
    public async Task ShouldThrowArgumentExceptionWithInvalidEmail()
    {
        try
        {
            await _service!.AddUser(new User.User() { 
                Name = "Test", 
                Email = string.Empty,
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

    [Test]
    public async Task ShouldReturnUserById()
    {
        var user = await _service!.GetUser(1);

        Assert.IsNotNull(user);
        Assert.IsTrue(user?.Name.Equals("Test"));
    }

    [Test]
    public async Task ShouldReturnNullInvalidId()
    {
        try
        {
            _ = await _service!.GetUser(99);
            Assert.Fail("Should Throw Exception");
        }
        catch (ArgumentException)
        {
            Assert.Pass();
        }
    }
}