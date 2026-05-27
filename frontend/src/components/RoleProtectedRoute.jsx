import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function RoleProtectedRoute({ allowedRoles = [], children }) {
  const { isAuthenticated, user } = useAuth()
  const location = useLocation()

  if (!isAuthenticated) {
    return <Navigate to="/auth" replace state={{ from: location.pathname }} />
  }

  const roles = user?.roles || []
  const allowed = allowedRoles.some((role) => roles.includes(role))

  if (!allowed) {
    return <Navigate to="/" replace />
  }

  return children
}

export default RoleProtectedRoute
