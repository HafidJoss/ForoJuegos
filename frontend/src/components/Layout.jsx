import { NavLink, useNavigate } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { useAuth } from '../context/AuthContext'
import { listNotifications } from '../services/notificationService'
import './Layout.css'

/**
 * Componente de Diseño Global (Layout)
 * 
 * Renderiza el encabezado sticky glassmorphic con estética premium,
 * navegación con iconos enriquecidos, perfil de usuario circular,
 * y contador de notificaciones animado por pulso.
 */
function Layout({ children }) {
  const { isAuthenticated, user, logout } = useAuth()
  const [notificationCount, setNotificationCount] = useState(0)
  const navigate = useNavigate()

  useEffect(() => {
    const loadNotifications = async () => {
      if (!isAuthenticated) {
        setNotificationCount(0)
        return
      }
      try {
        const response = await listNotifications(localStorage.getItem('cg_token'))
        const count = (response || []).filter((item) => !item.leida).length
        setNotificationCount(count)
      } catch {
        setNotificationCount(0)
      }
    }

    loadNotifications()
  }, [isAuthenticated])

  return (
    <div className="layout">
      {/* Encabezado Sticky Glassmorphic Premium */}
      <header className="header">
        <div className="container header-content">
          {/* Marca / Logo */}
          <div className="brand" onClick={() => navigate('/')}>
            <span className="brand-title">
              Comunidad<span className="brand-neon">Gamer</span>
            </span>
          </div>

          {/* Menú de Navegación */}
          <nav className="nav">
            <NavLink to="/" end>
              Inicio
            </NavLink>
            <NavLink to="/games">
              Juegos
            </NavLink>
            <NavLink to="/ugc-upload">
              UGC Hub
            </NavLink>
            <NavLink to="/profile">
              Perfil
            </NavLink>
            <NavLink to="/notifications" className="notifications-nav-link">
              Notificaciones
              {notificationCount > 0 && (
                <span className="nav-badge-pulse">{notificationCount}</span>
              )}
            </NavLink>
            {user?.roles?.some((role) => role === 'Admin' || role === 'Moderador') && (
              <NavLink to="/moderation">
                Moderación
              </NavLink>
            )}
          </nav>

          {/* Área de Acciones de Usuario / Login */}
          <div className="actions">
            {isAuthenticated ? (
              <div className="user-action-group">
                <div className="user-profile-pill" onClick={() => navigate('/profile')}>
                  <div className="avatar-circle">
                    {user?.username ? user.username.substring(0, 2).toUpperCase() : 'U'}
                  </div>
                  <span className="username-text">{user?.username || user?.email}</span>
                </div>
                <button className="btn-logout-neon" type="button" onClick={logout}>
                  Salir
                </button>
              </div>
            ) : (
              <div className="auth-action-group">
                <NavLink className="btn-login-ghost" to="/auth">
                  Ingresar
                </NavLink>
                <NavLink className="btn-register-glow" to="/auth">
                  Registrarse
                </NavLink>
              </div>
            )}
          </div>
        </div>
      </header>

      {/* Área Principal de Contenido */}
      <main className="main">
        <div className="container">{children}</div>
      </main>

      {/* Pie de Página */}
      <footer className="footer">
        <div className="container footer-content">
          <span>© 2026 Comunidad Gamer</span>
          <span>Contenido legal y seguro</span>
        </div>
      </footer>
    </div>
  )
}

export default Layout
