namespace LeaveRequestManager.Common.Models;

public record EmployeeAllocationVM : EmployeeListVM
{
    public List<LeaveAllocationVM> LeaveAllocations { get; set; }
}
