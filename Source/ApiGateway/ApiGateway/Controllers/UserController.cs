using Common;
using Common.Gprc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using UserProto;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase, IDisposable
{
    /// <summary>
    /// User service channel used by user grpc client
    /// </summary>
    private readonly GrpcChannel _userServiceChannel;
    /// <summary>
    /// User gprc client used to make grpc requests.
    /// </summary>
    private readonly UserGrpc.UserGrpcClient _userClient;
    public UserController(IChannelFactory factory, IConfiguration configuration)
    {
        _userServiceChannel = factory.GetChannel(configuration[Constants.UserServiceAddress]!);
        _userClient = new UserGrpc.UserGrpcClient(_userServiceChannel);
    }

    ~UserController()
    {
        Dispose(false);
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _userServiceChannel.Dispose();
    }
    

    /// <summary>
    /// Route for retrieving all user entities
    /// </summary>
    /// <returns>User collection</returns>
    [HttpGet]
    public async Task<ICollection<UserDto>> Get()
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        var result = await _userClient.GetAllAsync(new Empty(), metadata);
        return result.Data;
    }

    /// <summary>
    /// Route for retrieving an user entity by the id
    /// </summary>
    /// <param name="id">userId</param>
    /// <returns>User object</returns>
    [HttpGet("{id}")]
    public async Task<UserDto> GetById(string id)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _userClient.GetByIdAsync(new ID { Id = id }, metadata);
    }

    /// <summary>
    /// Route for creating user entity
    /// </summary>
    /// <param name="data">Create data transfer object</param>
    /// <returns>Empty response</returns>
    [HttpPost]
    public async Task<Empty> Create(CreateDto data)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _userClient.CreateAsync(data, metadata);
    }

    /// <summary>
    /// Route for updating user entity
    /// </summary>
    /// <param name="data">Update data transfer object</param>
    /// <returns>Empty response</returns>
    [HttpPut]
    public async Task<Empty> Update(UpdateDto data)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _userClient.UpdateAsync(data, metadata);
    }

    /// <summary>
    /// Route for deleting an user entity by id
    /// </summary>
    /// <param name="id">Id of the user entity that's meant to be deleted</param>
    /// <returns>Empty response</returns>
    [HttpDelete("{id}")]
    public async Task<Empty> Delete(string id)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _userClient.DeleteAsync(new ID { Id = id }, metadata);
    }

    /// <summary>
    /// Route for retrieving logged in user entity.
    /// </summary>
    /// <returns>User object</returns>
    [HttpGet("me")]
    public async Task<UserDto> GetUserData()
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _userClient.GetUserDataAsync(new Empty(), metadata);
    }
}
