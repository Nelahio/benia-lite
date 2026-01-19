namespace BeniaLite.Api.MealPrep.Contracts;

public sealed record RecipeIngredientDto(string Name, double Quantity, string Unit);

public sealed record RecipeListItemDto(Guid Id, string Name, int Servings);

public sealed record RecipeDetailDto(Guid Id, string Name, int Servings, List<RecipeIngredientDto> Ingredients);

public sealed record CreateRecipeRequest(string Name, int Servings, List<RecipeIngredientDto> Ingredients);

public sealed record MealPlanDto(DateTime DayUtc, string MealType, Guid RecipeId, string RecipeName);

public sealed record CreateMealPlanRequest(DateTime DayUtc, string MealType, Guid RecipeId);

public sealed record ShoppingListItemDto(string Name, double Quantity, string Unit);
