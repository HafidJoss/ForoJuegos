import { useEffect, useState } from 'react'
import Card from '../components/Card'
import Button from '../components/Button'
import { listReports, moderateReport } from '../services/reportService'
import { useAuth } from '../context/AuthContext'

const estados = [
  { value: '', label: 'Todos' },
  { value: 'Abierto', label: 'Abierto' },
  { value: 'Resuelto', label: 'Resuelto' },
  { value: 'Rechazado', label: 'Rechazado' },
]

const acciones = [
  { value: 'Ocultar', label: 'Ocultar' },
  { value: 'Eliminar', label: 'Eliminar' },
  { value: 'Advertir', label: 'Advertir' },
  { value: 'Suspender', label: 'Suspender' },
]

function Moderation() {
  const { token } = useAuth()
  const [estadoFiltro, setEstadoFiltro] = useState('')
  const [reports, setReports] = useState([])
  const [selectedReport, setSelectedReport] = useState(null)
  const [state, setState] = useState({ loading: true, error: '', success: '' })

  const loadReports = async () => {
    try {
      setState({ loading: true, error: '', success: '' })
      const data = await listReports(token, { estado: estadoFiltro })
      setReports(data || [])
      setState({ loading: false, error: '', success: '' })
    } catch {
      setState({ loading: false, error: 'No se pudieron cargar los reportes.', success: '' })
    }
  }

  useEffect(() => {
    loadReports()
  }, [estadoFiltro])

  const handleAction = async (accion) => {
    if (!selectedReport) return
    try {
      await moderateReport(
        selectedReport.id,
        { accionTomada: accion, rechazar: false },
        token,
      )
      setState({ loading: false, error: '', success: 'Acción aplicada.' })
      loadReports()
    } catch {
      setState({ loading: false, error: 'No se pudo aplicar la acción.', success: '' })
    }
  }

  const handleReject = async () => {
    if (!selectedReport) return
    try {
      await moderateReport(
        selectedReport.id,
        { accionTomada: 'Ninguna', rechazar: true },
        token,
      )
      setState({ loading: false, error: '', success: 'Reporte rechazado.' })
      loadReports()
    } catch {
      setState({ loading: false, error: 'No se pudo rechazar el reporte.', success: '' })
    }
  }

  return (
    <div className="page">
      <Card>
        <div className="card-header">
          <h2>Moderación</h2>
          <div className="stack">
            <label className="input-field">
              <span>Estado</span>
              <select value={estadoFiltro} onChange={(event) => setEstadoFiltro(event.target.value)}>
                {estados.map((estado) => (
                  <option key={estado.value} value={estado.value}>
                    {estado.label}
                  </option>
                ))}
              </select>
            </label>
          </div>
        </div>
        {state.loading && <p className="muted">Cargando reportes...</p>}
        {state.error && <p className="muted">{state.error}</p>}
        {state.success && <p className="muted">{state.success}</p>}
        {!state.loading && !state.error && reports.length === 0 && (
          <p className="muted">No hay reportes.</p>
        )}
        <div className="stack">
          {reports.map((report) => (
            <button
              key={report.id}
              type="button"
              className={`card ${selectedReport?.id === report.id ? 'selected' : ''}`}
              onClick={() => setSelectedReport(report)}
            >
              <div className="list-item">
                <div>
                  <strong>{report.motivo}</strong>
                  <p className="muted">{report.tipoObjetivo}</p>
                </div>
                <span className="muted">{report.estado}</span>
              </div>
            </button>
          ))}
        </div>
      </Card>

      {selectedReport && (
        <Card>
          <h3>Detalle del reporte</h3>
          <p className="muted">Motivo: {selectedReport.motivo}</p>
          {selectedReport.evidencia && <p className="muted">Evidencia: {selectedReport.evidencia}</p>}
          <div className="stack">
            {acciones.map((accion) => (
              <Button key={accion.value} onClick={() => handleAction(accion.value)}>
                {accion.label}
              </Button>
            ))}
            <Button variant="ghost" onClick={handleReject}>
              Rechazar
            </Button>
          </div>
        </Card>
      )}
    </div>
  )
}

export default Moderation
