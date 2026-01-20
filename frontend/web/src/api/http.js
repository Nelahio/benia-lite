const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export function getToken() {
  return localStorage.getItem("benia_token");
}

export function setToken(token) {
  localStorage.setItem("benia_token", token);
}

export function clearToken() {
  localStorage.removeItem("benia_token");
}

async function request(path, options = {}) {
  const token = getToken();

  const headers = {
    "Content-Type": "application/json",
    ...(options.headers || {}),
  };

  if (token) headers.Authorization = `Bearer ${token}`;

  const res = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  });

  // Si token invalide/expiré
  if (res.status === 401) {
    clearToken();
  }

  const contentType = res.headers.get("content-type") || "";
  const hasJson = contentType.includes("application/json");

  if (!res.ok) {
    const body = hasJson
      ? await res.json().catch(() => null)
      : await res.text().catch(() => "");
    const message =
      typeof body === "string" && body.length
        ? body
        : body?.message || body || `HTTP ${res.status}`;
    throw new Error(message);
  }

  return hasJson ? res.json() : null;
}

export const api = {
  auth: {
    register: (email, password) =>
      request("/api/auth/register", {
        method: "POST",
        body: JSON.stringify({ email, password }),
      }),
    login: (email, password) =>
      request("/api/auth/login", {
        method: "POST",
        body: JSON.stringify({ email, password }),
      }),
    me: () => request("/api/auth/me"),
  },

  routines: {
    today: () => request("/api/routines/today"),
    complete: (id, notes = null) =>
      request(`/api/routines/${id}/complete`, {
        method: "POST",
        body: JSON.stringify({ notes }),
      }),
    undoToday: (id) =>
      request(`/api/routines/${id}/complete/today`, { method: "DELETE" }),
  },

  symptoms: {
    list: () => request("/api/symptoms"),
    stats: (days = 7, category = null) => {
      const qs = new URLSearchParams({ days: String(days) });
      if (category) qs.set("category", category);
      return request(`/api/symptoms/stats?${qs.toString()}`);
    },
  },

  mealPrep: {
    recipes: () => request("/api/recipes"),
    recipe: (id) => request(`/api/recipes/${id}`),
    createRecipe: (payload) =>
      request("/api/recipes", {
        method: "POST",
        body: JSON.stringify(payload),
      }),

    week: (start) =>
      request(`/api/mealplans/week?start=${encodeURIComponent(start)}`),
    addMeal: (payload) =>
      request("/api/mealplans", {
        method: "POST",
        body: JSON.stringify(payload),
      }),

    shoppingList: (start) =>
      request(
        `/api/mealplans/shopping-list?start=${encodeURIComponent(start)}`,
      ),
  },
};
