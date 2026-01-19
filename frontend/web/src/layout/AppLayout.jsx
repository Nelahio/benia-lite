import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { clearToken } from "../api/http";

export default function AppLayout() {
  const navigate = useNavigate();

  function logout() {
    clearToken();
    navigate("/login");
  }

  return (
    <div>
      <header className="header">
        <div className="container spaceBetween">
          <div>
            <div style={{ fontWeight: 900, letterSpacing: 0.2 }}>
              Benia Lite
            </div>
            <div className="muted" style={{ fontSize: 13 }}>
              Routines • Symptoms • Meal prep
            </div>
          </div>

          <div className="row">
            <nav className="nav row">
              <NavLink to="/" end>
                Today
              </NavLink>
              <NavLink to="/meal-prep">Meal prep</NavLink>
              <NavLink to="/symptoms">Symptoms</NavLink>
            </nav>

            <button className="btn btnGhost" onClick={logout}>
              Logout
            </button>
          </div>
        </div>
      </header>

      <main className="container">
        <Outlet />
      </main>
    </div>
  );
}
