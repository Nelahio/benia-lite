import { useEffect, useMemo, useState } from "react";
import { api } from "../api/http";

function isoDate(d) {
  return d.toISOString().slice(0, 10);
}

function startOfWeekMonday(date = new Date()) {
  const d = new Date(date);
  const day = d.getDay(); // 0=Sun,1=Mon...
  const diff = (day === 0 ? -6 : 1) - day;
  d.setDate(d.getDate() + diff);
  d.setHours(0, 0, 0, 0);
  return d;
}

export default function MealPrepPage() {
  const [tab, setTab] = useState("recipes"); // recipes | plan | shopping

  return (
    <div>
      <div className="spaceBetween" style={{ marginBottom: 14 }}>
        <div>
          <h1 className="h1">Meal prep</h1>
          <div className="muted">Recettes • Planning • Liste de courses</div>
        </div>

        <div className="tabs">
          <button
            className={`tab ${tab === "recipes" ? "tabActive" : ""}`}
            onClick={() => setTab("recipes")}
          >
            Recipes
          </button>
          <button
            className={`tab ${tab === "plan" ? "tabActive" : ""}`}
            onClick={() => setTab("plan")}
          >
            Week plan
          </button>
          <button
            className={`tab ${tab === "shopping" ? "tabActive" : ""}`}
            onClick={() => setTab("shopping")}
          >
            Shopping list
          </button>
        </div>
      </div>

      {tab === "recipes" ? <RecipesTab /> : null}
      {tab === "plan" ? <WeekPlanTab /> : null}
      {tab === "shopping" ? <ShoppingListTab /> : null}
    </div>
  );
}

/* -------------------- TAB 1: RECIPES -------------------- */

