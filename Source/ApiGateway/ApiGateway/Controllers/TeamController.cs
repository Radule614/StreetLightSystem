using ApiGateway.Dto;
using Common;
using Common.Gprc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using TeamProto;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/team")]
public class TeamController : ControllerBase, IDisposable
{
    /// <summary>
    /// Team service channel used by team grpc client.
    /// </summary>
    private readonly GrpcChannel _teamServiceChannel;
    /// <summary>
    /// Team gprc client used to make grpc requests.
    /// </summary>
    private readonly TeamGrpc.TeamGrpcClient _teamClient;
    public TeamController(IChannelFactory factory, IConfiguration configuration)
    {
        _teamServiceChannel = factory.GetChannel(configuration[Constants.TeamServiceAddress]!);
        _teamClient = new TeamGrpc.TeamGrpcClient(_teamServiceChannel);
    }

    ~TeamController()
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
        _teamServiceChannel.Dispose();
    }

    /// <summary>
    /// Route for retrieving all team entities.
    /// </summary>
    /// <returns>Team collection.</returns>
    [HttpGet]
    public async Task<ICollection<TeamDetailsDto>> GetAll()
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        var result = await _teamClient.GetAllAsync(new Empty(), metadata);
        return result.Data;
    }

    /// <summary>
    /// Route for retrieving a team entity by the id.
    /// </summary>
    /// <param name="id">teamId</param>
    /// <returns>Team object.</returns>
    [HttpGet("{id}")]
    public async Task<TeamDetailsDto> GetById(string id)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _teamClient.GetByIdAsync(new ID { Id = id }, metadata);
    }

    /// <summary>
    /// Route for creating team entity.
    /// </summary>
    /// <param name="data">Create data transfer object.</param>
    /// <returns>Empty response</returns>
    [HttpPost]
    public async Task<Empty> Create(CreateTeamDto data)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        CreateDto dto = new()
        {
            Name = data.Name
        };
        dto.MemberIds.AddRange(data.MemberIds);
        return await _teamClient.CreateAsync(dto, metadata);
    }

    /// <summary>
    /// Route for updating team entity
    /// </summary>
    /// <param name="data">Update data transfer object.</param>
    /// <returns>Empty response</returns>
    [HttpPut]
    public async Task<Empty> Update(UpdateTeamDto data)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        UpdateDto dto = new()
        {
            Id = data.Id,
            Name = data.Name
        };
        dto.MemberIds.AddRange(data.MemberIds);
        return await _teamClient.UpdateAsync(dto, metadata);
    }

    /// <summary>
    /// Route for deleting an team entity by id.
    /// </summary>
    /// <param name="id">Id of the team entity that's meant to be deleted.</param>
    /// <returns>Empty response</returns>
    [HttpDelete("{id}")]
    public async Task<Empty> Delete(string id)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _teamClient.DeleteAsync(new ID { Id = id }, metadata);
    }

    /// <summary>
    /// Route for fetching all possible team members.
    /// </summary>
    /// <returns>Member list.</returns>
    [HttpGet("members")]
    public async Task<ICollection<MemberDetailsDto>> GetPossibleMembers()
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        var result = await _teamClient.GetPossibleMembersAsync(new Empty(), metadata);
        return result.Data;
    }

    /// <summary>
    /// Route for retrieving logged in user's team.
    /// </summary>
    /// <returns>User object</returns>
    [HttpGet("me")]
    public async Task<TeamDto> GetUserData()
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _teamClient.GetUserTeamAsync(new Empty(), metadata);
    }
}
