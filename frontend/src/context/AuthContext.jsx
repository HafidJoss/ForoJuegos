import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import * as authService from '../services/authService'

const AuthContext = createContext(null)

const storageKey = 'cg_token'

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem(storageKey) || '')
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const isAuthenticated = Boolean(token)

  const loadMe = async (jwt) => {
    try {
      setLoading(true)
      setError('')
      const me = await authService.getMe(jwt)
      setUser(me)
    } catch {
      setToken('')
      localStorage.removeItem(storageKey)
      setUser(null)
      setError('Sesión expirada, inicia sesión nuevamente.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (token) {
      loadMe(token)
    }
  }, [])

  const login = async (payload) => {
    setLoading(true)
    setError('')
    try {
      const result = await authService.login(payload)
      setToken(result.token)
      localStorage.setItem(storageKey, result.token)
      setUser({
        id: result.userId,
        email: result.email,
        username: result.username,
        nombre: result.nombre,
        roles: result.roles || [],
      })
      return true
    } catch {
      setError('Credenciales inválidas')
      return false
    } finally {
      setLoading(false)
    }
  }

  const register = async (payload) => {
    setLoading(true)
    setError('')
    try {
      const result = await authService.register(payload)
      setToken(result.token)
      localStorage.setItem(storageKey, result.token)
      setUser({
        id: result.userId,
        email: result.email,
        username: result.username,
        nombre: result.nombre,
        roles: result.roles || [],
      })
      return true
    } catch {
      setError('No fue posible registrar la cuenta')
      return false
    } finally {
      setLoading(false)
    }
  }

  const logout = () => {
    setToken('')
    setUser(null)
    localStorage.removeItem(storageKey)
  }

  const value = useMemo(
    () => ({
      token,
      user,
      setUser,
      loading,
      error,
      isAuthenticated,
      login,
      register,
      logout,
    }),
    [token, user, loading, error, isAuthenticated],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth debe usarse dentro de AuthProvider')
  }
  return context
}
