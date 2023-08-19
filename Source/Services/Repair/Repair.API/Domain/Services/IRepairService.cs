using Common.Saga.Repair.Dto;
using Repair.API.Domain.Entities;
using RepairProto;

namespace Repair.API.Domain.Services;

public interface IRepairService : IDisposable
{
    Task<RepairEntity> Create(Guid poleId);
    Task<RepairEntity> EndRepair(Guid repairId, bool isSuccessful);
    Task<ICollection<RepairDto>> GetPoleRepairHistory(Guid repairId);
    void BeginStartRepairTransaction(RepairData data, Guid userId);
    void BeginEndRepairTransaction(Guid repairId, Guid userId, bool success);
}
