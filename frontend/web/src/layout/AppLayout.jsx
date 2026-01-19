import { NavLink, Outlet } from "react-router-dom";
import { clearToken } from "../api/http";

const linkStyle = ({ isActive }) => ({
  padding: "8px 10px",
  borderRadius: 8,
  textDecoration: "none",
  color: isActive ? "white" : "#222",
  background: isActive ? "#222" : "transparent",
});

export default function AppLayout() {
  function logout() {
    clearToken();
    window.location.href = "/login";
  }

  return (
    <div style={{ fontFamily: "system-ui" }}>
      <header style={{ borderBottom: "1px solid #eee" }}>
        <div
          style={{
            maxWidth: 1000,
            margin: "0 auto",
            padding: "14px 12px",
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <div style={{ fontWeight: 800 }}>Benia Lite</div>
          <nav style={{ display: "flex", gap: 6, alignItems: "center" }}>
            <NavLink to="/" style={linkStyle} end>
              Today
            </NavLink>
            <NavLink to="/meal-prep" style={linkStyle}>
              Meal prep
            </NavLink>
            <NavLink to="/symptoms" style={linkStyle}>
              Symptoms
            </NavLink>
            <button onClick={logout} style={{ marginLeft: 10 }}>
              Logout
            </button>
          </nav>
        </div>
      </header>

      <main>
        <div style={{ maxWidth: 1000, margin: "0 auto", padding: "18px 12px" }}>
          <Outlet />
        </div>
      </main>
    </div>
  );
}
