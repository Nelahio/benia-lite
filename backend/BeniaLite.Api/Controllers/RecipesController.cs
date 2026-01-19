using BeniaLite.Api.Auth;
using BeniaLite.Api.Data;
using BeniaLite.Api.Entities;
using BeniaLite.Api.MealPrep.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeniaLite.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class RecipesController : ControllerBase
{
    private readonly BeniaDbContext _db;
    public RecipesController(BeniaDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<RecipeListItemDto>>> List()
    {
        var userId = CurrentUser.GetUserId(User);

        return Ok(await _db.Recipes
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.Name)
            .Select(r => new RecipeListItemDto(r.Id, r.Name, r.Servings))
            .ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RecipeDetailDto>> Get(Guid id)
    {
        var userId = CurrentUser.GetUserId(User);

        var recipe = await _db.Recipes
            .Include(r => r.Ingredients)
            .SingleOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (recipe is null) return NotFound();

        return Ok(new RecipeDetailDto(
            recipe.Id,
            recipe.Name,
            recipe.Servings,
            recipe.Ingredients.Select(i => new RecipeIngredientDto(i.Name, i.Quantity, i.Unit)).ToList()
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRecipeRequest req)
    {
        var userId = CurrentUser.GetUserId(User);

        var recipe = new Recipe
        {
            UserId = userId,
            Name = req.Name.Trim(),
            Servings = req.Servings
        };

        foreach (var i in req.Ingredients)
        {
            recipe.Ingredients.Add(new RecipeIngredient
            {
                Name = i.Name.Trim(),
                Quantity = i.Quantity,
                Unit = i.Unit.Trim()
            });
        }

        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = recipe.Id }, null);
    }
}
