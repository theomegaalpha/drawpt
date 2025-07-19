import { getAccessToken, isAuthenticated } from '@/lib/auth'

export async function getFeedback(page: number = 1): Promise<string[]> {
  const token = await getAccessToken()
  const response = await fetch(`/api/feedback?page=${page}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch feedback data')
  }

  return (await response.json()) as string[]
}

export async function submitFeedback(type: string, message: string): Promise<void> {
  const headers: HeadersInit = { 'Content-Type': 'application/json' }
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch('/api/feedback', {
    method: 'POST',
    headers,
    body: JSON.stringify({ type, message })
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to submit feedback')
  }
}
