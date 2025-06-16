import type { Player } from '@/models/player'
import { getAccessToken } from '@/lib/auth'

export async function getPlayer(): Promise<Player> {
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
}

export async function updatePlayer(player: Player): Promise<void> {
  const token = await getAccessToken()
  await fetch(`/api/player`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(player)
  })
}
