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
        var end = start.AddDays(7);

        return Ok(await _db.MealPlans
            .Include(m => m.Recipe)
            .Where(m => m.UserId == userId && m.DayUtc >= start && m.DayUtc < end)
            .OrderBy(m => m.DayUtc)
            .Select(m => new MealPlanDto(m.DayUtc, m.MealType, m.RecipeId, m.Recipe!.Name))
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMealPlanRequest req)
    {
        var userId = CurrentUser.GetUserId(User);

        _db.MealPlans.Add(new Entities.MealPlan
        {
            UserId = userId,
            DayUtc = req.DayUtc.Date,
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
        var end = start.AddDays(7);

        var items = await _db.MealPlans
            .Include(m => m.Recipe).ThenInclude(r => r.Ingredients)
            .Where(m => m.UserId == userId && m.DayUtc >= start && m.DayUtc < end)
            .SelectMany(m => m.Recipe!.Ingredients)
            .GroupBy(i => new { i.Name, i.Unit })
            .Select(g => new ShoppingListItemDto(
                g.Key.Name,
                g.Sum(x => x.Quantity),
                g.Key.Unit
            ))
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(items);
    }
}
