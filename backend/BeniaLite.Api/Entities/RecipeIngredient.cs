namespace BeniaLite.Api.Entities;

public sealed class RecipeIngredient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RecipeId { get; set; }

    public string Name { get; set; } = string.Empty; // "riz", "poulet"
    public double Quantity { get; set; }
    public string Unit { get; set; } = "g"; // g, ml, unit

    public Recipe? Recipe { get; set; }
}
