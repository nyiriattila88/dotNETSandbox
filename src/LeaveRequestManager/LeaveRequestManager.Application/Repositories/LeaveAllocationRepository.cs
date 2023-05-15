using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using LeaveRequestManager.Common.Models;
using LeaveRequestManager.Common.Constants;
using LeaveRequestManager.Data;
using LeaveRequestManager.Application.Contracts;

namespace LeaveRequestManager.Application.Repositories;

public class LeaveAllocationRepository : GenericRepository<LeaveAllocation>, ILeaveAllocationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Employee> _userManager;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IConfigurationProvider _configurationProvider;
    private readonly IEmailSender _emailSender;
    private readonly IMapper _mapper;

    public LeaveAllocationRepository(ApplicationDbContext context,
        UserManager<Employee> userManager,
        ILeaveTypeRepository leaveTypeRepository,
        IConfigurationProvider configurationProvider,
        IEmailSender emailSender,
        IMapper mapper) : base(context)
    {
        _context = context;
        _userManager = userManager;
        _leaveTypeRepository = leaveTypeRepository;
        _configurationProvider = configurationProvider;
        _emailSender = emailSender;
        _mapper = mapper;
    }

    public async Task<bool> AllocationExists(string employeeId, int leaveTypeId, int period) => await _context.LeaveAllocations.AnyAsync(q => q.EmployeeId == employeeId
                                                                                                                                                         && q.LeaveTypeId == leaveTypeId
                                                                                                                                                         && q.Period == period);

    public async Task<EmployeeAllocationVM> GetEmployeeAllocations(string employeeId)
    {
        List<LeaveAllocationVM> allocations = await _context.LeaveAllocations
            .Include(q => q.LeaveType)
            .Where(q => q.EmployeeId == employeeId)
            .ProjectTo<LeaveAllocationVM>(_configurationProvider)
            .ToListAsync();

        Employee employee = await _userManager.FindByIdAsync(employeeId);

        EmployeeAllocationVM employeeAllocationModel = _mapper.Map<EmployeeAllocationVM>(employee);
        employeeAllocationModel.LeaveAllocations = allocations;

        return employeeAllocationModel;
    }

    public async Task<LeaveAllocationEditVM> GetEmployeeAllocation(int id)
    {
        LeaveAllocationEditVM allocation = await _context.LeaveAllocations
            .Include(q => q.LeaveType)
            .ProjectTo<LeaveAllocationEditVM>(_configurationProvider)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (allocation is null)
            return null;

        Employee employee = await _userManager.FindByIdAsync(allocation.EmployeeId);

        LeaveAllocationEditVM model = _mapper.Map<LeaveAllocationEditVM>(allocation);
        model.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(allocation.EmployeeId));

        return model;
    }

    public async Task LeaveAllocation(int leaveTypeId)
    {
        IList<Employee> employees = await _userManager.GetUsersInRoleAsync(Roles.User);
        int period = DateTime.Now.Year;
        LeaveType leaveType = await _leaveTypeRepository.GetAsync(leaveTypeId);
        var allocations = new List<LeaveAllocation>();
        var employeesWithNewAllocations = new List<Employee>();

        foreach (Employee employee in employees)
        {
            if (await AllocationExists(employee.Id, leaveTypeId, period))
                continue;

            allocations.Add(new LeaveAllocation
            {
                EmployeeId = employee.Id,
                LeaveTypeId = leaveTypeId,
                Period = period,
                NumberOfDays = leaveType.DefaultDays
            });
            employeesWithNewAllocations.Add(employee);
        }

        await AddRangeAsync(allocations);

        foreach (Employee employee in employeesWithNewAllocations)
            await _emailSender.SendEmailAsync(employee.Email, $"Leave Allocation Posted for {period}", $"Your {leaveType.Name} " +
                $"has been posted for the period of {period}. You have been given {leaveType.DefaultDays}.");
    }

    public async Task<bool> UpdateEmployeeAllocation(LeaveAllocationEditVM model)
    {
        LeaveAllocation leaveAllocation = await GetAsync(model.Id);
        if (leaveAllocation is null)
            return false;

        leaveAllocation.Period = model.Period;
        leaveAllocation.NumberOfDays = model.NumberOfDays;
        await UpdateAsync(leaveAllocation);

        Employee user = await _userManager.FindByIdAsync(leaveAllocation.EmployeeId);

        await _emailSender.SendEmailAsync(user.Email, $"Leave Allocation Updated for {leaveAllocation.Period}",
            "Please review your leave allocations.");

        return true;
    }

    public async Task<LeaveAllocation> GetEmployeeAllocation(string employeeId, int leaveTypeId) => await _context.LeaveAllocations.FirstOrDefaultAsync(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId);
}
