using Common;
using Common.Auth;
using Common.Gprc.Exceptions;
using Common.Saga.Repair.Dto;
using Grpc.Core;
using Repair.API.Domain.Entities;
using Repair.API.Domain.Services;
using Repair.API.Domain.Specifications;
using Repair.API.Domain.Utility;
using Repair.API.Infrastructure.Data;
using RepairProto;

namespace Repair.API.Application;

public class RepairController : RepairGrpc.RepairGrpcBase
{
    private readonly RepairRepository _repairRepository;
    private readonly IRepairService _repairService;

    public RepairController(RepairRepository repairRepository, IRepairService repairService)
    {
        _repairRepository = repairRepository;
        _repairService = repairService;
    }

    [Auth(permissions: "PoleRepair")]
    public override Task<Empty> StartRepairProcess(StartRepairDto request, ServerCallContext context)
    {
        var userIdString = context.UserState["UserId"] as string;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), userIdString ?? "", Constants.GuidFormat);
        }
        if (!Guid.TryParse(request.PoleId, out var poleId))
        {
            throw new InvalidArgumentException(nameof(poleId), request.PoleId, Constants.GuidFormat);
        }
        RepairData repairData = new()
        {
            PoleId = poleId,
        };
        _repairService.BeginStartRepairTransaction(repairData, userId);
        return Task.FromResult(new Empty());
    }

    [Auth(permissions: "PoleRepair")]
    public override Task<Empty> EndRepairProcess(EndRepairDto request, ServerCallContext context)
    {
        var userIdString = context.UserState["UserId"] as string;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), userIdString ?? "", Constants.GuidFormat);
        }
        if (!Guid.TryParse(request.RepairId, out var repairId))
        {
            throw new InvalidArgumentException(nameof(request.RepairId), request.RepairId, Constants.GuidFormat);
        }
        _repairService.BeginEndRepairTransaction(repairId, userId, request.Success);
        return Task.FromResult(new Empty());
    }

    [Auth(permissions: "ReadRepairHistory")]
    public override async Task<History> GetByPole(ID request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var poleId))
        {
            throw new InvalidArgumentException(nameof(poleId), request.Id, Constants.GuidFormat);
        }
        var history = await _repairService.GetPoleRepairHistory(poleId);
        var response = new History();
        response.Data.AddRange(history);
        return response;
    }

    [Auth(permissions: "ReadRepairHistory")]
    public override async Task<History> GetByTeam(ID request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var teamId))
        {
            throw new InvalidArgumentException(nameof(teamId), request.Id, Constants.GuidFormat);
        }
        ICollection<RepairEntity> history = await _repairRepository.ListAsync(new TeamSpecification(teamId));
        var response = new History();
        response.Data.AddRange(CustomConvert.HistoryToDto(history));
        return response;
    }
}
