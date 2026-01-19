import { useState } from "react";
import { api, setToken } from "../api/http";

export default function LoginPage() {
  const [mode, setMode] = useState("login"); // "login" | "register"
  const [email, setEmail] = useState("test@benia.com");
  const [password, setPassword] = useState("Password123!");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function submit(e) {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const res =
        mode === "login"
          ? await api.auth.login(email, password)
          : await api.auth.register(email, password);

      // res = { accessToken }
      setToken(res.accessToken);

      // redirection vers l'app
      window.location.href = "/";
    } catch (err) {
      setError(err.message || "Erreur lors de l’authentification");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div
      style={{
        maxWidth: 420,
        margin: "80px auto",
        fontFamily: "system-ui",
        border: "1px solid #eee",
        borderRadius: 12,
        padding: 20,
      }}
    >
      <h1 style={{ marginBottom: 4 }}>Benia Lite</h1>
      <p style={{ color: "#666", marginTop: 0 }}>
        {mode === "login" ? "Connexion à votre espace" : "Création d’un compte"}
      </p>

      <div style={{ display: "flex", gap: 8, marginBottom: 16 }}>
        <button
          type="button"
          onClick={() => setMode("login")}
          disabled={mode === "login"}
        >
          Login
        </button>
        <button
          type="button"
          onClick={() => setMode("register")}
          disabled={mode === "register"}
        >
          Register
        </button>
      </div>

      <form onSubmit={submit} style={{ display: "grid", gap: 12 }}>
        <label>
          Email
          <input
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            style={{ width: "100%" }}
            required
          />
        </label>

        <label>
          Password
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={{ width: "100%" }}
            required
          />
        </label>

        {error && <div style={{ color: "crimson", fontSize: 14 }}>{error}</div>}

        <button type="submit" disabled={loading}>
          {loading
            ? "Chargement..."
            : mode === "login"
              ? "Se connecter"
              : "Créer le compte"}
        </button>
      </form>
    </div>
  );
}
