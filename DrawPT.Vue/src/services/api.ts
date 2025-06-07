import type { Player } from '@/models/player'
import { getAccessToken } from '@/lib/auth'

const Api = {
  getPlayer: async (): Promise<Player> => {
    const token = await getAccessToken()
    const response = await fetch(`/api/player`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    if (!response.ok) {
      console.error(response.status, response.statusText)
      throw new Error('Failed to fetch player data')
    }

    return (await response.json()) as Player
  },
  updatePlayer: async (player: Player): Promise<void> => {
    const token = await getAccessToken()
    await fetch(`/api/player`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(player)
    })
  },
  createRoom: async (): Promise<string> => {
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
  },
  checkRoom: async (roomCode: string): Promise<boolean> => {
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
}
export default Api
