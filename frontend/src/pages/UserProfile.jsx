import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Card from '../components/Card'
import Badge from '../components/Badge'
import Input from '../components/Input'
import Button from '../components/Button'
import { useAuth } from '../context/AuthContext'
import { updateProfile } from '../services/authService'
import { listReviewsByUser } from '../services/reviewService'
import { listUgcByUser } from '../services/ugcService'

function UserProfile() {
  const { user, token, setUser } = useAuth()
  const navigate = useNavigate()
  const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'
  const [reviews, setReviews] = useState([])
  const [ugc, setUgc] = useState([])
  const [loading, setLoading] = useState(true)

  // Estados de edición del Perfil
  const [editing, setEditing] = useState(false)
  const [profileForm, setProfileForm] = useState({
    nombre: user?.nombre || '',
    bio: user?.bio || '',
  })
  const [saveState, setSaveState] = useState({ loading: false, error: '', success: '' })

  useEffect(() => {
    if (user) {
      setProfileForm({
        nombre: user.nombre || '',
        bio: user.bio || '',
      })
      loadUserData()
    }
  }, [user])

  const loadUserData = async () => {
    if (!user) return
    try {
      setLoading(true)
      const [reviewsData, ugcData] = await Promise.all([
        listReviewsByUser(user.id),
        listUgcByUser(user.id),
      ])
      setReviews(reviewsData || [])
      setUgc(ugcData || [])
    } catch (error) {
      console.error('Error al cargar datos del perfil:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleProfileSubmit = async (e) => {
    e.preventDefault()
    if (!profileForm.nombre.trim()) {
      setSaveState({ loading: false, error: 'El nombre es obligatorio.', success: '' })
      return
    }

    try {
      setSaveState({ loading: true, error: '', success: '' })
      const updated = await updateProfile(
        {
          nombre: profileForm.nombre.trim(),
          bio: profileForm.bio.trim(),
        },
        token,
      )

      setUser((prev) => ({
        ...prev,
        nombre: updated.nombre,
        bio: updated.bio,
      }))

      setSaveState({ loading: false, error: '', success: 'Perfil actualizado con éxito.' })
      setEditing(false)
    } catch (err) {
      console.error('Error al guardar el perfil:', err)
      setSaveState({
        loading: false,
        error: err.message || 'Error al guardar cambios. Intenta de nuevo.',
        success: '',
      })
    }
  }

  const renderStars = (rating) => {
    const stars = []
    for (let i = 1; i <= 5; i++) {
      stars.push(
        <span
          key={i}
          className={`star ${i <= rating ? 'star-filled' : 'star-empty'}`}
          style={{ fontSize: '0.85rem' }}
        >
          ★
        </span>,
      )
    }
    return <div className="star-rating" style={{ display: 'inline-flex', gap: '2px' }}>{stars}</div>
  }

  if (!user) {
    return (
      <div className="page" style={{ textAlign: 'center', paddingTop: '100px' }}>
        <p className="muted">Por favor, inicia sesión para ver tu perfil.</p>
      </div>
    )
  }

  return (
    <div className="page profile-page-container">
      {/* Cabecera del Perfil con degradado y cristal */}
      <Card style={{ padding: '32px', borderRadius: '16px', border: '1px solid var(--border)', background: 'linear-gradient(135deg, rgba(23, 26, 34, 0.95) 0%, rgba(13, 14, 18, 0.95) 100%)', backdropFilter: 'blur(10px)', marginBottom: '32px' }}>
        <div className="profile-header-premium" style={{ display: 'flex', flexWrap: 'wrap', gap: '24px', alignItems: 'center', justifyContent: 'space-between' }}>
          <div style={{ display: 'flex', gap: '20px', alignItems: 'center', flexWrap: 'wrap' }}>
            <div className="profile-avatar-large" style={{ width: '80px', height: '80px', borderRadius: '50%', background: 'linear-gradient(135deg, var(--primary) 0%, var(--secondary) 100%)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '2rem', fontWeight: 'bold', color: '#fff', boxShadow: '0 8px 24px rgba(0, 210, 211, 0.3)' }}>
              {user.nombre ? user.nombre.charAt(0).toUpperCase() : user.username.charAt(0).toUpperCase()}
            </div>
            <div>
              <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                <h2 style={{ margin: 0, fontSize: '1.8rem', fontWeight: 700, letterSpacing: '-0.5px' }}>{user.nombre || 'Sin Nombre'}</h2>
                <Badge variant="success">@{user.username}</Badge>
              </div>
              <p className="muted" style={{ margin: '4px 0 0 0', fontSize: '0.9rem', color: 'var(--text-secondary)' }}>
                {user.email} · Registrado el {new Date(user.fechaRegistro || Date.now()).toLocaleDateString()}
              </p>
            </div>
          </div>

          <div style={{ display: 'flex', gap: '12px' }}>
            <Button variant={editing ? 'ghost' : 'primary'} onClick={() => { setEditing(!editing); setSaveState({ loading: false, error: '', success: '' }) }}>
              {editing ? 'Cancelar' : 'Editar Perfil'}
            </Button>
            {user.roles && user.roles.some(r => r === 'Admin' || r === 'Moderador') && (
              <Button variant="ghost" onClick={() => navigate('/moderation')}>
                Panel de Moderación
              </Button>
            )}
          </div>
        </div>

        {/* Biografía / Editor de perfil */}
        {!editing ? (
          <div style={{ marginTop: '24px', borderTop: '1px solid rgba(255,255,255,0.05)', paddingTop: '20px' }}>
            <h4 style={{ margin: '0 0 8px 0', fontSize: '0.85rem', textTransform: 'uppercase', letterSpacing: '1px', color: 'var(--primary)' }}>Sobre Mí</h4>
            <p style={{ margin: 0, lineHeight: 1.6, color: 'var(--text-secondary)', fontSize: '0.95rem' }}>
              {user.bio || 'Este usuario aún no ha escrito su biografía.'}
            </p>
          </div>
        ) : (
          <form onSubmit={handleProfileSubmit} style={{ marginTop: '24px', borderTop: '1px solid rgba(255,255,255,0.05)', paddingTop: '20px', display: 'flex', flexDirection: 'column', gap: '16px' }}>
            <h4 style={{ margin: 0, fontSize: '1rem', fontWeight: 600 }}>Editar Información del Perfil</h4>
            
            <div className="grid two" style={{ gap: '16px' }}>
              <div className="form-group">
                <label htmlFor="edit-nombre" style={{ fontSize: '0.85rem', display: 'block', marginBottom: '6px' }}>Nombre Público</label>
                <Input
                  id="edit-nombre"
                  value={profileForm.nombre}
                  onChange={(e) => setProfileForm(prev => ({ ...prev, nombre: e.target.value }))}
                  placeholder="Tu nombre público..."
                  maxLength="100"
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="edit-bio" style={{ fontSize: '0.85rem', display: 'block', marginBottom: '6px' }}>Biografía corta</label>
                <Input
                  id="edit-bio"
                  value={profileForm.bio}
                  onChange={(e) => setProfileForm(prev => ({ ...prev, bio: e.target.value }))}
                  placeholder="Cuéntanos sobre ti, tus juegos favoritos..."
                  maxLength="500"
                />
              </div>
            </div>

            {saveState.error && <p style={{ color: 'var(--error)', margin: 0, fontSize: '0.85rem' }}>{saveState.error}</p>}
            {saveState.success && <p style={{ color: 'var(--success)', margin: 0, fontSize: '0.85rem' }}>{saveState.success}</p>}

            <Button type="submit" disabled={saveState.loading} style={{ alignSelf: 'flex-start' }}>
              {saveState.loading ? 'Guardando...' : 'Guardar Cambios'}
            </Button>
          </form>
        )}

        {/* Tarjetas rápidas de estadísticas */}
        <div className="profile-stats-grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '16px', marginTop: '24px', borderTop: '1px solid rgba(255,255,255,0.05)', paddingTop: '20px' }}>
          <div className="stat-card" style={{ background: 'rgba(255,255,255,0.02)', padding: '16px', borderRadius: '8px', border: '1px solid rgba(255,255,255,0.03)', textAlign: 'center' }}>
            <span style={{ fontSize: '1.8rem', fontWeight: 700, color: 'var(--primary)', display: 'block' }}>{reviews.length}</span>
            <span className="muted" style={{ fontSize: '0.8rem', textTransform: 'uppercase' }}>Reseñas Escritas</span>
          </div>
          <div className="stat-card" style={{ background: 'rgba(255,255,255,0.02)', padding: '16px', borderRadius: '8px', border: '1px solid rgba(255,255,255,0.03)', textAlign: 'center' }}>
            <span style={{ fontSize: '1.8rem', fontWeight: 700, color: 'var(--secondary)', display: 'block' }}>{ugc.length}</span>
            <span className="muted" style={{ fontSize: '0.8rem', textTransform: 'uppercase' }}>Aportes UGC</span>
          </div>
          <div className="stat-card" style={{ background: 'rgba(255,255,255,0.02)', padding: '16px', borderRadius: '8px', border: '1px solid rgba(255,255,255,0.03)', textAlign: 'center' }}>
            <span style={{ fontSize: '1.8rem', fontWeight: 700, color: '#ffb900', display: 'block' }}>
              {reviews.length > 0 ? (reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length).toFixed(1) : '0.0'}
            </span>
            <span className="muted" style={{ fontSize: '0.8rem', textTransform: 'uppercase' }}>Calificación Promedio</span>
          </div>
        </div>
      </Card>

      {/* Grid inferior de Contenidos: Reseñas vs UGC */}
      <section className="grid two" style={{ gap: '32px' }}>
        {/* Columna Izquierda: Reseñas del Usuario */}
        <Card style={{ padding: '24px', border: '1px solid var(--border)' }}>
          <h3 style={{ margin: '0 0 20px 0', fontSize: '1.25rem', fontWeight: 600, borderBottom: '1px solid rgba(255,255,255,0.05)', paddingBottom: '12px' }}>
            Mis Reseñas Recientes
          </h3>

          {loading ? (
            <p className="muted">Cargando reseñas...</p>
          ) : reviews.length === 0 ? (
            <p className="muted" style={{ fontSize: '0.9rem' }}>Aún no has escrito ninguna reseña de videojuego.</p>
          ) : (
            <div className="stack" style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              {reviews.map((review) => (
                <div key={review.id} className="list-item" style={{ background: 'var(--surface-2)', padding: '16px', borderRadius: '8px', border: '1px solid var(--border)', display: 'flex', flexDirection: 'column', gap: '8px' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <strong
                      style={{ color: 'var(--primary)', cursor: 'pointer', fontSize: '0.95rem' }}
                      onClick={() => navigate(`/games/${review.juegoId}`)}
                    >
                      {review.juegoNombre || 'Videojuego'}
                    </strong>
                    {renderStars(review.rating)}
                  </div>
                  <p style={{ margin: 0, fontSize: '0.9rem', lineHeight: 1.5, color: 'var(--text-secondary)' }}>
                    "{review.texto}"
                  </p>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '4px', fontSize: '0.75rem', color: 'var(--text-muted)' }}>
                    <span className="muted">Publicado el {new Date(review.fechaCreacion).toLocaleDateString()}</span>
                    <span>Likes ({review.likes})</span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </Card>

        {/* Columna Derecha: UGC del Usuario */}
        <Card style={{ padding: '24px', border: '1px solid var(--border)' }}>
          <h3 style={{ margin: '0 0 20px 0', fontSize: '1.25rem', fontWeight: 600, borderBottom: '1px solid rgba(255,255,255,0.05)', paddingBottom: '12px' }}>
            Aportes UGC Publicados
          </h3>

          {loading ? (
            <p className="muted">Cargando publicaciones UGC...</p>
          ) : ugc.length === 0 ? (
            <p className="muted" style={{ fontSize: '0.9rem' }}>Aún no has subido contenido de mods o guías.</p>
          ) : (
            <div className="stack" style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              {ugc.map((item) => (
                <div key={item.id} className="list-item" style={{ background: 'var(--surface-2)', padding: '16px', borderRadius: '8px', border: '1px solid var(--border)', display: 'flex', flexDirection: 'column', gap: '8px' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <strong style={{ fontSize: '0.95rem', color: '#fff' }}>{item.titulo}</strong>
                    <Badge variant="warning">UGC</Badge>
                  </div>
                  {item.descripcion && (
                    <p style={{ margin: 0, fontSize: '0.85rem', lineHeight: 1.4, color: 'var(--text-secondary)' }}>
                      {item.descripcion}
                    </p>
                  )}
                  
                  {/* Info de Archivo descargable */}
                  <div style={{ background: 'rgba(0,0,0,0.2)', padding: '8px 12px', borderRadius: '4px', display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '4px', fontSize: '0.8rem' }}>
                    <span className="muted" style={{ display: 'inline-flex', alignItems: 'center', gap: '4px' }}>
                      {item.archivoNombre}
                    </span>
                    <a 
                      href={`${baseUrl}/ugc/download/${item.id}`} 
                      download 
                      target="_blank" 
                      rel="noreferrer" 
                      style={{ color: 'var(--secondary)', background: 'transparent', border: 'none', textDecoration: 'none', cursor: 'pointer', fontWeight: 600 }}
                    >
                      Descargar
                    </a>
                  </div>
                  
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '4px', fontSize: '0.75rem', color: 'var(--text-muted)' }}>
                    <span>Juego: {item.juegoNombre || 'Comunidad'}</span>
                    <span className="muted">Subido {new Date(item.fechaSubida).toLocaleDateString()}</span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </Card>
      </section>
    </div>
  )
}

export default UserProfile
