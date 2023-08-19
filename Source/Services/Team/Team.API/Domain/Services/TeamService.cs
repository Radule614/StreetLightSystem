using Common.Gprc;
using Team.API.Domain.Entities;
using Team.API.Domain.Exceptions;
using Team.API.Domain.Specifications;
using Team.API.Infrastructure.Data;

namespace Team.API.Domain.Services;

public class TeamService : ITeamService
{
    private readonly ILogger<TeamService> _logger;
    private readonly Repository<TeamEntity> _teamRepository;
    private readonly Repository<Member> _memberRepository;

    public TeamService(Repository<TeamEntity> teamRepository, Repository<Member> memberRepository, ILogger<TeamService> logger)
    {
        _teamRepository = teamRepository;
        _memberRepository = memberRepository;
        _logger = logger;
    }
    public async Task<TeamEntity> Create(string name, IEnumerable<string> memberIds)
    {
        ValidationExceptionBuilder exceptionBuilder = new();
        var id = Guid.NewGuid();
        var team = new TeamEntity
        {
            Id = id,
            Name = name,
        };
        team.ValidateData(exceptionBuilder);
        if (exceptionBuilder.HasErrors())
        {
            _logger.LogInformation($"Team service failed to create a team entity.");
            throw exceptionBuilder.Build();
        }
        var members = await _memberRepository.ListAsync(new MemberSpecification(memberIds));
        team.Members = members;
        await _teamRepository.AddAsync(team);
        _logger.LogInformation($"Team service created a team entity: {team.Name}");
        return team;
    }

    public async Task<TeamEntity> Update(Guid teamId, string name, IEnumerable<string> memberIds)
    {
        ValidationExceptionBuilder exceptionBuilder = new();
        var team = await _teamRepository.FirstOrDefaultAsync(new TeamSpecification(teamId));
        if (team == null)
        {
            _logger.LogInformation($"Team service failed to update a team entity.");
            throw new TeamNotFoundException(teamId);
        }
        var members = await _memberRepository.ListAsync(new MemberSpecification(memberIds));
        team.Name = name;
        team.Members = members;
        team.ValidateData(exceptionBuilder);
        if (exceptionBuilder.HasErrors())
        {
            _logger.LogInformation($"Team service failed to update a team entity.");
            throw exceptionBuilder.Build();
        }
        await _teamRepository.UpdateAsync(team);
        _logger.LogInformation($"Team service created a team entity: {team.Name}");
        return team;
    }
}
