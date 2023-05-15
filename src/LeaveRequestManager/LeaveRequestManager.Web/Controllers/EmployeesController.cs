using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LeaveRequestManager.Common.Models;
using LeaveRequestManager.Common.Constants;
using LeaveRequestManager.Data;
using LeaveRequestManager.Application.Contracts;

namespace LeaveRequestManager.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly UserManager<Employee> _userManager;
    private readonly IMapper _mapper;
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;

    public EmployeesController(UserManager<Employee> userManager,
        IMapper mapper, ILeaveAllocationRepository leaveAllocationRepository,
        ILeaveTypeRepository leaveTypeRepository)
    {
        _userManager = userManager;
        _mapper = mapper;
        _leaveAllocationRepository = leaveAllocationRepository;
        _leaveTypeRepository = leaveTypeRepository;
    }

    // GET: EmployeesController
    public async Task<IActionResult> Index()
    {
        IList<Employee> employees = await _userManager.GetUsersInRoleAsync(Roles.User);
        List<EmployeeListVM> model = _mapper.Map<List<EmployeeListVM>>(employees);
        return View(model);
    }

    // GET: EmployeesController/ViewAllocations/employeeId
    public async Task<ActionResult> ViewAllocations(string id)
    {
        EmployeeAllocationVM model = await _leaveAllocationRepository.GetEmployeeAllocations(id);
        return View(model);
    }

    // GET: EmployeesController/EditAllocation/5
    public async Task<ActionResult> EditAllocation(int id)
    {
        LeaveAllocationEditVM model = await _leaveAllocationRepository.GetEmployeeAllocation(id);
        return model is null ? NotFound() : View(model);
    }

    // POST: EmployeesController/EditAllocation/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditAllocation(int id, LeaveAllocationEditVM model)
    {
        try
        {
            if (ModelState.IsValid)
                if (await _leaveAllocationRepository.UpdateEmployeeAllocation(model))
                    return RedirectToAction(nameof(ViewAllocations), new { id = model.EmployeeId });
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An Error Has Occurred. Please Try Again Later");
        }

        model.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(model.EmployeeId));
        model.LeaveType = _mapper.Map<LeaveTypeVM>(await _leaveTypeRepository.GetAsync(model.LeaveTypeId));
        return View(model);
    }
}
