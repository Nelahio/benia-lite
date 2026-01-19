import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import RequireAuth from "./auth/RequireAuth";
import AppLayout from "./layout/AppLayout";

import LoginPage from "./pages/LoginPage";
import TodayPage from "./pages/TodayPage";
import MealPrepPage from "./pages/MealPrepPage";
import SymptomsPage from "./pages/SymptomsPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route
          path="/"
          element={
            <RequireAuth>
              <AppLayout />
            </RequireAuth>
          }
        >
          <Route index element={<TodayPage />} />
          <Route path="meal-prep" element={<MealPrepPage />} />
          <Route path="symptoms" element={<SymptomsPage />} />
        </Route>

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
