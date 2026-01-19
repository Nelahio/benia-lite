import { useState } from "react";
import { getToken } from "./api/http";
import AuthPage from "./pages/AuthPage";
import TodayPage from "./pages/TodayPage";

export default function App() {
  const [authed, setAuthed] = useState(!!getToken());

  if (!authed) {
    return <AuthPage onAuthed={() => setAuthed(true)} />;
  }

  return <TodayPage onLogout={() => setAuthed(false)} />;
}
