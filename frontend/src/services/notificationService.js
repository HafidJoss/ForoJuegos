import { request } from './apiClient'

export function listNotifications(token, { page = 1, pageSize = 20 } = {}) {
  const params = new URLSearchParams({ page, pageSize })
  return request(`/notifications?${params.toString()}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function markNotificationRead(id, token) {
  return request(`/notifications/${id}/read`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function markAllRead(token) {
  return request('/notifications/read-all', {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}
