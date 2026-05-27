import { request } from './apiClient'

/**
 * Obtiene lista de contenido UGC por juego con paginación.
 * @param {string} gameId - GUID del juego
 * @param {Object} options - Opciones de paginación
 * @param {number} options.page - Número de página (default: 1)
 * @param {number} options.pageSize - Tamaño de página (default: 10)
 * @returns {Promise} Lista de contenido UGC
 */
export function listUgcByGame(gameId, { page = 1, pageSize = 10 } = {}) {
  const params = new URLSearchParams({ page, pageSize })
  return request(`/ugc/by-game/${gameId}?${params.toString()}`)
}

export function listUgcByUser(userId) {
  return request(`/ugc/by-user/${userId}`)
}

export function listAllUgc({ page = 1, pageSize = 20 } = {}) {
  const params = new URLSearchParams({ page, pageSize })
  return request(`/ugc?${params.toString()}`)
}

/**
 * Crea nuevo contenido UGC con archivo y foto opcionales.
 * 
 * Envía FormData (no JSON) con los siguientes campos:
 * - Titulo: Título del contenido (requerido)
 * - Descripcion: Descripción opcional
 * - JuegoId: GUID del juego (opcional, backward compatibility)
 * - Tags: Etiquetas opcionales (separadas por comas)
 * - Archivo: Archivo a subir (requerido, máx 50 MB, sin restricciones)
 * - Foto: Foto/thumbnail opcional (máx 10 MB, sin restricciones)
 * - DeclaracionLegalAceptada: Boolean confirmando derechos de distribución
 * 
 * @param {FormData} formData - FormData con los campos listados arriba
 * @param {string} token - Token JWT de autenticación
 * @returns {Promise<Object>} Respuesta con ID y datos del contenido creado
 * @throws {Error} Si la solicitud falla
 * 
 * @example
 * const formData = new FormData()
 * formData.append('titulo', 'Mi Guía')
 * formData.append('archivo', fileInput.files[0])
 * formData.append('foto', photoInput.files[0]) // opcional
 * formData.append('declaracionLegalAceptada', true)
 * 
 * const response = await createUgc(formData, token)
 */
export function createUgc(formData, token) {
  // FormData ya contiene todos los campos necesarios
  const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

  return fetch(`${baseUrl}/ugc`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      // NO incluir Content-Type: FormData lo configura automáticamente con el boundary correcto
    },
    body: formData,
  })
    .then(response => {
      if (!response.ok) {
        return response.json().then(errorData => {
          const message = errorData?.message || errorData?.error || 'Error al crear UGC'
          throw new Error(message)
        }).catch(err => {
          if (err instanceof Error) throw err
          return response.text().then(text => {
            throw new Error(text || 'Error al crear UGC')
          })
        })
      }
      return response.json()
    })
    .catch(error => {
      console.error('Error en createUgc:', error)
      throw error
    })
}

/**
 * Actualiza contenido UGC existente.
 * @param {string} id - GUID del contenido
 * @param {Object} payload - Datos a actualizar
 * @param {string} token - Token JWT de autenticación
 * @returns {Promise} Contenido actualizado
 */
export function updateUgc(id, payload, token) {
  return request(`/ugc/${id}`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  })
}

/**
 * Elimina contenido UGC (soft delete).
 * @param {string} id - GUID del contenido
 * @param {string} token - Token JWT de autenticación
 * @returns {Promise} Confirmación de eliminación
 */
export function deleteUgc(id, token) {
  return request(`/ugc/${id}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
}
