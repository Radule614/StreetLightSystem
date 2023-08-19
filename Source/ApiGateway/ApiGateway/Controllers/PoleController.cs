using Common;
using Common.Gprc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using PoleProto;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/pole")]
public class PoleController : ControllerBase, IDisposable
{
    /// <summary>
    /// Pole service channel used by pole grpc client
    /// </summary>
    private readonly GrpcChannel _poleServiceChannel;
    /// <summary>
    /// Pole gprc client used to make grpc requests.
    /// </summary>
    private readonly PoleGrpc.PoleGrpcClient _poleClient;
    public PoleController(IChannelFactory factory, IConfiguration configuration)
    {
        _poleServiceChannel = factory.GetChannel(configuration[Constants.PoleServiceAddress]!);
        _poleClient  = new PoleGrpc.PoleGrpcClient(_poleServiceChannel);
    }

    ~PoleController()
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
        _poleServiceChannel.Dispose();
    }

    /// <summary>
    /// Route for retrieving all pole entities
    /// </summary>
    /// <returns>Pole collection</returns>
    [HttpGet]
    public async Task<ICollection<PoleDTO>> GetAll()
    {
        Metadata? metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        Poles result = await _poleClient.GetAllAsync(new Empty(), metadata);
        return result.Data;
    }

    /// <summary>
    /// Route for retrieving a pole entity by the id
    /// </summary>
    /// <param name="id">poleId</param>
    /// <returns>Pole object</returns>
    [HttpGet("{id}")]
    public async Task<PoleDTO> GetById(string id)
    {
        Metadata? metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _poleClient.GetByIDAsync(new ID { Id = id }, metadata);
    }

    /// <summary>
    /// Route for creating pole entity
    /// </summary>
    /// <param name="data">Create data transfer object</param>
    /// <returns>Id of the created pole entity</returns>
    [HttpPost]
    public async Task<ID> Create(CreateDTO data)
    {
        Metadata? metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _poleClient.CreateAsync(data, metadata);
    }

    /// <summary>
    /// Route for updating pole entity
    /// </summary>
    /// <param name="data">Update data transfer object</param>
    /// <returns>Empty response</returns>
    [HttpPut]
    public async Task<Empty> Update(UpdateDTO data)
    {
        Metadata? metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _poleClient.UpdateAsync(data, metadata);
    }

    /// <summary>
    /// Route for deleting a pole entity by id
    /// </summary>
    /// <param name="id">Id of the pole entity that's meant to be deleted</param>
    /// <returns>Empty response</returns>
    [HttpDelete("{id}")]
    public async Task<Empty> Delete(string id)
    {
        Metadata? metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _poleClient.DeleteAsync(new ID { Id = id }, metadata);
    }
}