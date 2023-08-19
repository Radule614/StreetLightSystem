using AutoMapper;
using Common;
using Common.Auth;
using Common.Gprc.Exceptions;
using Grpc.Core;
using Pole.API.Domain.Entities;
using Pole.API.Domain.Exceptions;
using Pole.API.Domain.Services;
using Pole.API.Domain.Specifications;
using Pole.API.Infrastructure.Data;
using PoleProto;
using Empty = PoleProto.Empty;

namespace Pole.API.Application;

/// <summary>
/// PoleController class used for specifying gRPC endpoints for pole micro service
/// </summary>
public class PoleController : PoleGrpc.PoleGrpcBase
{
    private readonly PoleRepository _poleRepository;
    private readonly IMapper _mapper;
    private readonly IPoleService _poleService;
    public PoleController(PoleRepository poleRepository, IMapper mapper, IPoleService poleService)
    {
        _poleRepository = poleRepository;
        _mapper = mapper;
        _poleService = poleService;
    }

    /// <summary>
    /// Rpc endpoint for retrieving all pole entities
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <param name="context"></param>
    /// <returns>All pole entities</returns>
    [Auth(permissions: "ReadPoles")]
    public override async Task<Poles> GetAll(Empty request, ServerCallContext context)
    {
        ICollection<PoleEntity> poleCollection = await _poleRepository.ListAsync(new PoleSpecification());

        var response = new Poles();
        response.Data.AddRange(_mapper.Map<ICollection<PoleDTO>>(poleCollection));
        return response;
    }

    /// <summary>
    /// Rpc endpoint for retrieving a pole entity by Id
    /// </summary>
    /// <param name="request">Request that contains pole entity's id</param>
    /// <param name="context"></param>
    /// <returns>Pole entity that matches the given id</returns>
    [Auth(permissions: "ReadPoles")]
    public override async Task<PoleDTO> GetByID(ID request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var poleId))
        {
            throw new InvalidArgumentException(nameof(poleId), request.Id, Constants.GuidFormat);
        }
        var pole = await _poleRepository.FirstOrDefaultAsync(new PoleSpecification(poleId));
        return pole == null ? throw new PoleNotFoundException(poleId) : _mapper.Map<PoleDTO>(pole);
    }

    /// <summary>
    /// Rpc endpoint for deleting a pole entity by Id
    /// </summary>
    /// <param name="request">Request that contains pole entity's id</param>
    /// <param name="context"></param>
    /// <returns>Empty response indicating that the pole entity has been deleted successfully</returns>
    [Auth(permissions: "ModifyPoles")]
    public override async Task<Empty> Delete(ID request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var poleId))
        {
            throw new InvalidArgumentException(nameof(poleId), request.Id, Constants.GuidFormat);
        }
        var pole = await _poleRepository.FirstOrDefaultAsync(new PoleSpecification(poleId));
        if (pole == null)
        {
            throw new PoleNotFoundException(poleId);
        }
        await _poleRepository.DeleteAsync(pole);
        return new Empty();
    }

    /// <summary>
    /// Rpc endpoint for creating pole entity
    /// </summary>
    /// <param name="request">Request that contains data necessary for creating pole entity</param>
    /// <param name="context"></param>
    /// <returns>Id of the created entity</returns>
    [Auth(permissions: "ModifyPoles")]
    public override async Task<ID> Create(CreateDTO request, ServerCallContext context)
    {
        PoleEntity pole = await _poleService.Create(request.Latitude, request.Longitude);
        return new ID { Id = pole.Id.ToString() };
    }

    /// <summary>
    /// Rpc endpoint for updating existing pole entity
    /// </summary>
    /// <param name="request">Request that contains data necessary for updating pole entity</param>
    /// <param name="context"></param>
    /// <returns>Empty response indicating that the pole entity has been updated successfully</returns>
    [Auth(permissions: "ModifyPoles")]
    public override async Task<Empty> Update(UpdateDTO request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var poleId))
        {
            throw new InvalidArgumentException(nameof(poleId), request.Id, Constants.GuidFormat);
        }
        await _poleService.Update(poleId, (PoleStatus)request.Status, request.Latitude, request.Longitude);
        return new Empty();
    }

    /// <summary>
    /// Rpc endpoint for updating pole statuses. This endpoint will primarily be used by MQTT Client and Repair service
    /// </summary>
    /// <param name="request">Contains pole id and the new status</param>
    /// <param name="context"></param>
    /// <returns>Empty response indicating that the status has been updated successfully</returns>
    public override async Task<Empty> UpdateStatus(UpdateStatusDTO request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var poleId))
        {
            throw new InvalidArgumentException(nameof(poleId), request.Id, Constants.GuidFormat);
        }
        await _poleService.UpdateStatus(poleId, (PoleStatus)request.Status);
        return new Empty();
    }

    /// <summary>
    /// Endpoint for fetching all poles from the simulator. This method needs to exist in order to avoid authentication.
    /// Regular GetAll method requires a valid token.
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<Poles> UnsecuredGetAll(Empty request, ServerCallContext context)
    {
        return await GetAll(request, context);
    }
}