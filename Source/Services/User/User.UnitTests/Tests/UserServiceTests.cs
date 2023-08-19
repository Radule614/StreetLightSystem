using Common;
using Common.Gprc.Exceptions;
using User.API.Domain.Exceptions;
using User.API.Domain.Specifications;

namespace User.UnitTests.Tests;

public class UserServiceTests
{
    [Fact]
    public async void CreateUser_ValidInputs_ShouldBeTrue()
    {
        //arrange
        var mockContext = new Mock<UserContext>();
        Mock<Repository<UserEntity>> userRepository = new(mockContext.Object);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<EmailSpecification>(), CancellationToken.None))
            .ReturnsAsync((UserEntity?)null);
        Mock<Repository<Role>> roleRepository = new(mockContext.Object);
        roleRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Specification<Role>>(), CancellationToken.None))
            .ReturnsAsync(new Role { Name = "User" });
        IUserService userService = new UserService(userRepository.Object, roleRepository.Object);

        //act
        var user = await userService.Create("Name", "Surname", "email@email.com", "12345678abc");

        //assert
        Assert.NotNull(user);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Name", user.FirstName);
        Assert.Equal("Surname", user.LastName);
        Assert.Equal("email@email.com", user.Email);
        Assert.Equal(PasswordValidator.HashPassword("12345678abc"), user.Password);
    }

    [Fact]
    public async void CreateUser_InvalidPasswordLength_ShouldThrowException()
    {
        //arrange
        var mockContext = new Mock<UserContext>();
        Mock<Repository<UserEntity>> userRepository = new(mockContext.Object);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<EmailSpecification>(), CancellationToken.None))
            .ReturnsAsync((UserEntity?)null);
        Mock<Repository<Role>> roleRepository = new(mockContext.Object);
        roleRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Specification<Role>>(), CancellationToken.None))
            .ReturnsAsync(new Role { Name = "User" });
        IUserService userService = new UserService(userRepository.Object, roleRepository.Object);

        //act
        var exception = Record.ExceptionAsync(() =>
            userService.Create("Name", "Surname", "email@email.com", "1234567"));

        //assert
        Assert.IsType<ValidationException>(await exception);
    }

    [Fact]
    public async void CreateUser_InvalidPassword_ShouldThrowException()
    {
        //arrange
        var mockContext = new Mock<UserContext>();
        Mock<Repository<UserEntity>> userRepository = new(mockContext.Object);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<EmailSpecification>(), CancellationToken.None))
            .ReturnsAsync((UserEntity?)null);
        Mock<Repository<Role>> roleRepository = new(mockContext.Object);
        roleRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Specification<Role>>(), CancellationToken.None))
            .ReturnsAsync(new Role { Name = "User" });
        IUserService userService = new UserService(userRepository.Object, roleRepository.Object);

        //act
        var exception = Record.ExceptionAsync(() =>
            userService.Create("Name", "Surname", "email@email.com", "12345678"));

        //assert
        Assert.IsType<ValidationException>(await exception);
    }

    [Fact]
    public async void CreateUser_EmailExists_ShouldThrowException()
    {
        //arrange
        var mockContext = new Mock<UserContext>();
        Mock<Repository<UserEntity>> userRepository = new(mockContext.Object);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<EmailSpecification>(), CancellationToken.None))
            .ReturnsAsync(new UserEntity());
        Mock<Repository<Role>> roleRepository = new(mockContext.Object);
        roleRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Specification<Role>>(), CancellationToken.None))
            .ReturnsAsync(new Role { Name = "User" });
        IUserService userService = new UserService(userRepository.Object, roleRepository.Object);

        //act
        var exception = Record.ExceptionAsync(() =>
            userService.Create("Name", "Surname", "email@email.com", "12345678abc"));

        //assert
        Assert.IsType<ValidationException>(await exception);
    }

    [Fact]
    public async void UpdateUser_ValidInputs_ShouldBeTrue()
    {
        //arrange
        var mockContext = new Mock<UserContext>();
        Mock<Repository<UserEntity>> userRepository = new(mockContext.Object);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<EmailSpecification>(), CancellationToken.None))
            .ReturnsAsync((UserEntity?)null);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<UserSpecification>(), CancellationToken.None))
            .ReturnsAsync(new UserEntity());
        IUserService userService = new UserService(userRepository.Object, null!);

        //act
        var user = await userService.Update(Guid.NewGuid(), "Name", "Surname", "email@email.com", "12345678abc");

        //assert
        Assert.NotNull(user);
        Assert.Equal("Name", user.FirstName);
        Assert.Equal("Surname", user.LastName);
        Assert.Equal("email@email.com", user.Email);
        Assert.Equal(PasswordValidator.HashPassword("12345678abc"), user.Password);
    }

    [Fact]
    public async void UpdateUser_InvalidPasswordLength_ShouldThrowException()
    {
        //arrange
        var mockContext = new Mock<UserContext>();
        Mock<Repository<UserEntity>> userRepository = new(mockContext.Object);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<EmailSpecification>(), CancellationToken.None))
            .ReturnsAsync((UserEntity?)null);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<UserSpecification>(), CancellationToken.None))
            .ReturnsAsync(new UserEntity());
        IUserService userService = new UserService(userRepository.Object, null!);

        //act
        var exception = Record.ExceptionAsync(() =>
            userService.Update(Guid.NewGuid(), "Name", "Surname", "email@email.com", "1234567"));

        //assert
        Assert.IsType<ValidationException>(await exception);
    }

    [Fact]
    public async void UpdateUser_InvalidPassword_ShouldThrowException()
    {
        //arrange
        var mockContext = new Mock<UserContext>();
        Mock<Repository<UserEntity>> userRepository = new(mockContext.Object);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<EmailSpecification>(), CancellationToken.None))
            .ReturnsAsync((UserEntity?)null);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<UserSpecification>(), CancellationToken.None))
            .ReturnsAsync(new UserEntity());
        IUserService userService = new UserService(userRepository.Object, null!);

        //act
        var exception = Record.ExceptionAsync(() =>
            userService.Update(Guid.NewGuid(), "Name", "Surname", "email@email.com", "12345678"));

        //assert
        Assert.IsType<ValidationException>(await exception);
    }

    [Fact]
    public async void UpdateUser_EmailExists_ShouldThrowException()
    {
        //arrange
        var mockContext = new Mock<UserContext>();
        Mock<Repository<UserEntity>> userRepository = new(mockContext.Object);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<EmailSpecification>(), CancellationToken.None))
            .ReturnsAsync(new UserEntity());
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<UserSpecification>(), CancellationToken.None))
            .ReturnsAsync(new UserEntity());
        IUserService userService = new UserService(userRepository.Object, null!);

        //act
        var exception = Record.ExceptionAsync(() =>
            userService.Update(Guid.NewGuid(), "Name", "Surname", "email@email.com", "12345678abc"));

        //assert
        Assert.IsType<ValidationException>(await exception);
    }

    [Fact]
    public async void UpdateUser_UserNotFound_ShouldThrowException()
    {
        //arrange
        var mockContext = new Mock<UserContext>();
        Mock<Repository<UserEntity>> userRepository = new(mockContext.Object);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<EmailSpecification>(), CancellationToken.None))
            .ReturnsAsync((UserEntity?)null);
        userRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<UserSpecification>(), CancellationToken.None))
            .ReturnsAsync((UserEntity?)null);
        IUserService userService = new UserService(userRepository.Object, null!);

        //act
        var exception = Record.ExceptionAsync(() =>
            userService.Update(Guid.NewGuid(), "Name", "Surname", "email@email.com", "12345678abc"));

        //assert
        Assert.IsType<UserNotFoundException>(await exception);
    }
}