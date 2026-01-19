namespace BeniaLite.Api.Entities;

public sealed class MealPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public DateTime DayUtc { get; set; }
    public string MealType { get; set; } = "Lunch"; // Breakfast/Snack/Lunch/Dinner

    public Guid RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
}
