using AutoMapper;
using LeaveRequestManager.Common.Models;
using LeaveRequestManager.Data;

namespace LeaveRequestManager.Application.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<LeaveType, LeaveTypeVM>().ReverseMap();
        CreateMap<Employee, EmployeeListVM>().ReverseMap();
        CreateMap<Employee, EmployeeAllocationVM>().ReverseMap();
        CreateMap<LeaveAllocation, LeaveAllocationVM>().ReverseMap();
        CreateMap<LeaveAllocation, LeaveAllocationEditVM>().ReverseMap();
        CreateMap<LeaveRequest, LeaveRequestCreateVM>().ReverseMap();
        CreateMap<LeaveRequest, LeaveRequestVM>().ReverseMap();
    }
}
