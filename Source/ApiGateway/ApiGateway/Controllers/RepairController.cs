using Common;
using Common.Gprc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using RepairProto;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/repair")]
public class RepairController : ControllerBase, IDisposable
{
    /// <summary>
    /// Repair service channel used by repair grpc client
    /// </summary>
    private readonly GrpcChannel _repairServiceChannel;
    /// <summary>
    /// Repair gprc client used to make grpc requests.
    /// </summary>
    private readonly RepairGrpc.RepairGrpcClient _repairClient;
    public RepairController(IChannelFactory factory, IConfiguration configuration)
    {
        _repairServiceChannel = factory.GetChannel(configuration[Constants.RepairServiceAddress]!);
        _repairClient = new RepairGrpc.RepairGrpcClient(_repairServiceChannel);
    }

    ~RepairController()
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
        _repairServiceChannel.Dispose();
    }

    /// <summary>
    /// Route for starting repair process.
    /// </summary>
    /// <param name="data">Data necessary for starting the repair process.</param>
    [HttpPost("start")]
    public async Task<Empty> StartRepairProcess(StartRepairDto data)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _repairClient.StartRepairProcessAsync(data, metadata);
    }

    /// <summary>
    /// Route for ending repair process.
    /// </summary>
    /// <param name="data">Data necessary for ending the repair process.</param>
    [HttpPut("end")]
    public async Task<Empty> EndRepairProcess(EndRepairDto data)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _repairClient.EndRepairProcessAsync(data, metadata);
    }

    /// <summary>
    /// Route for fetching pole repair history.
    /// </summary>
    /// <returns>Pole repair history.</returns>
    [HttpGet("history/pole/{poleId}")]
    public async Task<ICollection<RepairDto>> GetByPole(string poleId)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        var result = await _repairClient.GetByPoleAsync(new ID { Id = poleId }, metadata);
        return result.Data;
    }

    /// <summary>
    /// Route for fetching team repair history.
    /// </summary>
    /// <returns>Team repair history.</returns>
    [HttpGet("history/team/{teamId}")]
    public async Task<ICollection<RepairDto>> GetByTeam(string teamId)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        var result = await _repairClient.GetByTeamAsync(new ID { Id = teamId }, metadata);
        return result.Data;
    }
}
