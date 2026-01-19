import { Navigate } from "react-router-dom";
import { getToken } from "../api/http";

export default function RequireAuth({ children }) {
  const token = getToken();
  if (!token) return <Navigate to="/login" replace />;
  return children;
}
