import { request } from './apiClient'

export function login(payload) {
  return request('/auth/login', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function register(payload) {
  return request('/auth/register', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function getMe(token) {
  return request('/auth/me?include=roles', {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}

export function updateProfile(payload, token) {
  return request('/auth/profile', {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  })
}
