using Common.Test;
using NotificationProto;

namespace Pole.UnitTests.Tests;

public class PoleServiceTests
{
    [Fact]
    public async void CreatePole_ValidInputs_ShouldBeTrue()
    {
        //arrange
        var mockContext = new Mock<PoleContext>();
        Mock<PoleRepository> repository = new(mockContext.Object);
        var mockNotificationCall = Helpers.CreateAsyncUnaryCall(new Empty());
        var notificationClient = new Mock<NotificationGrpc.NotificationGrpcClient>();
        notificationClient
            .Setup(m => m.BroadcastNotificationAsync(
                It.IsAny<BroadcastNotificationDto>(), null, null, CancellationToken.None))
            .Returns(mockNotificationCall);
        IPoleService poleService = new PoleService(repository.Object, notificationClient.Object);

        //act
        var pole = await poleService.Create(1, 2);

        //assert
        Assert.NotNull(pole);
        Assert.NotEqual(Guid.Empty, pole.Id);
        Assert.Equal(1, pole.Latitude);
        Assert.Equal(2, pole.Longitude);
    }

    [Fact]
    public async void UpdatePoleStatus_ValidInputs_ShouldBeTrue()
    {
        //arrange
        var poleId = Guid.NewGuid();
        var mockContext = new Mock<PoleContext>();
        Mock<PoleRepository> repository = new(mockContext.Object);
        repository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Specification<PoleEntity>>(), CancellationToken.None))
            .Returns(Task.FromResult(new PoleEntity { Id = poleId })!);
        var mockNotificationCall = Helpers.CreateAsyncUnaryCall(new Empty());
        var notificationClient = new Mock<NotificationGrpc.NotificationGrpcClient>();
        notificationClient
            .Setup(m => m.BroadcastNotificationAsync(
                It.IsAny<BroadcastNotificationDto>(), null, null, CancellationToken.None))
            .Returns(mockNotificationCall);
        IPoleService poleService = new PoleService(repository.Object, notificationClient.Object);

        //act
        var pole = await poleService.UpdateStatus(poleId, PoleStatus.Broken);

        //assert
        Assert.NotNull(pole);
        Assert.Equal(PoleStatus.Broken, pole.Status);
    }

    [Fact]
    public async void UpdatePole_ValidInputs_ShouldBeTrue()
    {
        //arrange
        var poleId = Guid.NewGuid();
        var mockContext = new Mock<PoleContext>();
        Mock<PoleRepository> repository = new(mockContext.Object);
        repository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Specification<PoleEntity>>(), CancellationToken.None))
            .Returns(Task.FromResult(new PoleEntity { Id = poleId })!);
        var mockNotificationCall = Helpers.CreateAsyncUnaryCall(new Empty());
        var notificationClient = new Mock<NotificationGrpc.NotificationGrpcClient>();
        notificationClient
            .Setup(m => m.BroadcastNotificationAsync(
                It.IsAny<BroadcastNotificationDto>(), null, null, CancellationToken.None))
            .Returns(mockNotificationCall);
        IPoleService poleService = new PoleService(repository.Object, notificationClient.Object);

        //act
        var pole = await poleService.Update(poleId, PoleStatus.BeingRepaired, 2, 3);

        //assert
        Assert.NotNull(pole);
        Assert.Equal(poleId, pole.Id);
        Assert.Equal(PoleStatus.BeingRepaired, pole.Status);
        Assert.Equal(2, pole.Latitude);
        Assert.Equal(3, pole.Longitude);
    }
}
