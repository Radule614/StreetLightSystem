using AutoMapper;
using Common;
using Common.Auth;
using Common.Gprc.Exceptions;
using Grpc.Core;
using Team.API.Domain.Entities;
using Team.API.Domain.Exceptions;
using Team.API.Domain.Services;
using Team.API.Domain.Specifications;
using Team.API.Infrastructure.Data;
using TeamProto;

namespace Team.API.Application;

/// <summary>
/// TeamController class used for specifying gRPC endpoints for team micro service
/// </summary>
public class TeamController : TeamGrpc.TeamGrpcBase
{
    private readonly Repository<TeamEntity> _teamRepository;
    private readonly Repository<Member> _memberRepository;
    private readonly IMapper _mapper;
    private readonly ITeamService _teamService;
    public TeamController(Repository<TeamEntity> teamRepository, Repository<Member> memberRepository, IMapper mapper, ITeamService teamService)
    {
        _teamRepository = teamRepository;
        _memberRepository = memberRepository;
        _mapper = mapper;
        _teamService = teamService;
    }
    /// <summary>
    /// Rpc endpoint for retrieving all team entities.
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <param name="context"></param>
    /// <returns>All team entities</returns>
    [Auth(permissions: "ReadTeams")]
    public override async Task<Teams> GetAll(Empty request, ServerCallContext context)
    {
        ICollection<TeamEntity> teamCollection = await _teamRepository.ListAsync(new TeamSpecification());
        var response = new Teams();
        response.Data.AddRange(_mapper.Map<ICollection<TeamDetailsDto>>(teamCollection));
        return response;
    }

    /// <summary>
    /// Rpc endpoint for retrieving a team entity by Id.
    /// </summary>
    /// <param name="request">Request that contains team entity's id.</param>
    /// <param name="context"></param>
    /// <returns>Team entity that matches the given id.</returns>
    [Auth(permissions: "ReadTeams")]
    public override async Task<TeamDetailsDto> GetById(ID request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var teamId))
        {
            throw new InvalidArgumentException(nameof(teamId), request.Id, Constants.GuidFormat);
        }

        var team = await _teamRepository.FirstOrDefaultAsync(new TeamSpecification(teamId));
        return team == null ? throw new TeamNotFoundException(teamId) : _mapper.Map<TeamDetailsDto>(team);
    }

    /// <summary>
    /// Rpc endpoint for creating team entity.
    /// </summary>
    /// <param name="request">Request that contains data necessary for creating team entity.</param>
    /// <param name="context"></param>
    /// <returns>Empty response indicating that the team entity has been deleted successfully.</returns>
    [Auth(permissions: "ModifyTeams")]
    public override async Task<Empty> Create(CreateDto request, ServerCallContext context)
    {
        await _teamService.Create(request.Name, request.MemberIds);
        return new Empty();
    }

    /// <summary>
    /// Rpc endpoint for updating existing team entity.
    /// </summary>
    /// <param name="request">Request that contains data necessary for updating team entity.</param>
    /// <param name="context"></param>
    /// <returns>Empty response indicating that the team entity has been updated successfully.</returns>
    [Auth(permissions: "ModifyTeams")]
    public override async Task<Empty> Update(UpdateDto request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var teamId))
        {
            throw new InvalidArgumentException(nameof(teamId), request.Id, Constants.GuidFormat);
        }
        await _teamService.Update(teamId, request.Name, request.MemberIds);
        return new Empty();
    }

    /// <summary>
    /// Rpc endpoint for deleting a team entity by Id.
    /// </summary>
    /// <param name="request">Request that contains team entity's id.</param>
    /// <param name="context"></param>
    /// <returns>Empty response indicating that the team entity has been deleted successfully.</returns>

    [Auth(permissions: "ModifyTeams")]
    public override async Task<Empty> Delete(ID request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var teamId))
        {
            throw new InvalidArgumentException(nameof(teamId), request.Id, Constants.GuidFormat);
        }
        var team = await _teamRepository.FirstOrDefaultAsync(new TeamSpecification(teamId));
        if (team == null)
        {
            throw new TeamNotFoundException(teamId);
        }
        await _teamRepository.DeleteAsync(team);
        return new Empty();
    }

    /// <summary>
    /// Rpc endpoint for fetching all possible members.
    /// </summary>
    /// <param name="request">Empty request.</param>
    /// <param name="context"></param>
    /// <returns>List of member entities.</returns>
    [Auth(permissions: "ReadTeams")]
    public override async Task<Members> GetPossibleMembers(Empty request, ServerCallContext context)
    {
        ICollection<Member> memberCollection = await _memberRepository.ListAsync(new MemberSpecification());
        var response = new Members();
        response.Data.AddRange(_mapper.Map<ICollection<MemberDetailsDto>>(memberCollection));
        return response;
    }

    /// <summary>
    /// Rpc endpoint for fetching logged in user's team.
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <param name="context"></param>
    /// <returns>User's team</returns>
    [Auth]
    public override async Task<TeamDto> GetUserTeam(Empty request, ServerCallContext context)
    {
        var userIdString = context.UserState["UserId"] as string;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), userIdString ?? "", Constants.GuidFormat);
        }
        var team = await _teamRepository.FirstOrDefaultAsync(new TeamMemberSpecification(userId));
        return _mapper.Map<TeamDto>(team);
    }

    public override async Task<Teams> GetByIdList(IdList request, ServerCallContext context)
    {
        var teamCollection = await _teamRepository.ListAsync(new TeamSpecification(request.Ids));
        var response = new Teams();
        response.Data.AddRange(_mapper.Map<ICollection<TeamDetailsDto>>(teamCollection));
        return response;
    }
}
