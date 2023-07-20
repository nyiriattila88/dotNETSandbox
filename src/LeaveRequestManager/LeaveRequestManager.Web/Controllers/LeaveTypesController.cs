using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using LeaveRequestManager.Common.Constants;
using LeaveRequestManager.Common.Models;
using LeaveRequestManager.Data;
using LeaveRequestManager.Application.Contracts;

namespace LeaveRequestManager.Web.Controllers;

[Authorize(Roles = Roles.Administrator)]

public class LeaveTypesController : Controller
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IMapper _mapper;
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;

    public LeaveTypesController(ILeaveTypeRepository leaveTypeRepository
        , IMapper mapper
        , ILeaveAllocationRepository leaveAllocationRepository)
    {
        _leaveTypeRepository = leaveTypeRepository;
        _mapper = mapper;
        _leaveAllocationRepository = leaveAllocationRepository;
    }

    // GET: LeaveTypes
    public async Task<ActionResult> Index()
    {
        List<LeaveTypeVM> leaveTypes = _mapper.Map<List<LeaveTypeVM>>(await _leaveTypeRepository.GetAllAsync());
        return View(leaveTypes);
    }

    // GET: LeaveTypes/Details/5
    public async Task<ActionResult> Details(int? id)
    {
        LeaveType leaveType = await _leaveTypeRepository.GetAsync(id);
        if (leaveType is null)
            return NotFound();

        LeaveTypeVM leaveTypeVM = _mapper.Map<LeaveTypeVM>(leaveType);
        return View(leaveTypeVM);
    }

    // GET: LeaveTypes/Create
    public ActionResult Create() => View();

    // POST: LeaveTypes/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<ActionResult> Create(LeaveTypeVM leaveTypeVM)
    {
        if (ModelState.IsValid)
        {
            LeaveType leaveType = _mapper.Map<LeaveType>(leaveTypeVM);
            await _leaveTypeRepository.AddAsync(leaveType);
            return RedirectToAction(nameof(Index));
        }

        return View(leaveTypeVM);
    }

    // GET: LeaveTypes/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
        LeaveType leaveType = await _leaveTypeRepository.GetAsync(id);
        if (leaveType is null)
            return NotFound();

        LeaveTypeVM leaveTypeVM = _mapper.Map<LeaveTypeVM>(leaveType);
        return View(leaveTypeVM);
    }

    // POST: LeaveTypes/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(int id, LeaveTypeVM leaveTypeVM)
    {
        if (id != leaveTypeVM.Id)
            return NotFound();

        LeaveType leaveType = await _leaveTypeRepository.GetAsync(id);

        if (leaveType is null)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _mapper.Map(leaveTypeVM, leaveType);
                await _leaveTypeRepository.UpdateAsync(leaveType);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _leaveTypeRepository.Exists(leaveTypeVM.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(leaveTypeVM);
    }

    // POST: LeaveTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        await _leaveTypeRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> AllocateLeave(int id)
    {
        await _leaveAllocationRepository.LeaveAllocation(id);
        return RedirectToAction(nameof(Index));
    }
}
