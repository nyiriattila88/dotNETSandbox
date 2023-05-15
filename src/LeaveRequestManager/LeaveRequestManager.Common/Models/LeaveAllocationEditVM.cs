namespace LeaveRequestManager.Common.Models;

public record LeaveAllocationEditVM : LeaveAllocationVM
{
    public string EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public EmployeeListVM Employee { get; set; }
}
