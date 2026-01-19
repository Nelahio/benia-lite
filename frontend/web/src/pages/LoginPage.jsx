import { useState } from "react";
import { api, setToken } from "../api/http";

export default function LoginPage() {
  const [mode, setMode] = useState("login"); // "login" | "register"
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
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

      setToken(res.accessToken);
      window.location.href = "/";
    } catch (err) {
      setError(err.message || "Erreur lors de l'authentification");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div
      className="container"
      style={{
        minHeight: "calc(100vh - 40px)",
        display: "grid",
        placeItems: "center",
      }}
    >
      <div
        className="card"
        style={{ width: "100%", maxWidth: 440, padding: 18 }}
      >
        <div className="sectionTitle">
          <div className="h1">Benia Lite</div>
          <div className="muted" style={{ marginTop: 6 }}>
            {mode === "login" ? "Connexion" : "Création de compte"} — JWT + API
            .NET 10
          </div>
        </div>

        <div className="row" style={{ marginBottom: 14 }}>
          <button
            className={`btn ${mode === "login" ? "btnPrimary" : ""}`}
            type="button"
            onClick={() => setMode("login")}
          >
            Login
          </button>
          <button
            className={`btn ${mode === "register" ? "btnPrimary" : ""}`}
            type="button"
            onClick={() => setMode("register")}
          >
            Register
          </button>
        </div>

        <form onSubmit={submit} style={{ display: "grid", gap: 12 }}>
          <label className="label">
            Email
            <input
              className="input"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="ex: amandine@mail.com"
              required
            />
          </label>

          <label className="label">
            Password
            <input
              className="input"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              required
            />
          </label>

          {error ? <div className="error">{String(error)}</div> : null}

          <button className="btn btnPrimary" type="submit" disabled={loading}>
            {loading
              ? "Chargement..."
              : mode === "login"
                ? "Se connecter"
                : "Créer le compte"}
          </button>

          <div className="muted" style={{ fontSize: 13 }}>
            Astuce : utilise un email “test@benia.com” et un mot de passe
            simple, juste pour valider le flux.
          </div>
        </form>
      </div>
    </div>
  );
}
