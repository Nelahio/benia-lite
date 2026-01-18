using BeniaLite.Api.Auth;
using BeniaLite.Api.Data;
using BeniaLite.Api.Entities;
using BeniaLite.Api.Routines.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeniaLite.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class RoutinesController : ControllerBase
{
    private readonly BeniaDbContext _db;

    public RoutinesController(BeniaDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<RoutineListItemDto>>> List()
    {
        var userId = CurrentUser.GetUserId(User);

        var data = await _db.Routines
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new RoutineListItemDto(r.Id, r.Name, r.Category, r.IsActive))
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoutineDetailDto>> Get(Guid id)
    {
        var userId = CurrentUser.GetUserId(User);

        var routine = await _db.Routines
            .Include(r => r.Steps.OrderBy(s => s.SortOrder))
            .SingleOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (routine is null) return NotFound();

        var dto = new RoutineDetailDto(
            routine.Id,
            routine.Name,
            routine.Category,
            routine.IsActive,
            routine.Steps
                .OrderBy(s => s.SortOrder)
                .Select(s => new RoutineStepDto(s.Id, s.Title, s.Notes, s.SortOrder, s.FrequencyType, s.FrequencyValue))
                .ToList()
        );

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<RoutineDetailDto>> Create(CreateRoutineRequest req)
    {
        var userId = CurrentUser.GetUserId(User);

        var routine = new Routine
        {
            UserId = userId,
            Name = req.Name.Trim(),
            Category = req.Category.Trim()
        };

        _db.Routines.Add(routine);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = routine.Id },
            new RoutineDetailDto(routine.Id, routine.Name, routine.Category, routine.IsActive, new()));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateRoutineRequest req)
    {
        var userId = CurrentUser.GetUserId(User);

        var routine = await _db.Routines.SingleOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (routine is null) return NotFound();

        routine.Name = req.Name.Trim();
        routine.Category = req.Category.Trim();
        routine.IsActive = req.IsActive;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/steps")]
    public async Task<IActionResult> AddStep(Guid id, AddStepRequest req)
    {
        var userId = CurrentUser.GetUserId(User);

        var routine = await _db.Routines.SingleOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (routine is null) return NotFound();

        var exists = await _db.RoutineSteps.AnyAsync(s => s.RoutineId == id && s.SortOrder == req.SortOrder);
        if (exists) return Conflict("SortOrder already exists for this routine.");

        var step = new RoutineStep
        {
            RoutineId = id,
            Title = req.Title.Trim(),
            Notes = req.Notes?.Trim(),
            SortOrder = req.SortOrder,
            FrequencyType = req.FrequencyType.Trim(),
            FrequencyValue = req.FrequencyValue
        };

        _db.RoutineSteps.Add(step);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id }, null);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CompleteRoutineRequest req)
    {
        var userId = CurrentUser.GetUserId(User);

        var routine = await _db.Routines.SingleOrDefaultAsync(r => r.Id == id && r.UserId == userId && r.IsActive);
        if (routine is null) return NotFound();

        var alreadyCompletedToday = await _db.RoutineCompletions.AnyAsync(c =>
            c.UserId == userId &&
            c.RoutineId == id &&
            c.CompletedAtUtc.Date == DateTime.UtcNow.Date
        );

        if (alreadyCompletedToday)
            return Conflict("Routine already completed today.");

        _db.RoutineCompletions.Add(new RoutineCompletion
        {
            UserId = userId,
            RoutineId = id,
            Notes = req.Notes
        });

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:guid}/complete/today")]
    public async Task<IActionResult> DeleteCompleteToday(Guid id)
    {
        var userId = CurrentUser.GetUserId(User);

        var start = DateTime.UtcNow.Date;
        var end = start.AddDays(1);

        var completedToday = await _db.RoutineCompletions.SingleOrDefaultAsync(c =>
            c.UserId == userId &&
            c.RoutineId == id &&
            c.CompletedAtUtc >= start &&
            c.CompletedAtUtc < end);

        if (completedToday is null)
            return NotFound();

        _db.RoutineCompletions.Remove(completedToday);

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
