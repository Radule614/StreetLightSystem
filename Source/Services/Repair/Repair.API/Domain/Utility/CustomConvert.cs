using System.Globalization;
using System.Linq;
using Repair.API.Domain.Entities;
using RepairProto;
using TeamProto;

namespace Repair.API.Domain.Utility;

public class CustomConvert
{
    public static ICollection<RepairDto> HistoryToDto(ICollection<RepairEntity> history, ICollection<TeamDetailsDto>? teams = null)
    {
        List<RepairDto> list = new();
        foreach (var repair in history)
        {
            RepairDto dto = new()
            {
                Id = repair.Id.ToString(),
                PoleId = repair.PoleId.ToString(),
                TeamId = repair.TeamId.ToString(),
                IsSuccessful = repair.IsSuccessful,
                IsFinished = repair.EndDate != null,
                StartDate = repair.StartDate.ToString(CultureInfo.InvariantCulture)
            };
            if (repair.EndDate != null)
            {
                dto.EndDate = repair.EndDate.ToString();
            }
            var team = teams?.FirstOrDefault(t => t.Id.Equals(repair.TeamId.ToString()));
            if (team != null)
            {
                dto.Team = team;
            }
            list.Add(dto);
        }
        return list;
    }
}
