import { useAuth } from './hooks/useAuth'
import { Routes, Route } from 'react-router-dom'
import { PrivateRoute } from './components/PrivateRoute'
import HomePage from './pages/HomePage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import AdminPage from './pages/AdminPage'
import Navbar from './components/Navbar'
import { Role } from './types/roles'

function App() {
  const { user, logout } = useAuth()

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar user={user} onLogout={logout} />
      
      <div className="container mx-auto px-4 py-8">
        <Routes>
          {/* Public routes */}
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* Protected routes */}
          <Route
            path="/admin"
            element={
              <PrivateRoute roles={[Role.Admin]}>
                <AdminPage />
              </PrivateRoute>
            }
          />

          {/* Authenticated-only routes */}
          <Route
            path="/profile"
            element={
              <PrivateRoute>
                <div>User Profile Page</div>
              </PrivateRoute>
            }
          />
        </Routes>
      </div>
    </div>
  )
}

export default App