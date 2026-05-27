import { request } from './apiClient'

export function createReport(payload, token) {
  return request('/reports', {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  })
}

export function listReports(token, { estado, page = 1, pageSize = 20 } = {}) {
  const params = new URLSearchParams({ page, pageSize })
  if (estado) params.append('estado', estado)
  return request(`/reports?${params.toString()}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function getReportById(id, token) {
  return request(`/reports/${id}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function moderateReport(id, payload, token) {
  return request(`/reports/${id}/action`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  })
}
