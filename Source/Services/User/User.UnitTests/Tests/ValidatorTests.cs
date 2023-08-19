using Common.Gprc;

namespace User.UnitTests.Tests;

public class ValidatorTests
{
    [Fact]
    public void ValidateDate_InvalidFirstName_ShouldBeTrue()
    {
        //arrange
        UserEntity entity = new()
        {
            FirstName = "Na"
        };
        var exceptionBuilder = new ValidationExceptionBuilder();

        //act
        entity.ValidateData(exceptionBuilder);

        //assert
        Assert.True(exceptionBuilder.HasErrors());
    }

    [Fact]
    public void ValidateData_InvalidLastName_ShouldBeTrue()
    {
        //arrange
        UserEntity entity = new()
        {
            LastName = "Na"
        };
        var exceptionBuilder = new ValidationExceptionBuilder();

        //act
        entity.ValidateData(exceptionBuilder);

        //assert
        Assert.True(exceptionBuilder.HasErrors());
    }

    [Fact]
    public void ValidateDate_InvalidEmail_ShouldBeTrue()
    {
        //arrange
        UserEntity entity = new()
        {
            Email = "@@email.com"
        };
        var exceptionBuilder = new ValidationExceptionBuilder();

        //act
        entity.ValidateData(exceptionBuilder);

        //assert
        Assert.True(exceptionBuilder.HasErrors());
    }

    [Fact]
    public void ValidateData_ValidUserEntity_ShouldBeFalse()
    {
        //arrange
        UserEntity entity = new()
        {
            FirstName = "Name",
            LastName = "Surname",
            Email = "email@email.com"
        };
        var exceptionBuilder = new ValidationExceptionBuilder();

        //act
        entity.ValidateData(exceptionBuilder);

        //assert
        Assert.False(exceptionBuilder.HasErrors());
    }
}
