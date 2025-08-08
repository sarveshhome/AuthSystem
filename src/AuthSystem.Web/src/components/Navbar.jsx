import { Link } from 'react-router-dom'
import { Role } from '../types/roles'

export default function Navbar({ user, onLogout }) {
  return (
    <nav className="bg-white shadow-sm">
      <div className="container mx-auto px-4 py-3 flex justify-between items-center">
        <Link to="/" className="text-xl font-semibold text-blue-600">
          AuthSystem
        </Link>
        
        <div className="flex items-center space-x-4">
          {user ? (
            <>
              {user.role === Role.Admin && (
                <Link to="/admin" className="text-gray-700 hover:text-blue-600">
                  Admin
                </Link>
              )}
              <Link to="/profile" className="text-gray-700 hover:text-blue-600">
                Profile
              </Link>
              <button
                onClick={onLogout}
                className="text-gray-700 hover:text-blue-600"
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="text-gray-700 hover:text-blue-600">
                Login
              </Link>
              <Link to="/register" className="text-gray-700 hover:text-blue-600">
                Register
              </Link>
            </>
          )}
        </div>
      </div>
    </nav>
  )
}