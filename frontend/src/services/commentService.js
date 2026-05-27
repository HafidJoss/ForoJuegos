import { request } from './apiClient'

export function listCommentsByReview(reviewId, { page = 1, pageSize = 10 } = {}) {
  const params = new URLSearchParams({ page, pageSize })
  return request(`/comments/by-review/${reviewId}?${params.toString()}`)
}

export function listCommentsByUgc(ugcId, { page = 1, pageSize = 10 } = {}) {
  const params = new URLSearchParams({ page, pageSize })
  return request(`/comments/by-ugc/${ugcId}?${params.toString()}`)
}

export function createComment(payload, token) {
  return request('/comments', {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  })
}

export function deleteComment(id, token) {
  return request(`/comments/${id}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}
