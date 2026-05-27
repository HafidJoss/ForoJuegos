import { useEffect, useState } from 'react'
import Card from '../components/Card'
import Button from '../components/Button'
import { listNotifications, markAllRead, markNotificationRead } from '../services/notificationService'
import { useAuth } from '../context/AuthContext'

function Notifications() {
  const { token } = useAuth()
  const [items, setItems] = useState([])
  const [state, setState] = useState({ loading: true, error: '' })

  const loadNotifications = async () => {
    try {
      setState({ loading: true, error: '' })
      const response = await listNotifications(token)
      setItems(response || [])
    } catch {
      setState({ loading: false, error: 'No se pudieron cargar las notificaciones.' })
      return
    }
    setState({ loading: false, error: '' })
  }

  useEffect(() => {
    loadNotifications()
  }, [])

  const handleMarkRead = async (id) => {
    await markNotificationRead(id, token)
    loadNotifications()
  }

  const handleMarkAll = async () => {
    await markAllRead(token)
    loadNotifications()
  }

  return (
    <div className="page">
      <Card>
        <div className="card-header">
          <h2>Notificaciones</h2>
          <Button variant="ghost" onClick={handleMarkAll}>
            Marcar todas como leídas
          </Button>
        </div>

        {state.loading && <p className="muted">Cargando...</p>}
        {state.error && <p className="muted">{state.error}</p>}
        {!state.loading && !state.error && items.length === 0 && (
          <p className="muted">No hay notificaciones.</p>
        )}

        <div className="stack">
          {items.map((notification) => (
            <div key={notification.id} className="list-item">
              <div>
                <strong>{notification.mensaje}</strong>
                <p className="muted">{notification.tipo}</p>
              </div>
              {!notification.leida && (
                <Button variant="secondary" onClick={() => handleMarkRead(notification.id)}>
                  Marcar leída
                </Button>
              )}
            </div>
          ))}
        </div>
      </Card>
    </div>
  )
}

export default Notifications
