using BeniaLite.Api.Auth;
using BeniaLite.Api.Data;
using BeniaLite.Api.MealPrep.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeniaLite.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class MealPlansController : ControllerBase
{
    private readonly BeniaDbContext _db;
    public MealPlansController(BeniaDbContext db) => _db = db;

    [HttpGet("week")]
    public async Task<ActionResult<List<MealPlanDto>>> Week([FromQuery] DateTime start)
    {
        var userId = CurrentUser.GetUserId(User);

        var startUtc = DateTime.SpecifyKind(start.Date, DateTimeKind.Utc);
        var endUtc = startUtc.AddDays(7);

        return Ok(await _db.MealPlans
            .Include(m => m.Recipe)
            .Where(m => m.UserId == userId && m.DayUtc >= startUtc && m.DayUtc < endUtc)
            .OrderBy(m => m.DayUtc)
            .Select(m => new MealPlanDto(m.DayUtc, m.MealType, m.RecipeId, m.Recipe!.Name))
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMealPlanRequest req)
    {
        var userId = CurrentUser.GetUserId(User);
        var dayUtc = req.DayUtc.Kind == DateTimeKind.Utc
            ? req.DayUtc.Date
            : DateTime.SpecifyKind(req.DayUtc.Date, DateTimeKind.Utc);

        _db.MealPlans.Add(new Entities.MealPlan
        {
            UserId = userId,
            DayUtc = dayUtc,
            MealType = req.MealType.Trim(),
            RecipeId = req.RecipeId
        });

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("shopping-list")]
    public async Task<ActionResult<List<ShoppingListItemDto>>> ShoppingList([FromQuery] DateTime start)
    {
        var userId = CurrentUser.GetUserId(User);
        var startUtc = DateTime.SpecifyKind(start.Date, DateTimeKind.Utc);
        var endUtc = startUtc.AddDays(7);

        var ingredients = await _db.MealPlans
        .Where(m => m.UserId == userId && m.DayUtc >= startUtc && m.DayUtc < endUtc)
        .Include(m => m.Recipe)
            .ThenInclude(r => r!.Ingredients)
        .SelectMany(m => m.Recipe!.Ingredients)
        .Select(i => new { i.Name, i.Unit, i.Quantity })
        .ToListAsync();

        var items = ingredients
        .GroupBy(i => new { Name = i.Name, Unit = i.Unit })
        .Select(g => new ShoppingListItemDto(
            g.Key.Name,
            g.Sum(x => x.Quantity),
            g.Key.Unit
        ))
        .OrderBy(x => x.Name)
        .ToList();

        return Ok(items);
    }
}
