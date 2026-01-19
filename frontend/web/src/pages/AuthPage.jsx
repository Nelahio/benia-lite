import { useState } from "react";
import { api } from "../api/http";

export default function AuthPage({ onAuthed }) {
  const [mode, setMode] = useState("login"); // login | register
  const [email, setEmail] = useState("test@benia.com");
  const [password, setPassword] = useState("Password123!");
  const [error, setError] = useState("");

  async function submit(e) {
    e.preventDefault();
    setError("");

    try {
      const res =
        mode === "login"
          ? await api.auth.login(email, password)
          : await api.auth.register(email, password);

      // res = { accessToken }
      localStorage.setItem("benia_token", res.accessToken);
      onAuthed();
    } catch (err) {
      setError(err.message || "Erreur");
    }
  }

  return (
    <div
      style={{ maxWidth: 420, margin: "60px auto", fontFamily: "system-ui" }}
    >
      <h1>Benia Lite</h1>
      <p style={{ color: "#666" }}>Connexion pour tester l’API.</p>

      <div style={{ display: "flex", gap: 8, marginBottom: 12 }}>
        <button onClick={() => setMode("login")} disabled={mode === "login"}>
          Login
        </button>
        <button
          onClick={() => setMode("register")}
          disabled={mode === "register"}
        >
          Register
        </button>
      </div>

      <form onSubmit={submit} style={{ display: "grid", gap: 10 }}>
        <label>
          Email
          <input
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            style={{ width: "100%" }}
          />
        </label>

        <label>
          Password
          <input
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            type="password"
            style={{ width: "100%" }}
          />
        </label>

        {error ? <div style={{ color: "crimson" }}>{String(error)}</div> : null}

        <button type="submit">
          {mode === "login" ? "Se connecter" : "Créer un compte"}
        </button>
      </form>
    </div>
  );
}
