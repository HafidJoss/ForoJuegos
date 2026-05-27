import { useState, useEffect, useMemo } from 'react'
import Card from '../components/Card'
import Input from '../components/Input'
import Button from '../components/Button'
import Badge from '../components/Badge'
import { createUgc, listAllUgc } from '../services/ugcService'
import { getGamesForSelect } from '../services/gameService'
import { useAuth } from '../context/AuthContext'
import { createReport } from '../services/reportService'
import '../styles/UgcUpload.css'

/**
 * Formatea el tamaño en bytes a un formato legible (KB, MB, GB).
 */
function formatBytes(bytes, decimals = 2) {
  if (!bytes) return '0 Bytes'
  const k = 1024
  const dm = decimals < 0 ? 0 : decimals
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i]
}

/**
 * UGC Hub Premium
 * 
 * Permite explorar aportes globales de la comunidad (mods, guías, parches)
 * y compartir contenido nuevo mediante un formulario premium glassmorphism.
 */
function UgcUpload() {
  const { token, isAuthenticated } = useAuth()
  
  // Pestaña activa: 'explore' (Galería de la Comunidad) o 'upload' (Compartir Aporte)
  const [activeTab, setActiveTab] = useState('explore')

  // Datos globales
  const [juegos, setJuegos] = useState([])
  const [juegosCargando, setJuegosCargando] = useState(false)
  const [ugcs, setUgcs] = useState([])
  const [ugcsCargando, setUgcsCargando] = useState(false)
  const [ugcsError, setUgcsError] = useState('')

  // Filtros de búsqueda en la galería
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedGameId, setSelectedGameId] = useState('')
  const [selectedTag, setSelectedTag] = useState('')

  // Reportes
  const [reportTarget, setReportTarget] = useState(null)
  const [reportForm, setReportForm] = useState({ motivo: '', evidencia: '' })
  const [reportState, setReportState] = useState({ loading: false, error: '', success: '' })

  // Formulario de Subida
  const [form, setForm] = useState({
    titulo: '',
    descripcion: '',
    juegoId: '',
    tags: '',
    archivo: null,
    nombreArchivo: '',
    foto: null,
    nombreFoto: '',
    fotoPreview: null,
    declaracionLegalAceptada: false,
  })

  const [uploadState, setUploadState] = useState({
    loading: false,
    error: '',
    success: '',
  })

  const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

  // Cargar juegos al montar
  useEffect(() => {
    const cargarJuegos = async () => {
      setJuegosCargando(true)
      try {
        const data = await getGamesForSelect()
        setJuegos(data || [])
      } catch (error) {
        console.error('Error al cargar juegos:', error)
        setJuegos([])
      } finally {
        setJuegosCargando(false)
      }
    }
    cargarJuegos()
  }, [])

  // Cargar UGC global
  const cargarUgcGlobal = async () => {
    setUgcsCargando(true)
    setUgcsError('')
    try {
      const data = await listAllUgc({ page: 1, pageSize: 50 })
      setUgcs(data || [])
    } catch (error) {
      console.error('Error al cargar UGC:', error)
      setUgcsError('No se pudo cargar la galería de aportes. Intenta recargar la página.')
      setUgcs([])
    } finally {
      setUgcsCargando(false)
    }
  }

  useEffect(() => {
    if (activeTab === 'explore') {
      cargarUgcGlobal()
    }
  }, [activeTab])

  // Filtrado reactivo en el cliente para máxima velocidad
  const filteredUgcs = useMemo(() => {
    return ugcs.filter(item => {
      const matchSearch = searchTerm === '' || 
        item.titulo.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (item.descripcion && item.descripcion.toLowerCase().includes(searchTerm.toLowerCase())) ||
        (item.tags && item.tags.toLowerCase().includes(searchTerm.toLowerCase())) ||
        (item.juegoNombre && item.juegoNombre.toLowerCase().includes(searchTerm.toLowerCase()))

      const matchGame = selectedGameId === '' || item.juegoId === selectedGameId

      const matchTag = selectedTag === '' || 
        (item.tags && item.tags.split(',').map(t => t.trim().toLowerCase()).includes(selectedTag.toLowerCase()))

      return matchSearch && matchGame && matchTag
    })
  }, [ugcs, searchTerm, selectedGameId, selectedTag])

  // Extraer todas las etiquetas únicas para mostrarlas como filtros rápidos
  const tagFilters = useMemo(() => {
    const tagsSet = new Set()
    ugcs.forEach(item => {
      if (item.tags) {
        item.tags.split(',').forEach(tag => {
          const trimmed = tag.trim().toLowerCase()
          if (trimmed) {
            tagsSet.add(trimmed)
          }
        })
      }
    })
    return Array.from(tagsSet).slice(0, 15) // Limitar a las 15 etiquetas más frecuentes
  }, [ugcs])

  // Manejo de inputs del formulario de subida
  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target
    setForm((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }))
    if (uploadState.error || uploadState.success) {
      setUploadState({ loading: false, error: '', success: '' })
    }
  }

  // Manejo de archivo principal
  const handleArchivoChange = (e) => {
    const file = e.target.files?.[0]
    if (file) {
      setForm((prev) => ({
        ...prev,
        archivo: file,
        nombreArchivo: file.name,
      }))
      setUploadState({ loading: false, error: '', success: '' })
    }
  }

  // Manejo de foto principal con preview
  const handleFotoChange = (e) => {
    const file = e.target.files?.[0]
    if (file) {
      const reader = new FileReader()
      reader.onload = (event) => {
        setForm((prev) => ({
          ...prev,
          foto: file,
          nombreFoto: file.name,
          fotoPreview: event.target.result,
        }))
        setUploadState({ loading: false, error: '', success: '' })
      }
      reader.readAsDataURL(file)
    }
  }

  const handleEliminarFoto = () => {
    setForm((prev) => ({
      ...prev,
      foto: null,
      nombreFoto: '',
      fotoPreview: null,
    }))
  }

  const handleEliminarArchivo = () => {
    setForm((prev) => ({
      ...prev,
      archivo: null,
      nombreArchivo: '',
    }))
  }

  // Validación de subida
  const validarFormulario = () => {
    if (!form.titulo.trim()) {
      setUploadState({ loading: false, error: 'El título es requerido', success: '' })
      return false
    }
    if (form.titulo.trim().length < 2 || form.titulo.trim().length > 200) {
      setUploadState({ loading: false, error: 'El título debe tener entre 2 y 200 caracteres', success: '' })
      return false
    }
    if (form.descripcion && form.descripcion.length > 4000) {
      setUploadState({ loading: false, error: 'La descripción no puede exceder 4000 caracteres', success: '' })
      return false
    }
    if (!form.archivo) {
      setUploadState({ loading: false, error: 'Debes seleccionar un archivo para subir', success: '' })
      return false
    }
    if (!form.declaracionLegalAceptada) {
      setUploadState({ loading: false, error: 'Debes aceptar la declaración legal para publicar', success: '' })
      return false
    }
    return true
  }

  // Envío del formulario
  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!validarFormulario()) return

    setUploadState({ loading: true, error: '', success: '' })

    try {
      const formData = new FormData()
      formData.append('titulo', form.titulo.trim())
      formData.append('descripcion', form.descripcion?.trim() || '')
      formData.append('juegoId', form.juegoId || '')
      formData.append('tags', form.tags?.trim() || '')
      formData.append('archivo', form.archivo)
      if (form.foto) {
        formData.append('foto', form.foto)
      }
      formData.append('declaracionLegalAceptada', form.declaracionLegalAceptada)

      const response = await createUgc(formData, token)

      setUploadState({
        loading: false,
        error: '',
        success: '¡Contenido publicado exitosamente! Redirigiendo a la galería...',
      })

      // Limpiar formulario
      setForm({
        titulo: '',
        descripcion: '',
        juegoId: '',
        tags: '',
        archivo: null,
        nombreArchivo: '',
        foto: null,
        nombreFoto: '',
        fotoPreview: null,
        declaracionLegalAceptada: false,
      })

      // Redirigir a galería tras 1.5s
      setTimeout(() => {
        setUploadState(prev => ({ ...prev, success: '' }))
        setActiveTab('explore')
      }, 1500)

    } catch (error) {
      console.error('Error al crear UGC:', error)
      setUploadState({
        loading: false,
        error: error.message || 'Error al publicar contenido. Intenta de nuevo.',
        success: '',
      })
    }
  }

  // Reportar UGC
  const handleReportarUgc = async (e) => {
    e.preventDefault()
    if (!isAuthenticated) {
      setReportState({ loading: false, error: 'Debes iniciar sesión para reportar.', success: '' })
      return
    }
    if (!reportForm.motivo.trim()) {
      setReportState({ loading: false, error: 'El motivo es obligatorio.', success: '' })
      return
    }

    setReportState({ loading: true, error: '', success: '' })
    try {
      await createReport(
        {
          tipoObjetivo: 'ContenidoUgc',
          objetivoId: reportTarget.id,
          motivo: reportForm.motivo,
          evidencia: reportForm.evidencia || null,
        },
        token
      )
      setReportState({ loading: false, error: '', success: 'Reporte enviado con éxito.' })
      setReportForm({ motivo: '', evidencia: '' })
      setTimeout(() => {
        setReportTarget(null)
        setReportState(prev => ({ ...prev, success: '' }))
      }, 1500)
    } catch (err) {
    }
  }


  return (
    <div className="ugc-hub-page">
      {/* Encabezado del Hub */}
      <div className="ugc-hub-banner">
        <div className="ugc-hub-banner-content">
          <h1>HUB DE CONTENIDO UGC</h1>
          <p>Explora, descarga y comparte guías, mods y parches premium creados por y para la comunidad gamer.</p>
        </div>
      </div>

      {/* Selector de Pestañas Glassmorphic */}
      <div className="ugc-tabs-container">
          <button 
            className={`ugc-tab-btn ${activeTab === 'explore' ? 'active' : ''}`}
            onClick={() => setActiveTab('explore')}
          >
            Explorar Comunidad
          </button>
          <button 
            className={`ugc-tab-btn ${activeTab === 'upload' ? 'active' : ''}`}
            onClick={() => setActiveTab('upload')}
          >
            Compartir Aporte
          </button>
        </div>

      {/* CONTENIDO DE PESTAÑA: EXPLORAR */}
      {activeTab === 'explore' && (
        <div className="ugc-explore-section">
          {/* Barra de Filtros Inmersiva */}
          <div className="ugc-filters-bar">
            <div className="search-box">
              <Input
                type="text"
                placeholder="Buscar por título, descripción, tag o juego..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="ugc-search-input"
                style={{ paddingLeft: '1rem' }}
              />
              {searchTerm && (
                <button className="clear-search-btn" onClick={() => setSearchTerm('')}>
                  ✕
                </button>
              )}
            </div>

            <div className="filters-dropdowns">
              {/* Filtro por Juego */}
              <div className="filter-select-wrapper">
                <select
                  value={selectedGameId}
                  onChange={(e) => setSelectedGameId(e.target.value)}
                  className="ugc-filter-select"
                >
                  <option value="">Todos los Juegos</option>
                  {juegos.map((j) => (
                    <option key={j.id} value={j.id}>
                      {j.nombre}
                    </option>
                  ))}
                </select>
              </div>

              {/* Filtro por Tag */}
              <div className="filter-select-wrapper">
                <select
                  value={selectedTag}
                  onChange={(e) => setSelectedTag(e.target.value)}
                  className="ugc-filter-select"
                >
                  <option value="">Todos los Tags</option>
                  {tagFilters.map((tag) => (
                    <option key={tag} value={tag}>
                      #{tag}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          </div>

          {/* Filtros Rápidos en pills */}
          {tagFilters.length > 0 && (
            <div className="quick-tags-container">
              <span className="quick-tags-title">Tags populares:</span>
              <div className="quick-tags-scroll">
                <button 
                  className={`quick-tag-pill ${selectedTag === '' ? 'active' : ''}`}
                  onClick={() => setSelectedTag('')}
                >
                  Todos
                </button>
                {tagFilters.map(tag => (
                  <button 
                    key={tag}
                    className={`quick-tag-pill ${selectedTag === tag ? 'active' : ''}`}
                    onClick={() => setSelectedTag(selectedTag === tag ? '' : tag)}
                  >
                    #{tag}
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* Alertas */}
          {ugcsError && (
            <div className="alert alert-error">
              <p>{ugcsError}</p>
            </div>
          )}

          {/* Grilla o Estado de Carga */}
          {ugcsCargando ? (
            <div className="ugc-skeleton-grid">
              {[1, 2, 3, 4, 5, 6].map((i) => (
                <div key={i} className="ugc-skeleton-card">
                  <div className="shimmer skeleton-media"></div>
                  <div className="shimmer skeleton-title"></div>
                  <div className="shimmer skeleton-text"></div>
                  <div className="shimmer skeleton-footer"></div>
                </div>
              ))}
            </div>
          ) : filteredUgcs.length === 0 ? (
            <Card className="ugc-no-results-card">
              <h2>No se encontraron aportes</h2>
              <p>
                {searchTerm || selectedGameId || selectedTag 
                  ? 'Intenta modificando los términos de búsqueda o filtros aplicados.' 
                  : 'La comunidad aún no ha subido contenido. ¡Atrévete a ser el primero!'}
              </p>
              {(searchTerm || selectedGameId || selectedTag) && (
                <Button 
                  className="btn-primary" 
                  onClick={() => { setSearchTerm(''); setSelectedGameId(''); setSelectedTag(''); }}
                >
                  Limpiar Filtros
                </Button>
              )}
              {!searchTerm && !selectedGameId && !selectedTag && (
                <Button className="btn-primary" onClick={() => setActiveTab('upload')}>
                  Publicar Aporte Ahora
                </Button>
              )}
            </Card>
          ) : (
            <div className="ugc-hub-grid">
              {filteredUgcs.map((item) => (
                <Card key={item.id} className="ugc-hub-card">
                  {/* Imagen de Portada / Preview */}
                  <div className="ugc-card-media">
                    {item.fotoPath ? (
                      <img 
                        src={`${baseUrl}${item.fotoPath}`} 
                        alt={item.titulo} 
                        className="ugc-card-img"
                        onError={(e) => { e.target.style.display = 'none'; }}
                      />
                    ) : (
                      <div className="ugc-card-img-fallback">
                        <div className="abstract-pattern"></div>
                        <span className="fallback-badge">UGC</span>
                      </div>
                    )}
                    {/* Badge del Juego */}
                    <div className="ugc-card-badge-container">
                      <Badge variant="warning" className="ugc-juego-badge">
                        {item.juegoNombre || 'Global'}
                      </Badge>
                    </div>
                  </div>

                  {/* Contenido de la Tarjeta */}
                  <div className="ugc-card-content">
                    <h3 className="ugc-card-title">{item.titulo}</h3>
                    <div className="ugc-card-meta">
                      <span className="date">{new Date(item.fechaSubida).toLocaleDateString()}</span>
                    </div>

                    <p className="ugc-card-desc">
                      {item.descripcion || 'Sin descripción disponible.'}
                    </p>

                    {/* Lista de Etiquetas */}
                    {item.tags && (
                      <div className="ugc-card-tags">
                        {item.tags.split(',').map((tag) => (
                          <span 
                            key={tag} 
                            className="tag-pill"
                            onClick={() => setSelectedTag(tag.trim().toLowerCase())}
                          >
                            #{tag.trim()}
                          </span>
                        ))}
                      </div>
                    )}
                  </div>

                  {/* Pie de la Tarjeta (Descarga y Acciones) */}
                  <div className="ugc-card-footer">
                    {item.archivoUrl ? (
                      <div className="download-group">
                        <span className="file-info" title={item.archivoNombre}>
                          {formatBytes(item.archivoSize)}
                        </span>
                        <a 
                          href={`${baseUrl}/ugc/download/${item.id}`} 
                          download
                          target="_blank" 
                          rel="noreferrer" 
                          className="btn-download-glow"
                          style={{ display: 'inline-block', textDecoration: 'none' }}
                        >
                          Descargar
                        </a>
                      </div>
                    ) : (
                      <span className="no-file-text">Sin archivo adjunto</span>
                    )}

                    <button
                      className="btn-report-ghost"
                      type="button"
                      onClick={() => setReportTarget(item)}
                    >
                      Reportar
                    </button>
                  </div>
                </Card>
              ))}
            </div>
          )}
        </div>
      )}

      {/* CONTENIDO DE PESTAÑA: COMPARTIR / FORMULARIO */}
      {activeTab === 'upload' && (
        <div className="ugc-upload-section">
          {!isAuthenticated ? (
            <Card className="ugc-auth-locked-card">
              <h2>Acceso Restringido</h2>
              <p>Debes estar registrado e iniciar sesión en la plataforma para poder compartir mods, guías y parches con la comunidad.</p>
              <div className="hint-alert">
                ¡La comunidad gamer está esperando tus increíbles creaciones! Inicia sesión para unirte.
              </div>
            </Card>
          ) : (
            <Card className="ugc-upload-card-wrapper">
              <div className="ugc-upload-card-header">
                <h2>Compartir Contenido con la Comunidad</h2>
                <p>Publica archivos de soporte técnico, guías estratégicas, packs de texturas o mods sin límites.</p>
              </div>

              {/* Mensajes del formulario */}
              {uploadState.error && (
                <div className="alert alert-error">
                  <div className="alert-content">
                    <strong>Error</strong>
                    <p>{uploadState.error}</p>
                  </div>
                </div>
              )}

              {uploadState.success && (
                <div className="alert alert-success">
                  <div className="alert-content">
                    <strong>¡Excelente!</strong>
                    <p>{uploadState.success}</p>
                  </div>
                </div>
              )}

              <form onSubmit={handleSubmit} className="ugc-premium-form">
                {/* Bloque 1: Datos */}
                <div className="form-grid-2">
                  <div className="form-group">
                    <label htmlFor="titulo">
                      Título del Aporte <span className="required">*</span>
                    </label>
                    <Input
                      id="titulo"
                      name="titulo"
                      type="text"
                      value={form.titulo}
                      onChange={handleInputChange}
                      placeholder="Ej: Mod de texturas 4K ultra-realista..."
                      maxLength="200"
                      disabled={uploadState.loading}
                      required
                    />
                    <small className="char-count">{form.titulo.length}/200</small>
                  </div>

                  <div className="form-group">
                    <label htmlFor="juegoId">
                      Juego Vinculado <span className="optional">(Opcional)</span>
                    </label>
                    {juegosCargando ? (
                      <div className="select-placeholder">Cargando catálogo...</div>
                    ) : (
                      <select
                        id="juegoId"
                        name="juegoId"
                        value={form.juegoId}
                        onChange={handleInputChange}
                        disabled={uploadState.loading}
                        className="select-input"
                      >
                        <option value="">-- Vincular a un Juego (Opcional) --</option>
                        {juegos.map((j) => (
                          <option key={j.id} value={j.id}>
                            {j.nombre}
                          </option>
                        ))}
                      </select>
                    )}
                  </div>
                </div>

                <div className="form-group">
                  <label htmlFor="descripcion">
                    Descripción Detallada <span className="optional">(Opcional)</span>
                  </label>
                  <textarea
                    id="descripcion"
                    name="descripcion"
                    value={form.descripcion}
                    onChange={handleInputChange}
                    placeholder="Escribe detalles del mod, características de la guía, compatibilidad de parches, instrucciones de instalación, etc..."
                    maxLength="4000"
                    disabled={uploadState.loading}
                    rows="6"
                    className="textarea-input"
                  />
                  <small className="char-count">{form.descripcion.length}/4000</small>
                </div>

                <div className="form-group">
                  <label htmlFor="tags">
                    Etiquetas de búsqueda <span className="optional">(Opcional)</span>
                  </label>
                  <Input
                    id="tags"
                    name="tags"
                    type="text"
                    value={form.tags}
                    onChange={handleInputChange}
                    placeholder="Ej: mod, texturas, 4k, guía, instalación, optimización..."
                    maxLength="1000"
                    disabled={uploadState.loading}
                  />
                  <small>Separa cada etiqueta con comas ( , )</small>
                </div>

                {/* Bloque 2: Archivos Drag & Drop estético */}
                <div className="files-upload-block">
                  {/* Archivo Principal */}
                  <div className="form-group file-field-group">
                    <label>
                      Archivo Principal del Aporte <span className="required">*</span>
                    </label>
                    <div className="ugc-custom-dropzone">
                      <input
                        id="archivo"
                        type="file"
                        onChange={handleArchivoChange}
                        disabled={uploadState.loading}
                        className="file-input-hidden"
                        required
                      />
                      <label htmlFor="archivo" className="dropzone-label">
                        <div className="dropzone-info">
                          <span className="primary-text">
                            {form.nombreArchivo ? `✓ ${form.nombreArchivo}` : 'Haga clic para seleccionar archivo'}
                          </span>
                          <span className="secondary-text">Formatos admitidos: .zip, .rar, .exe, .pdf, .txt, etc.</span>
                        </div>
                      </label>
                    </div>
                    {form.archivo && (
                      <button
                        type="button"
                        className="btn-remove-file"
                        onClick={handleEliminarArchivo}
                        disabled={uploadState.loading}
                      >
                        ✕ Eliminar archivo seleccionado
                      </button>
                    )}
                  </div>

                  {/* Miniatura / Foto */}
                  <div className="form-group file-field-group">
                    <label>
                      Imagen de Portada / Preview <span className="optional">(Opcional)</span>
                    </label>
                    <div className="ugc-custom-dropzone">
                      <input
                        id="foto"
                        type="file"
                        onChange={handleFotoChange}
                        disabled={uploadState.loading}
                        className="file-input-hidden"
                      />
                      <label htmlFor="foto" className="dropzone-label">
                        <div className="dropzone-info">
                          <span className="primary-text">
                            {form.nombreFoto ? `✓ ${form.nombreFoto}` : 'Haga clic para seleccionar una imagen'}
                          </span>
                          <span className="secondary-text">Se mostrará como miniatura visual en la grilla de la comunidad.</span>
                        </div>
                      </label>
                    </div>

                    {form.fotoPreview && (
                      <div className="form-foto-preview-box">
                        <img src={form.fotoPreview} alt="Preview" />
                        <button
                          type="button"
                          className="btn-remove-foto-overlay"
                          onClick={handleEliminarFoto}
                          disabled={uploadState.loading}
                          title="Eliminar miniatura"
                        >
                          ✕
                        </button>
                      </div>
                    )}
                  </div>
                </div>

                {/* Declaración Legal */}
                <div className="form-legal-wrapper">
                  <div className="legal-checkbox-container">
                    <input
                      id="declaracionLegalAceptada"
                      name="declaracionLegalAceptada"
                      type="checkbox"
                      checked={form.declaracionLegalAceptada}
                      onChange={handleInputChange}
                      disabled={uploadState.loading}
                      required
                    />
                    <label htmlFor="declaracionLegalAceptada">
                      Declaro y confirmo bajo mi responsabilidad que poseo todos los derechos de uso y libre distribución sobre el contenido subido, y que este material no infringe leyes de derechos de autor (copyright) o marcas comerciales de terceros. <span className="required">*</span>
                    </label>
                  </div>
                </div>

                {/* Botón de Enviar */}
                <div className="form-actions-submit">
                  <Button
                    type="submit"
                    disabled={uploadState.loading || !form.archivo || !form.declaracionLegalAceptada}
                    className={`ugc-btn-submit-premium ${uploadState.loading ? 'loading' : ''}`}
                  >
                    {uploadState.loading ? (
                      <>
                        <span className="spinner-small"></span> Procesando Archivos...
                      </>
                    ) : (
                      'PUBLICAR APORTE EN EL HUB'
                    )}
                  </Button>
                </div>
              </form>
            </Card>
          )}
        </div>
      )}

      {/* MODAL DE REPORTES GLASSMORPHIC */}
      {reportTarget && (
        <div className="ugc-report-modal-overlay">
          <Card className="ugc-report-modal-card">
            <button className="modal-close-btn" onClick={() => setReportTarget(null)}>✕</button>
            <h3>Reportar Aporte de la Comunidad</h3>
            <p className="target-title">Aporte: <strong>"{reportTarget.titulo}"</strong></p>

            {reportState.error && (
              <div className="alert alert-error" style={{ margin: '10px 0' }}>
                <p>{reportState.error}</p>
              </div>
            )}

            {reportState.success && (
              <div className="alert alert-success" style={{ margin: '10px 0' }}>
                <p>{reportState.success}</p>
              </div>
            )}

            <form onSubmit={handleReportarUgc} className="report-modal-form">
              <div className="form-group">
                <label htmlFor="motivo">Motivo del reporte <span className="required">*</span></label>
                <textarea
                  id="motivo"
                  rows="3"
                  value={reportForm.motivo}
                  onChange={(e) => setReportForm(prev => ({ ...prev, motivo: e.target.value }))}
                  placeholder="Detalla por qué este aporte infringe las normas (ej: virus, contenido ofensivo, copyright ilegítimo)..."
                  maxLength="500"
                  required
                  disabled={reportState.loading}
                  className="textarea-input"
                />
              </div>

              <div className="form-group">
                <label htmlFor="evidencia">Evidencias o Enlaces adicionales <span className="optional">(Opcional)</span></label>
                <Input
                  id="evidencia"
                  type="text"
                  value={reportForm.evidencia}
                  onChange={(e) => setReportForm(prev => ({ ...prev, evidencia: e.target.value }))}
                  placeholder="Ej: enlace a la obra original o capturas..."
                  disabled={reportState.loading}
                />
              </div>

              <div className="modal-actions">
                <button 
                  type="button" 
                  className="btn-cancel" 
                  onClick={() => setReportTarget(null)}
                  disabled={reportState.loading}
                >
                  Cancelar
                </button>
                <button 
                  type="submit" 
                  className="btn-submit"
                  disabled={reportState.loading}
                >
                  {reportState.loading ? 'Enviando...' : 'Enviar Reporte'}
                </button>
              </div>
            </form>
          </Card>
        </div>
      )}
    </div>
  )
}

export default UgcUpload
