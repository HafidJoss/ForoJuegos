import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Input from '../components/Input'
import Badge from '../components/Badge'
import { listGames, getGenres, getTags } from '../services/gameService'

function GamesCatalog() {
  const navigate = useNavigate()
  const [filters, setFilters] = useState({ texto: '', genero: '', tags: '' })
  const [genresList, setGenresList] = useState([])
  const [tagsList, setTagsList] = useState([])
  const [data, setData] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const loadGames = async (currentFilters = filters) => {
    try {
      setLoading(true)
      setError('')
      const response = await listGames({
        texto: currentFilters.texto || undefined,
        genero: currentFilters.genero || undefined,
        tags: currentFilters.tags || undefined,
      })
      const items = response?.items ?? response
      setData(items || [])
    } catch (err) {
      setError(err.message || 'Error al cargar juegos')
    } finally {
      setLoading(false)
    }
  }

  const handleClearFilters = () => {
    const cleared = { texto: '', genero: '', tags: '' }
    setFilters(cleared)
    loadGames(cleared)
  }

  useEffect(() => {
    const fetchMetadata = async () => {
      try {
        const [gList, tList] = await Promise.all([getGenres(), getTags()])
        setGenresList(gList || [])
        setTagsList(tList || [])
      } catch (err) {
        console.error('Error al cargar géneros y tags:', err)
      }
    }
    fetchMetadata()
    loadGames()
  }, [])

  return (
    <div className="page">
      <h1>Catálogo de juegos</h1>
      <div className="filters" style={{ background: 'rgba(23, 26, 34, 0.4)', padding: '16px', borderRadius: '12px', backdropFilter: 'blur(10px)', border: '1px solid var(--border)', marginBottom: '32px' }}>
        <div className="filter-group">
          <label className="filter-label" htmlFor="filter-buscar">Buscar</label>
          <Input
            id="filter-buscar"
            placeholder="Buscar por nombre o descripción"
            value={filters.texto}
            onChange={(event) => setFilters((prev) => ({ ...prev, texto: event.target.value }))}
          />
        </div>

        <div className="filter-group">
          <label className="filter-label" htmlFor="filter-genero">Género</label>
          <select
            id="filter-genero"
            className="select-input"
            value={filters.genero}
            onChange={(event) => setFilters((prev) => ({ ...prev, genero: event.target.value }))}
            style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid var(--border)', backgroundColor: 'var(--surface-2)', color: 'var(--text-primary)' }}
          >
            <option value="">-- Todos los géneros --</option>
            {genresList.map((genero) => (
              <option key={genero} value={genero}>
                {genero}
              </option>
            ))}
          </select>
        </div>

        <div className="filter-group">
          <label className="filter-label" htmlFor="filter-tag">Etiqueta</label>
          <select
            id="filter-tag"
            className="select-input"
            value={filters.tags}
            onChange={(event) => setFilters((prev) => ({ ...prev, tags: event.target.value }))}
            style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid var(--border)', backgroundColor: 'var(--surface-2)', color: 'var(--text-primary)' }}
          >
            <option value="">-- Todas las etiquetas --</option>
            {tagsList.map((tag) => (
              <option key={tag} value={tag}>
                {tag}
              </option>
            ))}
          </select>
        </div>

        <div className="filter-actions" style={{ display: 'flex', gap: '8px', alignItems: 'flex-end' }}>
          <button className="btn btn-secondary" onClick={() => loadGames(filters)} type="button">
            Filtrar
          </button>
          <button className="btn btn-ghost" onClick={handleClearFilters} type="button">
            Limpiar
          </button>
        </div>
      </div>

      {loading && <p className="muted">Cargando juegos...</p>}
      {error && <p className="muted">{error}</p>}
      {!loading && !error && data.length === 0 && <p className="muted">No hay juegos disponibles.</p>}

      <div className="grid three" style={{ gap: '24px' }}>
        {data.map((game) => (
          <div key={game.id} className="game-card">
            <div className="game-card-banner">
              {game.imagenPortadaUrl ? (
                <img 
                  src={game.imagenPortadaUrl} 
                  alt={game.nombre} 
                  className="game-card-img"
                  onError={(e) => {
                    e.target.onerror = null;
                    e.target.style.display = 'none';
                  }}
                />
              ) : null}
              {game.generoPrincipal && (
                <Badge className="game-card-badge-overlay">
                  {game.generoPrincipal}
                </Badge>
              )}
            </div>
            <div className="game-card-body">
              <h3 className="game-card-title">{game.nombre}</h3>
              {game.plataforma && (
                <span className="game-card-platform">{game.plataforma}</span>
              )}
              <p className="game-card-desc">
                {game.descripcion || 'Descripción no disponible.'}
              </p>
              
              {game.tags && (
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '6px', marginBottom: '16px' }}>
                  {game.tags.split(',').map((tag) => (
                    <span key={tag} className="ugc-game-tag" style={{ margin: 0, padding: '2px 8px', fontSize: '0.7rem' }}>
                      #{tag.trim()}
                    </span>
                  ))}
                </div>
              )}

              <button 
                className="btn btn-primary" 
                type="button"
                style={{ width: '100%', marginTop: 'auto' }}
                onClick={() => navigate(`/games/${game.id}`)}
              >
                Ver detalle
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default GamesCatalog
