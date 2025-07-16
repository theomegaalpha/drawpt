import { getAccessToken } from '@/lib/auth'

export async function getBackgroundMusic(): Promise<string[]> {
  const token = await getAccessToken()
  const response = await fetch(`/api/backgroundmusic`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch background music data')
  }

  return (await response.json()) as string[]
}
