namespace Common.Saga.Repair.Dto;
public class RepairData
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? TeamId { get; set; }
    public Guid PoleId { get; set; }
    public bool IsSuccessful { get; set; }
}
