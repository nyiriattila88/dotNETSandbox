using LeaveRequestManager.Application.Contracts;
using LeaveRequestManager.Data;

namespace LeaveRequestManager.Application.Repositories;

public class LeaveTypeRepository : GenericRepository<LeaveType>, ILeaveTypeRepository
{
    public LeaveTypeRepository(ApplicationDbContext context) : base(context)
    {
    }
}
