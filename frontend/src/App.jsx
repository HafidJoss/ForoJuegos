import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import ProtectedRoute from './components/ProtectedRoute'
import Home from './pages/Home'
import GamesCatalog from './pages/GamesCatalog'
import GameDetail from './pages/GameDetail'
import Auth from './pages/Auth'
import UserProfile from './pages/UserProfile'
import UgcUpload from './pages/UgcUpload'
import NotFound from './pages/NotFound'
import Notifications from './pages/Notifications'
import RoleProtectedRoute from './components/RoleProtectedRoute'
import Moderation from './pages/Moderation'
import './App.css'

function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/games" element={<GamesCatalog />} />
          <Route path="/games/:id" element={<GameDetail />} />
          <Route path="/auth" element={<Auth />} />
          <Route
            path="/profile"
            element={
              <ProtectedRoute>
                <UserProfile />
              </ProtectedRoute>
            }
          />
          <Route
            path="/ugc-upload"
            element={
              <ProtectedRoute>
                <UgcUpload />
              </ProtectedRoute>
            }
          />
          <Route
            path="/notifications"
            element={
              <ProtectedRoute>
                <Notifications />
              </ProtectedRoute>
            }
          />
          <Route
            path="/moderation"
            element={
              <RoleProtectedRoute allowedRoles={["Admin", "Moderador"]}>
                <Moderation />
              </RoleProtectedRoute>
            }
          />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  )
}

export default App
