import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom"

import { AppLayout } from "@/components/layout/AppLayout"
import { Toaster } from "@/components/ui/sonner"
import { AuthProvider } from "@/contexts/AuthProvider"
import { ConfiguracoesPage } from "@/pages/ConfiguracoesPage"
import { DashboardPage } from "@/pages/DashboardPage"
import { DisponibilidadePage } from "@/pages/DisponibilidadePage"
import { ServicosPage } from "@/pages/ServicosPage"
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
            element={
              <ProtectedRoute>
                <AppLayout />
              </ProtectedRoute>
            }
          >
            <Route path="/" element={<DashboardPage />} />
            <Route path="/servicos" element={<ServicosPage />} />
            <Route path="/disponibilidade" element={<DisponibilidadePage />} />
            <Route path="/configuracoes" element={<ConfiguracoesPage />} />
          </Route>
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
        <Toaster />
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
