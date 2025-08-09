import { useAuth } from '../hooks/useAuth'

export default function HomePage() {
  const { user } = useAuth()

  return (
    <div>
      <h1 className="text-3xl font-bold mb-4">Welcome to AuthSystem</h1>
      {user ? (
        <p className="text-lg">Hello, {user.email}! You're logged in as {user.role}.</p>
      ) : (
        <p className="text-lg">Please login or register to continue.</p>
      )}
    </div>
  )
}