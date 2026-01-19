namespace BeniaLite.Api.Entities;

public sealed class Recipe
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public int Servings { get; set; } = 1;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<RecipeIngredient> Ingredients { get; set; } = new();
}
