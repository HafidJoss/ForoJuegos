import { request } from './apiClient'

export function getFeed({ page = 1, pageSize = 20 } = {}) {
  const params = new URLSearchParams({ page, pageSize })
  return request(`/feed?${params.toString()}`)
}
