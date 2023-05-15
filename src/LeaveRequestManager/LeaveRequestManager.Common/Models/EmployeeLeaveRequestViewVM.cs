namespace LeaveRequestManager.Common.Models;

public record EmployeeLeaveRequestViewVM
{
    public EmployeeLeaveRequestViewVM(List<LeaveAllocationVM> leaveAllocations, List<LeaveRequestVM> leaveRequests)
    {
        LeaveAllocations = leaveAllocations;
        LeaveRequests = leaveRequests;
    }
    public List<LeaveAllocationVM> LeaveAllocations { get; set; }
    public List<LeaveRequestVM> LeaveRequests { get; set; }
}
