import { request } from './apiClient'

export function listReviewsByGame(gameId, { page = 1, pageSize = 10 } = {}) {
  const params = new URLSearchParams({ page, pageSize })
  return request(`/reviews/by-game/${gameId}?${params.toString()}`)
}

export function listReviewsByUser(userId) {
  return request(`/reviews/by-user/${userId}`)
}

export function createReview(payload, token) {
  return request('/reviews', {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  })
}

export function updateReview(id, payload, token) {
  return request(`/reviews/${id}`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  })
}

export function deleteReview(id, token) {
  return request(`/reviews/${id}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}
