using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeaveRequestManager.Common.Models;
using LeaveRequestManager.Common.Constants;
using LeaveRequestManager.Application.Contracts;

namespace LeaveRequestManager.Web.Controllers;

[Authorize]
public class LeaveRequestsController : Controller
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;

    public LeaveRequestsController(ILeaveRequestRepository leaveRequestRepository, ILeaveTypeRepository leaveTypeRepository)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _leaveTypeRepository = leaveTypeRepository;
    }

    [Authorize(Roles = Roles.Administrator)]
    // GET: LeaveRequests
    public async Task<IActionResult> Index()
    {
        AdminLeaveRequestViewVM model = await _leaveRequestRepository.GetAdminLeaveRequestList();
        return View(model);
    }

    public async Task<ActionResult> MyLeave()
    {
        EmployeeLeaveRequestViewVM model = await _leaveRequestRepository.GetMyLeaveDetails();
        return View(model);
    }

    // GET: LeaveRequests/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        LeaveRequestVM model = await _leaveRequestRepository.GetLeaveRequestAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveRequest(int id, bool approved)
    {
        try
        {
            await _leaveRequestRepository.ChangeApprovalStatus(id, approved);
        }
        catch (Exception)
        {

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _leaveRequestRepository.CancelLeaveRequest(id);
        }
        catch (Exception)
        {

            throw;
        }

        return RedirectToAction(nameof(MyLeave));
    }

    // GET: LeaveRequests/Create
    public async Task<IActionResult> Create()
    {
        var model = new LeaveRequestCreateVM
        {
            LeaveTypes = new SelectList(await _leaveTypeRepository.GetAllAsync(), "Id", "Name")
        };
        return View(model);
    }

    // POST: LeaveRequests/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveRequestCreateVM model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                bool isValidRequest = await _leaveRequestRepository.CreateLeaveRequest(model);
                if (isValidRequest)
                    return RedirectToAction(nameof(MyLeave));

                ModelState.AddModelError(string.Empty, "You have exceeded your allocation with this request.");
            }
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An Error Has Occurred. Please Try Again Later");
        }

        model.LeaveTypes = new SelectList(await _leaveTypeRepository.GetAllAsync(), "Id", "Name", model.LeaveTypeId);
        return View(model);
    }
}
