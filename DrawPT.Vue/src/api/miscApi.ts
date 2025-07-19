import { getAccessToken, isAuthenticated } from '@/lib/auth'
import type { Feedback } from '@/models/feedback'

export async function getFeedback(
  page: number = 1,
  includeResolved: boolean = false
): Promise<Feedback[]> {
  const token = await getAccessToken()
  // add includeResolved flag to query
  const params = new URLSearchParams({
    page: page.toString(),
    includeResolved: includeResolved.toString()
  })
  const response = await fetch(`/api/feedback?${params.toString()}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch feedback data')
  }

  return (await response.json()) as Feedback[]
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

export async function updateFeedback(feedback: Feedback): Promise<void> {
  const token = await getAccessToken()
  const response = await fetch(`/api/feedback/${feedback.id}`, {
    method: 'PATCH',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(feedback)
  })
  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to resolve feedback')
  }
}
