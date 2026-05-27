import { request } from './apiClient'

/**
 * Lista juegos con filtros opcionales (búsqueda, género, tags, paginación).
 * @param {Object} options - Opciones de filtrado
 * @param {string} options.texto - Texto para buscar en nombre/descripción
 * @param {string} options.genero - Género a filtrar
 * @param {string} options.tags - Tags a filtrar
 * @param {number} options.page - Número de página (default: 1)
 * @param {number} options.pageSize - Tamaño de página (default: 20)
 * @returns {Promise} Lista paginada de juegos
 */
export function listGames({ texto, genero, tags, page = 1, pageSize = 20 } = {}) {
  const params = new URLSearchParams()
  if (texto) params.append('texto', texto)
  if (genero) params.append('genero', genero)
  if (tags) params.append('tags', tags)
  params.append('page', page)
  params.append('pageSize', pageSize)

  return request(`/games?${params.toString()}`)
}

/**
 * Obtiene un juego específico por ID.
 * @param {string} id - GUID del juego
 * @returns {Promise} Detalles completos del juego
 */
export function getGameById(id) {
  return request(`/games/${id}`)
}

/**
 * Obtiene lista simplificada de todos los juegos para selector en UI.
 * Retorna solo Id, Nombre e ImagenPortadaUrl sin paginación.
 * Ideal para dropdowns, autocomplete y selectores.
 * @returns {Promise<Array>} Array de juegos con propiedades: { id, nombre, imagenPortadaUrl }
 */
export function getGamesForSelect() {
  return request('/games/select')
}

/**
 * Obtiene la lista de géneros únicos en la BD.
 * @returns {Promise<Array<string>>}
 */
export function getGenres() {
  return request('/games/genres')
}

/**
 * Obtiene la lista de los tags únicos más populares en la BD.
 * @returns {Promise<Array<string>>}
 */
export function getTags() {
  return request('/games/tags')
}
