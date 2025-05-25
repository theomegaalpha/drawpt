import type { Player, PlayerWithExpiration } from '@/models/player'

const Api = {
  getPlayer: async (): Promise<Player> => {
    const storedPlayer = window.localStorage.getItem('storedPlayer')
    if (storedPlayer) {
      const playerWithExpiration = JSON.parse(storedPlayer) as PlayerWithExpiration
      if (playerWithExpiration && new Date(playerWithExpiration.expiration) > new Date()) {
        return playerWithExpiration as Player
      }
    }

    const response = await fetch(`/api/player`)

    if (!response.ok) {
      console.error(response.status, response.statusText)
      throw new Error('Failed to fetch player data')
    }

    const player = await response.json()
    const playerWithExpiration = {} as PlayerWithExpiration
    Object.assign(playerWithExpiration, player)
    playerWithExpiration.expiration = new Date(Date.now() + 6 * 60 * 60 * 1000)
    window.localStorage.setItem('storedPlayer', JSON.stringify(playerWithExpiration))

    return player as Player
  },
  updatePlayer: async (player: Player): Promise<void> => {
    window.localStorage.removeItem('storedPlayer')
    fetch(`/api/player`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(player)
    })
      .then((_) => {
        const playerWithExpiration = {} as PlayerWithExpiration
        Object.assign(playerWithExpiration, player)
        playerWithExpiration.expiration = new Date(Date.now() + 6 * 60 * 60 * 1000)
        window.localStorage.setItem('storedPlayer', JSON.stringify(playerWithExpiration))
      })
      .catch((error: Error) => {
        console.error('Failed to update player.', error)
      })
  },
  createRoom: async (): Promise<string> => {
    const response = await fetch(`/api/room`)

    if (!response.ok) {
      console.error(response.status, response.statusText)
      throw new Error('Failed to create room')
    }

    return response.text()
  },
  checkRoom: async (roomCode: string): Promise<boolean> => {
    const response = await fetch(`/api/room/${roomCode}`)

    if (!response.ok) {
      console.error(response.status, response.statusText)
      return false
    }

    const roomExists = await response.json()
    return roomExists
  }
}
export default Api
