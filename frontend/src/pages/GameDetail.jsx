import { useEffect, useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'
import Card from '../components/Card'
import Badge from '../components/Badge'
import Tabs from '../components/Tabs'
import Input from '../components/Input'
import { getGameById } from '../services/gameService'
import { createReview, deleteReview, listReviewsByGame, updateReview } from '../services/reviewService'
import { listUgcByGame } from '../services/ugcService'
import { useAuth } from '../context/AuthContext'
import { createComment, deleteComment, listCommentsByReview } from '../services/commentService'
import { likeReview, unlikeReview } from '../services/likeService'
import { createReport } from '../services/reportService'
import '../styles/GameDetail.css'

function GameDetail() {
  const { id } = useParams()
  const { user, token, isAuthenticated } = useAuth()
  const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'
  const [game, setGame] = useState(null)
  const [reviews, setReviews] = useState([])
  const [ugc, setUgc] = useState([])
  const [commentsByReview, setCommentsByReview] = useState({})
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [form, setForm] = useState({ rating: 5, texto: '' })
  const [actionState, setActionState] = useState({ loading: false, error: '', success: '' })
  const [editingId, setEditingId] = useState(null)
  const [commentForm, setCommentForm] = useState({})
  const [likeState, setLikeState] = useState({})
  const [reportForm, setReportForm] = useState({ motivo: '', evidencia: '' })
  const [reportTarget, setReportTarget] = useState(null)

  useEffect(() => {
    const loadData = async () => {
      if (!id) return
      try {
        setLoading(true)
        setError('')
        const [gameData, reviewsData, ugcData] = await Promise.all([
          getGameById(id),
          listReviewsByGame(id),
          listUgcByGame(id),
        ])

        setGame(gameData)
        const reviewItems = reviewsData || []
        setReviews(reviewItems)
        setUgc(ugcData || [])

        const commentsEntries = await Promise.all(
          reviewItems.map(async (review) => {
            const comments = await listCommentsByReview(review.id)
            return [review.id, comments || []]
          }),
        )
        setCommentsByReview(Object.fromEntries(commentsEntries))
      } catch (error) {
        setError(error.message || 'Error al cargar detalle del juego')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [id])

  const handleReport = async () => {
    if (!isAuthenticated || !reportTarget) {
      setActionState({ loading: false, error: 'Inicia sesión para reportar.', success: '' })
      return
    }
    if (!reportForm.motivo.trim()) {
      setActionState({ loading: false, error: 'El motivo es obligatorio.', success: '' })
      return
    }
    try {
      await createReport(
        {
          tipoObjetivo: reportTarget.tipoObjetivo,
          objetivoId: reportTarget.objetivoId,
          motivo: reportForm.motivo,
          evidencia: reportForm.evidencia || null,
        },
        token,
      )
      setReportForm({ motivo: '', evidencia: '' })
      setReportTarget(null)
      setActionState({ loading: false, error: '', success: 'Reporte enviado.' })
    } catch {
      setActionState({ loading: false, error: 'No se pudo enviar el reporte.', success: '' })
    }
  }

  const handleLike = async (reviewId) => {
    if (!isAuthenticated) {
      setActionState({ loading: false, error: 'Inicia sesión para dar like.', success: '' })
      return
    }
    try {
      const liked = likeState[reviewId]
      if (liked) {
        await unlikeReview(reviewId, token)
      } else {
        await likeReview(reviewId, token)
      }
      const updated = await listReviewsByGame(game.id)
      setReviews(updated || [])
      setLikeState((prev) => ({ ...prev, [reviewId]: !liked }))
    } catch {
      setActionState({ loading: false, error: 'No se pudo actualizar el like.', success: '' })
    }
  }

  const handleComment = async (reviewId) => {
    if (!isAuthenticated) {
      setActionState({ loading: false, error: 'Inicia sesión para comentar.', success: '' })
      return
    }
    const texto = commentForm[reviewId]
    if (!texto || !texto.trim()) {
      setActionState({ loading: false, error: 'El comentario es obligatorio.', success: '' })
      return
    }

    const originalText = texto
    const tempId = `temp-${Date.now()}`

    // Crear el comentario optimista instantáneo
    const optimisticComment = {
      id: tempId,
      resenaId: reviewId,
      usuarioId: user.id,
      usuarioNombre: user.username || user.email || 'Yo',
      texto: texto.trim(),
      fechaCreacion: new Date().toISOString()
    }

    // Actualizar el estado local inmediatamente
    setCommentsByReview((prev) => ({
      ...prev,
      [reviewId]: [optimisticComment, ...(prev[reviewId] || [])]
    }))

    // Limpiar el campo de texto inmediatamente
    setCommentForm((prev) => ({ ...prev, [reviewId]: '' }))

    try {
      await createComment({ resenaId: reviewId, texto: originalText }, token)
      const updated = await listCommentsByReview(reviewId)
      setCommentsByReview((prev) => ({ ...prev, [reviewId]: updated || [] }))
    } catch (err) {
      console.error('Error al enviar comentario:', err)
      // Revertir estado si falla la petición
      setCommentsByReview((prev) => ({
        ...prev,
        [reviewId]: (prev[reviewId] || []).filter(c => c.id !== tempId)
      }))
      setCommentForm((prev) => ({ ...prev, [reviewId]: originalText }))
      setActionState({ loading: false, error: 'No se pudo publicar el comentario.', success: '' })
    }
  }

  const handleDeleteComment = async (reviewId, commentId) => {
    try {
      await deleteComment(commentId, token)
      const updated = await listCommentsByReview(reviewId)
      setCommentsByReview((prev) => ({ ...prev, [reviewId]: updated || [] }))
    } catch {
      setActionState({ loading: false, error: 'No se pudo eliminar el comentario.', success: '' })
    }
  }

  const userReviews = useMemo(
    () => (user ? reviews.filter((review) => review.usuarioId === user.id) : []),
    [reviews, user],
  )

  const handleSubmit = async () => {
    if (!isAuthenticated) {
      setActionState({ loading: false, error: 'Inicia sesión para reseñar.', success: '' })
      return
    }
    if (!form.texto.trim()) {
      setActionState({ loading: false, error: 'El texto es obligatorio.', success: '' })
      return
    }

    try {
      setActionState({ loading: true, error: '', success: '' })
      if (editingId) {
        await updateReview(editingId, { rating: Number(form.rating), texto: form.texto }, token)
      } else {
        await createReview({ juegoId: game.id, rating: Number(form.rating), texto: form.texto }, token)
      }
      const updated = await listReviewsByGame(game.id)
      setReviews(updated || [])
      setForm({ rating: 5, texto: '' })
      setEditingId(null)
      setActionState({ loading: false, error: '', success: 'Reseña guardada con éxito.' })
    } catch (err) {
      console.error('Error al guardar la reseña:', err)
      setActionState({ loading: false, error: err.message || 'No se pudo guardar la reseña.', success: '' })
    }
  }

  const handleEdit = (review) => {
    setEditingId(review.id)
    setForm({ rating: review.rating, texto: review.texto })
  }

  const handleDelete = async (reviewId) => {
    try {
      setActionState({ loading: true, error: '', success: '' })
      await deleteReview(reviewId, token)
      const updated = await listReviewsByGame(game.id)
      setReviews(updated || [])
      setActionState({ loading: false, error: '', success: 'Reseña eliminada.' })
    } catch {
      setActionState({ loading: false, error: 'No se pudo eliminar la reseña.', success: '' })
    }
  }

  const renderStars = (rating) => {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      stars.push(
        <span key={i} className={`star ${i <= rating ? 'star-filled' : 'star-empty'}`} style={{ fontSize: '0.9rem' }}>★</span>
      );
    }
    return <div className="star-rating" style={{ display: 'inline-flex', gap: '2px' }}>{stars}</div>;
  };

  const renderInteractiveStars = () => {
    const stars = []
    for (let i = 1; i <= 5; i++) {
      const isActive = i <= form.rating
      stars.push(
        <button
          key={i}
          type="button"
          className={`interactive-star ${isActive ? 'active' : ''}`}
          onClick={() => setForm(prev => ({ ...prev, rating: i }))}
          title={`Calificar con ${i} estrellas`}
        >
          ★
        </button>
      )
    }
    return (
      <div className="star-selector-container">
        <span className="star-selector-label">Calificación:</span>
        <div className="interactive-star-group">
          {stars}
        </div>
      </div>
    )
  }

  if (loading) {
    return (
      <div className="page">
        <p className="muted">Cargando detalle...</p>
      </div>
    )
  }

  if (error) {
    return (
      <div className="page">
        <p className="muted">{error}</p>
      </div>
    )
  }

  if (!game) {
    return (
      <div className="page">
        <p className="muted">Juego no encontrado.</p>
      </div>
    )
  }

  return (
    <div className="page">
      <div className="game-detail-hero">
        <div 
          className="game-hero-bg" 
          style={{ backgroundImage: game.imagenPortadaUrl ? `url(${game.imagenPortadaUrl})` : 'none' }}
        />
        <div className="game-hero-overlay" />
        <div className="game-hero-content">
          {game.imagenPortadaUrl && (
            <img 
              src={game.imagenPortadaUrl} 
              alt={game.nombre} 
              className="game-hero-cover"
              onError={(e) => {
                e.target.onerror = null;
                e.target.style.display = 'none';
              }}
            />
          )}
          <div className="game-hero-info">
            <h1 className="game-hero-title">{game.nombre}</h1>
            <div className="game-hero-meta">
              {game.generoPrincipal && (
                <span className="game-hero-badge">{game.generoPrincipal}</span>
              )}
              {game.plataforma && (
                <span className="game-hero-badge" style={{ background: 'rgba(0, 210, 211, 0.2)', borderColor: 'rgba(0, 210, 211, 0.4)', color: 'var(--secondary)' }}>
                  {game.plataforma}
                </span>
              )}
              {game.fechaLanzamiento && (
                <span className="muted text-sm" style={{ color: '#fff' }}>
                  Lanzado el: {new Date(game.fechaLanzamiento).toLocaleDateString()}
                </span>
              )}
            </div>
          </div>
        </div>
      </div>

      <Tabs
        items={[
          { value: 'reviews', label: 'Reseñas' },
          { value: 'ugc', label: 'UGC' },
          { value: 'info', label: 'Información' },
        ]}
        active="reviews"
      />

      <section className="grid two">
        <div>
          <Card>
            <h2>Reseñas de la comunidad</h2>
            {isAuthenticated && (
              <div className="card review-form-card" style={{ padding: '20px', borderRadius: '12px', marginBottom: '24px', border: '1px solid var(--border)' }}>
                <div className="review-form-title-group">
                  {game.imagenPortadaUrl && (
                    <img src={game.imagenPortadaUrl} alt="Mini portada" className="review-form-mini-cover" />
                  )}
                  <div>
                    <h3 style={{ margin: 0, fontSize: '1rem', fontWeight: 600 }}>
                      {editingId ? 'Editar tu reseña' : `Escribe una reseña para ${game.nombre}`}
                    </h3>
                    <p className="muted" style={{ margin: 0, fontSize: '0.8rem' }}>Comparte tu opinión con la comunidad gamer</p>
                  </div>
                </div>

                <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
                  {renderInteractiveStars()}

                  <div className="review-textarea-wrapper">
                    <textarea
                      rows="4"
                      className="review-textarea"
                      placeholder="Cuéntanos más detalladamente tu experiencia con este juego... (mínimo 5 caracteres)"
                      value={form.texto}
                      maxLength="4000"
                      onChange={(event) =>
                        setForm((prev) => ({ ...prev, texto: event.target.value }))
                      }
                    />
                    <span className="char-counter">{form.texto.length}/4000</span>
                  </div>

                  {actionState.error && (
                    <p style={{ color: 'var(--error)', margin: 0, fontSize: '0.85rem', display: 'flex', alignItems: 'center', gap: '4px' }}>
                      Error: {actionState.error}
                    </p>
                  )}
                  
                  {actionState.success && (
                    <p style={{ color: 'var(--success)', margin: 0, fontSize: '0.85rem', display: 'flex', alignItems: 'center', gap: '4px' }}>
                      {actionState.success}
                    </p>
                  )}

                  <button 
                    className="btn btn-primary" 
                    type="button" 
                    onClick={handleSubmit} 
                    disabled={actionState.loading || form.texto.trim().length < 5}
                    style={{ alignSelf: 'flex-end', padding: '10px 24px' }}
                  >
                    {actionState.loading ? 'Procesando...' : editingId ? 'Actualizar reseña' : 'Publicar reseña'}
                  </button>
                </div>
              </div>
            )}

            {reviews.length === 0 ? (
              <p className="muted">No hay reseñas para este juego.</p>
            ) : (
              <div className="stack">
                {reviews.map((review) => (
                  <div key={review.id} className="list-item" style={{ background: 'var(--surface-2)', padding: '16px', borderRadius: '8px', border: '1px solid var(--border)', marginBottom: '12px', display: 'flex', flexDirection: 'column', gap: '12px' }}>
                    <div style={{ flex: 1, minWidth: 0 }}>
                      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '8px' }}>
                        <span className="muted text-xs" style={{ fontWeight: 600 }}>Usuario de NuevoForo</span>
                        {renderStars(review.rating)}
                      </div>
                      <p style={{ margin: 0, fontSize: '0.9rem', color: 'var(--text-secondary)', lineHeight: 1.5, wordBreak: 'break-word' }}>
                        {review.texto}
                      </p>
                      <small className="muted text-xs" style={{ display: 'block', marginTop: '6px' }}>
                        Publicado el {new Date(review.fechaCreacion).toLocaleDateString()}
                      </small>
                    </div>
                    <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end', borderTop: '1px solid rgba(255,255,255,0.05)', paddingTop: '8px' }}>
                      <button
                        className={`reaction-btn ${likeState[review.id] ? 'active-like' : ''}`}
                        type="button"
                        onClick={() => handleLike(review.id)}
                        style={{ padding: '4px 10px', fontSize: '0.75rem' }}
                      >
                        <span>Like ({review.likes})</span>
                      </button>
                      <button
                        className="btn btn-ghost btn-sm"
                        type="button"
                        onClick={() =>
                          setReportTarget({ tipoObjetivo: 'Resena', objetivoId: review.id })
                        }
                      >
                        Reportar
                      </button>
                      {userReviews.some((item) => item.id === review.id) && (
                        <>
                          <button className="btn btn-ghost btn-sm" type="button" onClick={() => handleEdit(review)}>
                            Editar
                          </button>
                          <button className="btn btn-ghost btn-sm" type="button" onClick={() => handleDelete(review.id)}>
                            Eliminar
                          </button>
                        </>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </Card>

          <Card style={{ marginTop: '24px' }}>
            <h2>Comentarios de las reseñas</h2>
            {reviews.length === 0 ? (
              <p className="muted">Sin reseñas para comentar.</p>
            ) : (
              <div className="stack">
                {reviews.map((review) => (
                  <div key={review.id} className="stack" style={{ background: 'var(--surface-2)', padding: '16px', borderRadius: '8px', border: '1px solid var(--border)', marginBottom: '16px' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderBottom: '1px solid rgba(255,255,255,0.05)', paddingBottom: '8px', marginBottom: '8px' }}>
                      <span className="text-xs muted" style={{ display: '-webkit-box', WebkitLineClamp: 1, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
                        Reseña: "{review.texto}"
                      </span>
                      {renderStars(review.rating)}
                    </div>
                    <div className="comments-container">
                      {(commentsByReview[review.id] || []).map((comment) => {
                        const firstLetter = comment.usuarioNombre ? comment.usuarioNombre.substring(0, 2).toUpperCase() : 'U'
                        return (
                          <div key={comment.id} className="premium-comment-card">
                            {/* Avatar del autor del comentario */}
                            <div className="comment-avatar-circle">
                              {firstLetter}
                            </div>

                            {/* Contenido principal */}
                            <div className="comment-main-content">
                              <div className="comment-header-row">
                                <span className="comment-author-name">
                                  {comment.usuarioNombre || 'Anónimo'}
                                </span>
                              </div>

                              <p className="comment-text-body">{comment.texto}</p>

                              <div className="comment-footer-row">
                                <span className="comment-date-text">
                                  {new Date(comment.fechaCreacion).toLocaleString()}
                                </span>

                                <div className="comment-actions-group">
                                  <button
                                    className="btn-comment-action"
                                    type="button"
                                    onClick={() =>
                                      setReportTarget({ tipoObjetivo: 'Comentario', objetivoId: comment.id })
                                    }
                                  >
                                    Reportar
                                  </button>
                                  {user?.id === comment.usuarioId && (
                                    <button
                                      className="btn-comment-action delete"
                                      type="button"
                                      onClick={() => handleDeleteComment(review.id, comment.id)}
                                    >
                                      Eliminar
                                    </button>
                                  )}
                                </div>
                              </div>
                            </div>
                          </div>
                        )
                      })}
                      {(commentsByReview[review.id] || []).length === 0 && (
                        <p className="muted text-xs" style={{ margin: '8px 0', textAlign: 'center' }}>
                          Sé el primero en comentar esta reseña.
                        </p>
                      )}
                    </div>
                    {isAuthenticated && (
                      <div className="premium-comment-form-box">
                        <Input
                          value={commentForm[review.id] || ''}
                          onChange={(event) =>
                            setCommentForm((prev) => ({ ...prev, [review.id]: event.target.value }))
                          }
                          placeholder="Escribe un comentario..."
                          className="ugc-comment-input"
                        />
                        <button 
                          className="btn-comment-submit-glow" 
                          type="button" 
                          onClick={() => handleComment(review.id)}
                        >
                          Comentar
                        </button>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </Card>
        </div>

        <div>
          <Card style={{ padding: '24px', border: '1px solid var(--border)' }}>
            <h2>Contenido UGC</h2>
            {ugc.length === 0 ? (
              <p className="muted">No hay contenido UGC disponible para este juego.</p>
            ) : (
              <div className="stack" style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
                {ugc.map((item) => (
                  <div key={item.id} className="ugc-card-item" style={{ background: 'var(--surface-2)', padding: '20px', borderRadius: '12px', border: '1px solid var(--border)', display: 'flex', flexDirection: 'column', gap: '12px', transition: 'all 0.3s ease', position: 'relative', overflow: 'hidden' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'wrap', gap: '10px' }}>
                      <div>
                        <strong style={{ fontSize: '1.1rem', fontWeight: 600, color: '#fff', display: 'block', marginBottom: '4px' }}>{item.titulo}</strong>
                        <span className="muted text-xs">Publicado el {new Date(item.fechaSubida).toLocaleDateString()}</span>
                      </div>
                      <Badge variant="warning">UGC</Badge>
                    </div>

                    {/* UGC Thumbnail & Description Group */}
                    <div style={{ display: 'flex', gap: '16px', flexWrap: 'wrap', alignItems: 'flex-start' }}>
                      {item.fotoPath && (
                        <div style={{ width: '120px', height: '80px', borderRadius: '8px', overflow: 'hidden', border: '1px solid rgba(255,255,255,0.05)', flexShrink: 0, backgroundColor: 'rgba(0,0,0,0.3)' }}>
                          <img 
                            src={`http://localhost:5000${item.fotoPath}`} 
                            alt={item.titulo} 
                            style={{ width: '100%', height: '100%', objectFit: 'cover' }} 
                            onError={(e) => { e.target.style.display = 'none'; }}
                          />
                        </div>
                      )}
                      
                      {item.descripcion && (
                        <p className="muted text-sm" style={{ margin: 0, lineHeight: 1.5, flex: 1, color: 'var(--text-secondary)' }}>
                          {item.descripcion}
                        </p>
                      )}
                    </div>

                    {/* Tags List */}
                    {item.tags && (
                      <div style={{ display: 'flex', flexWrap: 'wrap', gap: '6px', marginTop: '4px' }}>
                        {item.tags.split(',').map((tag) => (
                          <span key={tag} className="ugc-game-tag" style={{ margin: 0, padding: '2px 8px', fontSize: '0.75rem' }}>
                            #{tag.trim()}
                          </span>
                        ))}
                      </div>
                    )}

                    {/* Download & Actions row */}
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '8px', paddingTop: '12px', borderTop: '1px solid rgba(255,255,255,0.05)', flexWrap: 'wrap', gap: '12px' }}>
                      {item.archivoUrl ? (
                        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                          <span className="muted text-xs" style={{ display: 'inline-flex', alignItems: 'center', gap: '4px' }}>
                            {item.archivoNombre}
                          </span>
                          <a 
                            href={`${baseUrl}/ugc/download/${item.id}`} 
                            download 
                            target="_blank" 
                            rel="noreferrer" 
                            className="btn btn-secondary btn-sm" 
                            style={{ display: 'inline-block', textDecoration: 'none', background: 'linear-gradient(135deg, var(--primary) 0%, var(--secondary) 100%)', border: 'none', color: '#fff', fontWeight: 600, padding: '6px 14px', borderRadius: '6px', fontSize: '0.8rem' }}
                          >
                            Descargar
                          </a>
                        </div>
                      ) : (
                        <span className="muted text-xs">Sin archivo adjunto</span>
                      )}

                      <button
                        className="btn btn-ghost btn-sm"
                        type="button"
                        onClick={() =>
                          setReportTarget({ tipoObjetivo: 'ContenidoUgc', objetivoId: item.id })
                        }
                        style={{ padding: '6px 12px', fontSize: '0.8rem', color: 'rgba(255,255,255,0.4)', border: '1px solid rgba(255,255,255,0.08)' }}
                      >
                        Reportar
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </Card>

          {reportTarget && (
            <Card style={{ marginTop: '24px' }}>
              <h2>Reportar contenido</h2>
              <div className="stack">
                <label className="input-field">
                  <span>Motivo</span>
                  <input
                    value={reportForm.motivo}
                    onChange={(event) =>
                      setReportForm((prev) => ({ ...prev, motivo: event.target.value }))
                    }
                  />
                </label>
                <label className="input-field">
                  <span>Evidencia (opcional)</span>
                  <input
                    value={reportForm.evidencia}
                    onChange={(event) =>
                      setReportForm((prev) => ({ ...prev, evidencia: event.target.value }))
                    }
                  />
                </label>
                <div className="stack" style={{ display: 'flex', flexDirection: 'row', gap: '8px' }}>
                  <button className="btn btn-primary" type="button" onClick={handleReport}>
                    Enviar reporte
                  </button>
                  <button className="btn btn-ghost" type="button" onClick={() => setReportTarget(null)}>
                    Cancelar
                  </button>
                </div>
              </div>
            </Card>
          )}

          <Card style={{ marginTop: '24px' }}>
            <h2>Información General</h2>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '12px', fontSize: '0.9rem' }}>
              <div>
                <span className="muted" style={{ display: 'block', fontSize: '0.8rem', textTransform: 'uppercase' }}>Descripción</span>
                <p style={{ margin: 0, lineHeight: 1.5, color: 'var(--text-secondary)' }}>
                  {game.descripcion || 'Descripción detallada no disponible.'}
                </p>
              </div>
              <div style={{ display: 'flex', gap: '24px' }}>
                <div>
                  <span className="muted" style={{ display: 'block', fontSize: '0.8rem', textTransform: 'uppercase' }}>Plataforma</span>
                  <strong>{game.plataforma || 'Varias'}</strong>
                </div>
                <div>
                  <span className="muted" style={{ display: 'block', fontSize: '0.8rem', textTransform: 'uppercase' }}>Género</span>
                  <strong>{game.generoPrincipal || 'General'}</strong>
                </div>
              </div>
              {game.tags && (
                <div>
                  <span className="muted" style={{ display: 'block', fontSize: '0.8rem', textTransform: 'uppercase', marginBottom: '4px' }}>Etiquetas</span>
                  <div style={{ display: 'flex', flexWrap: 'wrap', gap: '6px' }}>
                    {game.tags.split(',').map(tag => (
                      <span key={tag} className="ugc-game-tag" style={{ margin: 0, padding: '2px 8px' }}>
                        #{tag.trim()}
                      </span>
                    ))}
                  </div>
                </div>
              )}
            </div>
          </Card>
        </div>
      </section>
    </div>
  )
}

export default GameDetail
