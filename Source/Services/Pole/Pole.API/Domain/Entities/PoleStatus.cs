namespace Pole.API.Domain.Entities;

/// <summary>
/// Working: The pole is working correctly.
/// Broken: The pole is broken and waiting to be repaired by a crew.
/// BeingRepaired: Status indicating that a crew has arrived and started repairing the pole.
/// </summary>
public enum PoleStatus
{
    Working = 0, 
    Broken, 
    BeingRepaired
}