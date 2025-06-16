import { getAccessToken } from '@/lib/auth'

export async function createRoom(): Promise<string> {
  const token = await getAccessToken()
  const response = await fetch(`/api/room`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to create room')
  }

  return response.text()
}

export async function checkRoom(roomCode: string): Promise<boolean> {
  const token = await getAccessToken()
  const response = await fetch(`/api/room/${roomCode}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    return false
  }

  const roomExists = await response.json()
  return roomExists
}
