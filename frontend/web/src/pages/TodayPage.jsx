import { useEffect, useState } from "react";
import { api, clearToken } from "../api/http";

export default function TodayPage({ onLogout }) {
  const [me, setMe] = useState(null);
  const [routines, setRoutines] = useState([]);
  const [error, setError] = useState("");

  async function load() {
    setError("");
    try {
      const meRes = await api.auth.me();
      setMe(meRes);

      const data = await api.routines.today();
      setRoutines(data);
    } catch (err) {
      setError(err.message || "Erreur");
    }
  }

  useEffect(() => {
    (async () => {
      await load();
    })();
  }, []);

  async function toggleComplete(routine) {
    try {
      if (routine.isCompletedToday) {
        await api.routines.undoToday(routine.id);
      } else {
        await api.routines.complete(routine.id, null);
      }
      await load();
    } catch (err) {
      alert(err.message || "Erreur");
    }
  }

  function logout() {
    clearToken();
    onLogout();
  }

  return (
    <div
      style={{ maxWidth: 900, margin: "40px auto", fontFamily: "system-ui" }}
    >
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <h1>Aujourd’hui</h1>
        <button onClick={logout}>Logout</button>
      </div>

      {me ? <div style={{ color: "#666" }}>Connectée : {me.email}</div> : null}
      {error ? (
        <div style={{ color: "crimson", marginTop: 10 }}>{String(error)}</div>
      ) : null}

      <div style={{ display: "grid", gap: 10, marginTop: 20 }}>
        {routines.map((r) => (
          <div
            key={r.id}
            style={{ border: "1px solid #ddd", borderRadius: 8, padding: 12 }}
          >
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <div>
                <div style={{ fontWeight: 700 }}>{r.name}</div>
                <div style={{ fontSize: 13, color: "#666" }}>{r.category}</div>
              </div>

              <button onClick={() => toggleComplete(r)}>
                {r.isCompletedToday ? "Annuler" : "Compléter"}
              </button>
            </div>

            {r.steps?.length ? (
              <ul style={{ marginTop: 10 }}>
                {r.steps
                  .slice()
                  .sort((a, b) => a.sortOrder - b.sortOrder)
                  .map((s) => (
                    <li key={s.id}>
                      <b>{s.title}</b>
                      {s.notes ? (
                        <span style={{ color: "#666" }}> — {s.notes}</span>
                      ) : null}
                    </li>
                  ))}
              </ul>
            ) : (
              <div style={{ marginTop: 10, color: "#999" }}>Aucune étape</div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
