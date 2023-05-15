using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using LeaveRequestManager.Common.Models;
using LeaveRequestManager.Data;
using LeaveRequestManager.Application.Contracts;

namespace LeaveRequestManager.Application.Repositories;

public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly IConfigurationProvider _configurationProvider;
    private readonly IEmailSender _emailSender;
    private readonly UserManager<Employee> _userManager;

    public LeaveRequestRepository(ApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ILeaveAllocationRepository leaveAllocationRepository,
        IConfigurationProvider configurationProvider,
        IEmailSender emailSender,
        UserManager<Employee> userManager) : base(context)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _leaveAllocationRepository = leaveAllocationRepository;
        _configurationProvider = configurationProvider;
        _emailSender = emailSender;
        _userManager = userManager;
    }

    public async Task CancelLeaveRequest(int leaveRequestId)
    {
        LeaveRequest leaveRequest = await GetAsync(leaveRequestId);
        leaveRequest.Cancelled = true;
        await UpdateAsync(leaveRequest);

        Employee user = await _userManager.FindByIdAsync(leaveRequest.RequestingEmployeeId);

        await _emailSender.SendEmailAsync(user.Email, $"Leave Request Cancelled", $"Your leave request from " +
            $"{leaveRequest.StartDate} to {leaveRequest.EndDate} has been Cancelled Successfully.");

    }

    public async Task ChangeApprovalStatus(int leaveRequestId, bool approved)
    {
        LeaveRequest leaveRequest = await GetAsync(leaveRequestId);
        leaveRequest.Approved = approved;

        if (approved)
        {
            LeaveAllocation allocation = await _leaveAllocationRepository.GetEmployeeAllocation(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
            int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
            allocation.NumberOfDays -= daysRequested;

            await _leaveAllocationRepository.UpdateAsync(allocation);
        }

        await UpdateAsync(leaveRequest);

        Employee user = await _userManager.FindByIdAsync(leaveRequest.RequestingEmployeeId);
        string approvalStatus = approved ? "Approved" : "Declined";

        await _emailSender.SendEmailAsync(user.Email, $"Leave Request {approvalStatus}", $"Your leave request from " +
            $"{leaveRequest.StartDate} to {leaveRequest.EndDate} has been {approvalStatus}");

    }

    public async Task<bool> CreateLeaveRequest(LeaveRequestCreateVM model)
    {
        Employee user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);

        LeaveAllocation leaveAllocation = await _leaveAllocationRepository.GetEmployeeAllocation(user.Id, model.LeaveTypeId);

        if (leaveAllocation is null)
            return false;

        int daysRequested = (int)(model.EndDate.Value - model.StartDate.Value).TotalDays;

        if (daysRequested > leaveAllocation.NumberOfDays)
            return false;

        LeaveRequest leaveRequest = _mapper.Map<LeaveRequest>(model);
        leaveRequest.DateRequested = DateTime.Now;
        leaveRequest.RequestingEmployeeId = user.Id;

        await AddAsync(leaveRequest);

        await _emailSender.SendEmailAsync(user.Email, "Leave Request Submitted Successfully", $"Your leave request from " +
            $"{leaveRequest.StartDate} to {leaveRequest.EndDate} has been submitted for approval");

        return true;
    }

    public async Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList()
    {
        List<LeaveRequest> leaveRequests = await _context.LeaveRequests.Include(q => q.LeaveType).ToListAsync();
        var model = new AdminLeaveRequestViewVM
        {
            TotalRequests = leaveRequests.Count,
            ApprovedRequests = leaveRequests.Count(q => q.Approved == true),
            PendingRequests = leaveRequests.Count(q => q.Approved is null),
            RejectedRequests = leaveRequests.Count(q => q.Approved == false),
            LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(leaveRequests),
        };

        foreach (LeaveRequestVM leaveRequest in model.LeaveRequests)
            leaveRequest.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(leaveRequest.RequestingEmployeeId));

        return model;
    }

    public async Task<List<LeaveRequestVM>> GetAllAsync(string employeeId) => await _context.LeaveRequests.Where(q => q.RequestingEmployeeId == employeeId)
            .ProjectTo<LeaveRequestVM>(_configurationProvider)
            .ToListAsync();

    public async Task<LeaveRequestVM> GetLeaveRequestAsync(int? id)
    {
        LeaveRequest leaveRequest = await _context.LeaveRequests
            .Include(q => q.LeaveType)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (leaveRequest is null)
            return null;

        LeaveRequestVM model = _mapper.Map<LeaveRequestVM>(leaveRequest);
        model.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(leaveRequest?.RequestingEmployeeId));
        return model;
    }

    public async Task<EmployeeLeaveRequestViewVM> GetMyLeaveDetails()
    {
        Employee user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
        List<LeaveAllocationVM> allocations = (await _leaveAllocationRepository.GetEmployeeAllocations(user.Id)).LeaveAllocations;
        List<LeaveRequestVM> requests = await GetAllAsync(user.Id);

        var model = new EmployeeLeaveRequestViewVM(allocations, requests);

        return model;
    }
}
