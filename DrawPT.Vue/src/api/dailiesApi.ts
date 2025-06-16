import type { DailyAnswer, DailyQuestion } from '@/models/dailyModels'
import { getAccessToken, isAuthenticated } from '@/lib/auth'

export async function getDailyQuestion(): Promise<DailyQuestion> {
  const headers: HeadersInit = {}
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch('/api/dailyprompt', {
    headers
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch player data')
  }

  return (await response.json()) as DailyQuestion
}

export async function getDailyAnswer(): Promise<DailyAnswer> {
  const headers: HeadersInit = {}
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch('/api/dailyprompt/myanswer', {
    headers
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch player data')
  }

  return (await response.json()) as DailyAnswer
}

export async function submitAnswer(answer: string): Promise<DailyAnswer> {
  const headers: HeadersInit = {
    'Content-Type': 'application/json'
  }
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch(`/api/dailyprompt`, {
    method: 'POST',
    headers,
    body: JSON.stringify(answer)
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch player data')
  }

  return (await response.json()) as DailyAnswer
}
