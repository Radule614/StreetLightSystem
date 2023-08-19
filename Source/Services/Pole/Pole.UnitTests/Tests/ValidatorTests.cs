using Common.Gprc;

namespace Pole.UnitTests.Tests;

public class ValidatorTests
{
    [Fact]
    public void ValidateData_InvalidPoleStatus_ShouldBeTrue()
    {
        //arrange
        PoleEntity entity = new()
        {
            Status = (PoleStatus)5
        };
        var exceptionBuilder = new ValidationExceptionBuilder();

        //act
        entity.ValidateData(exceptionBuilder);

        //assert
        Assert.True(exceptionBuilder.HasErrors());
    }

    [Fact]
    public void ValidateData_ValidPoleEntity_ShouldBeFalse()
    {
        //arrange
        PoleEntity entity = new()
        {
            Latitude = 1,
            Longitude = 2,
            Status = 0
        };
        var exceptionBuilder = new ValidationExceptionBuilder();

        //act
        entity.ValidateData(exceptionBuilder);

        //assert
        Assert.False(exceptionBuilder.HasErrors());
    }
}
