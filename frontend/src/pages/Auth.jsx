import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Card from '../components/Card'
import Input from '../components/Input'
import Button from '../components/Button'
import { useAuth } from '../context/AuthContext'

function Auth() {
  const navigate = useNavigate()
  const { login, register, loading, error, isAuthenticated } = useAuth()
  const [loginData, setLoginData] = useState({ emailOrUsername: '', password: '' })
  const [registerData, setRegisterData] = useState({
    nombre: '',
    email: '',
    username: '',
    password: '',
  })

  if (isAuthenticated) {
    navigate('/profile')
  }

  const handleLogin = async () => {
    const success = await login({
      emailOrUsername: loginData.emailOrUsername,
      password: loginData.password,
    })
    if (success) {
      navigate('/profile')
    }
  }

  const handleRegister = async () => {
    const success = await register({
      nombre: registerData.nombre,
      email: registerData.email,
      username: registerData.username,
      password: registerData.password,
    })
    if (success) {
      navigate('/profile')
    }
  }

  return (
    <div className="page auth-grid">
      <Card>
        <h2>Iniciar sesión</h2>
        <div className="stack">
          <Input
            label="Email o usuario"
            placeholder="correo@ejemplo.com"
            value={loginData.emailOrUsername}
            onChange={(event) =>
              setLoginData((prev) => ({ ...prev, emailOrUsername: event.target.value }))
            }
          />
          <Input
            label="Contraseña"
            type="password"
            placeholder="••••••••"
            value={loginData.password}
            onChange={(event) => setLoginData((prev) => ({ ...prev, password: event.target.value }))}
          />
          {error && <p className="muted">{error}</p>}
          <Button onClick={handleLogin} disabled={loading}>
            Entrar
          </Button>
        </div>
      </Card>
      <Card>
        <h2>Registro</h2>
        <div className="stack">
          <Input
            label="Nombre"
            placeholder="Tu nombre"
            value={registerData.nombre}
            onChange={(event) => setRegisterData((prev) => ({ ...prev, nombre: event.target.value }))}
          />
          <Input
            label="Email"
            placeholder="correo@ejemplo.com"
            value={registerData.email}
            onChange={(event) => setRegisterData((prev) => ({ ...prev, email: event.target.value }))}
          />
          <Input
            label="Usuario"
            placeholder="nickname"
            value={registerData.username}
            onChange={(event) => setRegisterData((prev) => ({ ...prev, username: event.target.value }))}
          />
          <Input
            label="Contraseña"
            type="password"
            placeholder="••••••••"
            value={registerData.password}
            onChange={(event) => setRegisterData((prev) => ({ ...prev, password: event.target.value }))}
          />
          {error && <p className="muted">{error}</p>}
          <Button variant="secondary" onClick={handleRegister} disabled={loading}>
            Crear cuenta
          </Button>
        </div>
      </Card>
    </div>
  )
}

export default Auth
