import { request } from './apiClient'

export function likeReview(reviewId, token) {
  return request(`/reviews/${reviewId}/like`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function unlikeReview(reviewId, token) {
  return request(`/reviews/${reviewId}/like`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function dislikeReview(reviewId, token) {
  return request(`/reviews/${reviewId}/dislike`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function likeUgc(ugcId, token) {
  return request(`/ugc/${ugcId}/like`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function unlikeUgc(ugcId, token) {
  return request(`/ugc/${ugcId}/like`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function dislikeUgc(ugcId, token) {
  return request(`/ugc/${ugcId}/dislike`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}