function RecipesTab() {
  const [recipes, setRecipes] = useState([]);
  const [selectedId, setSelectedId] = useState(null);
  const [detail, setDetail] = useState(null);
  const [error, setError] = useState("");

  // form
  const [name, setName] = useState("");
  const [servings, setServings] = useState(2);
  const [ingredients, setIngredients] = useState([
    { name: "", quantity: 0, unit: "g" },
  ]);
  const [loading, setLoading] = useState(false);

  async function loadList() {
    setError("");
    try {
      const data = await api.mealPrep.recipes();
      setRecipes(data);
    } catch (e) {
      setError(e.message || "Erreur");
    }
  }

  async function loadDetail(id) {
    setError("");
    try {
      const d = await api.mealPrep.recipe(id);
      setDetail(d);
    } catch (e) {
      setError(e.message || "Erreur");
    }
  }

  useEffect(() => {
    loadList();
  }, []);

  useEffect(() => {
    if (selectedId) loadDetail(selectedId);
    else setDetail(null);
  }, [selectedId]);

  function updateIngredient(idx, patch) {
    setIngredients((prev) =>
      prev.map((x, i) => (i === idx ? { ...x, ...patch } : x)),
    );
  }

  function addIngredientRow() {
    setIngredients((prev) => [...prev, { name: "", quantity: 0, unit: "g" }]);
  }

  function removeIngredientRow(idx) {
    setIngredients((prev) => prev.filter((_, i) => i !== idx));
  }

  async function createRecipe(e) {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const payload = {
        name,
        servings: Number(servings),
        ingredients: ingredients
          .map((i) => ({
            name: (i.name || "").trim(),
            quantity: Number(i.quantity),
            unit: (i.unit || "").trim(),
          }))
          .filter((i) => i.name.length > 0 && !Number.isNaN(i.quantity)),
      };

      await api.mealPrep.createRecipe(payload);

      // reset form
      setName("");
      setServings(2);
      setIngredients([{ name: "", quantity: 0, unit: "g" }]);

      await loadList();
    } catch (e2) {
      setError(e2.message || "Erreur");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="grid2">
      {/* LEFT: list */}
      <div className="card" style={{ padding: 14 }}>
        <div className="spaceBetween" style={{ marginBottom: 10 }}>
          <h2 className="h2">Recipes</h2>
          <button className="btn" onClick={loadList}>
            Refresh
          </button>
        </div>

        {error ? <div className="error">{error}</div> : null}

        <div style={{ display: "grid", gap: 8, marginTop: 10 }}>
          {recipes.map((r) => (
            <button
              key={r.id}
              className="btn"
              onClick={() => setSelectedId(r.id)}
              style={{
                textAlign: "left",
                borderColor: selectedId === r.id ? "#c7cad1" : undefined,
                background: selectedId === r.id ? "#f9fafb" : undefined,
              }}
            >
              <div style={{ fontWeight: 900 }}>{r.name}</div>
              <div className="muted" style={{ fontSize: 13 }}>
                {r.servings} servings
              </div>
            </button>
          ))}
          {recipes.length === 0 ? (
            <div className="muted">Aucune recette pour le moment.</div>
          ) : null}
        </div>

        {/* detail */}
        {detail ? (
          <div style={{ marginTop: 14 }}>
            <div className="spaceBetween">
              <div>
                <div style={{ fontWeight: 900, fontSize: 16 }}>
                  {detail.name}
                </div>
                <div className="muted" style={{ fontSize: 13 }}>
                  {detail.servings} servings
                </div>
              </div>
              <button
                className="btn btnGhost"
                onClick={() => setSelectedId(null)}
              >
                Close
              </button>
            </div>

            <div style={{ marginTop: 10 }}>
              <div style={{ fontWeight: 800, marginBottom: 6 }}>
                Ingredients
              </div>
              <ul style={{ margin: 0, paddingLeft: 18 }}>
                {detail.ingredients.map((i, idx) => (
                  <li key={idx}>
                    {i.name} — {i.quantity} {i.unit}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        ) : null}
      </div>

      {/* RIGHT: create */}
      <div className="card" style={{ padding: 14 }}>
        <h2 className="h2" style={{ marginBottom: 10 }}>
          Create recipe
        </h2>

        <form onSubmit={createRecipe} style={{ display: "grid", gap: 12 }}>
          <label className="label">
            Name
            <input
              className="input"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
          </label>

          <label className="label">
            Servings
            <input
              className="input"
              type="number"
              min="1"
              value={servings}
              onChange={(e) => setServings(e.target.value)}
              required
            />
          </label>

          <div>
            <div style={{ fontWeight: 800, marginBottom: 6 }}>Ingredients</div>

            <div style={{ display: "grid", gap: 10 }}>
              {ingredients.map((ing, idx) => (
                <div
                  key={idx}
                  style={{
                    display: "grid",
                    gridTemplateColumns: "2fr 1fr 1fr auto",
                    gap: 8,
                  }}
                >
                  <input
                    className="input"
                    placeholder="ex: Poulet"
                    value={ing.name}
                    onChange={(e) =>
                      updateIngredient(idx, { name: e.target.value })
                    }
                  />
                  <input
                    className="input"
                    type="number"
                    step="0.1"
                    value={ing.quantity}
                    onChange={(e) =>
                      updateIngredient(idx, { quantity: e.target.value })
                    }
                  />
                  <input
                    className="input"
                    placeholder="g"
                    value={ing.unit}
                    onChange={(e) =>
                      updateIngredient(idx, { unit: e.target.value })
                    }
                  />
                  <button
                    className="btn btnGhost"
                    type="button"
                    onClick={() => removeIngredientRow(idx)}
                    disabled={ingredients.length === 1}
                    title="Remove"
                  >
                    ✕
                  </button>
                </div>
              ))}
            </div>

            <button
              className="btn"
              type="button"
              onClick={addIngredientRow}
              style={{ marginTop: 10 }}
            >
              + Add ingredient
            </button>
          </div>

          <button className="btn btnPrimary" type="submit" disabled={loading}>
            {loading ? "Saving..." : "Create"}
          </button>
        </form>
      </div>
    </div>
  );
}

/* -------------------- TAB 2: WEEK PLAN -------------------- */

function WeekPlanTab() {
  const [weekStart, setWeekStart] = useState(
    isoDate(startOfWeekMonday(new Date())),
  );
  const [recipes, setRecipes] = useState([]);
  const [meals, setMeals] = useState([]);
  const [error, setError] = useState("");

  const mealTypes = ["Breakfast", "Lunch", "Dinner"];

  const days = useMemo(() => {
    const start = new Date(weekStart);
    start.setHours(0, 0, 0, 0);
    return Array.from({ length: 7 }, (_, i) => {
      const d = new Date(start);
      d.setDate(start.getDate() + i);
      return d;
    });
  }, [weekStart]);

  async function load() {
    setError("");
    try {
      const [r, w] = await Promise.all([
        api.mealPrep.recipes(),
        api.mealPrep.week(weekStart),
      ]);
      setRecipes(r);
      setMeals(w);
    } catch (e) {
      setError(e.message || "Erreur");
    }
  }

  useEffect(() => {
    (async () => {
      await load();
    })();
  }, [weekStart]);

  function getMeal(dayUtc, mealType) {
    const keyDay = dayUtc.toISOString().slice(0, 10);
    return (
      meals.find(
        (m) => m.dayUtc.slice(0, 10) === keyDay && m.mealType === mealType,
      ) || null
    );
  }

  async function assignMeal(dayUtc, mealType, recipeId) {
    setError("");
    try {
      await api.mealPrep.addMeal({
        dayUtc: dayUtc.toISOString(),
        mealType,
        recipeId,
      });
      await load();
    } catch (e) {
      setError(e.message || "Erreur");
    }
  }

  return (
    <div className="card" style={{ padding: 14 }}>
      <div className="spaceBetween" style={{ marginBottom: 12 }}>
        <div>
          <h2 className="h2">Week plan</h2>
          <div className="muted">Planifie tes repas sur la semaine</div>
        </div>

        <div className="row">
          <label className="label" style={{ width: 200 }}>
            Week start (Monday)
            <input
              className="input"
              type="date"
              value={weekStart}
              onChange={(e) => setWeekStart(e.target.value)}
            />
          </label>
          <button className="btn" onClick={load}>
            Refresh
          </button>
        </div>
      </div>

      {error ? <div className="error">{error}</div> : null}

      <div style={{ overflowX: "auto" }}>
        <table
          style={{
            width: "100%",
            borderCollapse: "separate",
            borderSpacing: 0,
          }}
        >
          <thead>
            <tr>
              <th
                style={{
                  textAlign: "left",
                  padding: 10,
                  borderBottom: "1px solid var(--border)",
                }}
              >
                Day
              </th>
              {mealTypes.map((mt) => (
                <th
                  key={mt}
                  style={{
                    textAlign: "left",
                    padding: 10,
                    borderBottom: "1px solid var(--border)",
                  }}
                >
                  {mt}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {days.map((d) => (
              <tr key={d.toISOString()}>
                <td
                  style={{
                    padding: 10,
                    borderBottom: "1px solid var(--border)",
                    whiteSpace: "nowrap",
                  }}
                >
                  <div style={{ fontWeight: 900 }}>
                    {d.toLocaleDateString()}
                  </div>
                </td>

                {mealTypes.map((mt) => {
                  const m = getMeal(d, mt);
                  return (
                    <td
                      key={mt}
                      style={{
                        padding: 10,
                        borderBottom: "1px solid var(--border)",
                      }}
                    >
                      <select
                        className="input"
                        value={m?.recipeId || ""}
                        onChange={(e) => {
                          const v = e.target.value;
                          if (!v) return;
                          assignMeal(d, mt, v);
                        }}
                      >
                        <option value="">— Select recipe —</option>
                        {recipes.map((r) => (
                          <option key={r.id} value={r.id}>
                            {r.name}
                          </option>
                        ))}
                      </select>

                      {m ? (
                        <div
                          className="muted"
                          style={{ fontSize: 13, marginTop: 6 }}
                        >
                          Selected:{" "}
                          <b style={{ color: "var(--text)" }}>{m.recipeName}</b>
                        </div>
                      ) : null}
                    </td>
                  );
                })}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="muted" style={{ fontSize: 13, marginTop: 10 }}>
        Note : si tu choisis deux fois le même slot (jour + mealType), l’API
        peut renvoyer un conflit (index unique).
      </div>
    </div>
  );
}

function ShoppingListTab() {
  const [weekStart, setWeekStart] = useState(
    isoDate(startOfWeekMonday(new Date())),
  );
  const [items, setItems] = useState([]);
  const [error, setError] = useState("");

  async function load() {
    setError("");
    try {
      const data = await api.mealPrep.shoppingList(weekStart);
      setItems(data);
    } catch (e) {
      setError(e.message || "Erreur");
    }
  }

  useEffect(() => {
    (async () => {
      await load();
    })();
  }, [weekStart]);

  return (
    <div className="card" style={{ padding: 14 }}>
      <div className="spaceBetween" style={{ marginBottom: 12 }}>
        <div>
          <h2 className="h2">Shopping list</h2>
          <div className="muted">Générée à partir du planning</div>
        </div>

        <div className="row">
          <label className="label" style={{ width: 200 }}>
            Week start
            <input
              className="input"
              type="date"
              value={weekStart}
              onChange={(e) => setWeekStart(e.target.value)}
            />
          </label>
          <button className="btn" onClick={load}>
            Refresh
          </button>
        </div>
      </div>

      {error ? <div className="error">{error}</div> : null}

      {items.length === 0 ? (
        <div className="muted">
          Aucun item. Planifie des repas pour générer la liste.
        </div>
      ) : (
        <div style={{ display: "grid", gap: 8 }}>
          {items.map((i, idx) => (
            <div
              key={idx}
              className="card"
              style={{ padding: 12, boxShadow: "none" }}
            >
              <div style={{ fontWeight: 900 }}>{i.name}</div>
              <div className="muted">
                {i.quantity} {i.unit}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
