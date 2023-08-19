using System.ComponentModel.DataAnnotations.Schema;

namespace Repair.API.Domain.Entities;

/// <summary>
/// Repair entity used to model repair data in the database through Entity framework.
/// </summary>
[Table("Repair")]
public class RepairEntity
{
    /// <summary>
    /// Repair id used as primary key in a database
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Date when repair process has started.
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// Date when repair process has ended.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Id of the repairing team.
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// Id of the pole that's being repaired.
    /// </summary>
    public Guid PoleId { get; set; }

    /// <summary>
    /// Flag value determining whether the repair process was successful
    /// </summary>
    public bool IsSuccessful { get; set; }
}