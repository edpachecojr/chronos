import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom"

import { AuthProvider } from "@/contexts/AuthProvider"
import { DashboardPage } from "@/pages/DashboardPage"
import { LoginPage } from "@/pages/auth/LoginPage"
import { OnboardingPage } from "@/pages/auth/OnboardingPage"
import { RegisterPage } from "@/pages/auth/RegisterPage"
import { OnboardingRoute } from "@/routes/OnboardingRoute"
import { ProtectedRoute } from "@/routes/ProtectedRoute"
import { PublicOnlyRoute } from "@/routes/PublicOnlyRoute"

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route
            path="/login"
            element={
              <PublicOnlyRoute>
                <LoginPage />
              </PublicOnlyRoute>
            }
          />
          <Route
            path="/registro"
            element={
              <PublicOnlyRoute>
                <RegisterPage />
              </PublicOnlyRoute>
            }
          />
          <Route
            path="/onboarding"
            element={
              <OnboardingRoute>
                <OnboardingPage />
              </OnboardingRoute>
            }
          />
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <DashboardPage />
              </ProtectedRoute>
            }
          />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
