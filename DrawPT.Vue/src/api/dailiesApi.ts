import type { DailyAnswer, DailyQuestion } from '@/models/dailyModels'
import { getAccessToken, isAuthenticated } from '@/lib/auth'

export async function getDailyQuestion(): Promise<DailyQuestion> {
  const headers: HeadersInit = {}
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch('/api/dailyquestion', {
    headers
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch player data')
  }

  return (await response.json()) as DailyQuestion
}

export async function getDailyAnswer(): Promise<DailyAnswer | null> {
  const headers: HeadersInit = {}
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch('/api/dailyanswer', {
    headers
  })

  if (!response.ok) {
    return null
  }

  return (await response.json()) as DailyAnswer
}

export async function getTop20DailyAnswers(): Promise<DailyAnswer[]> {
  const headers: HeadersInit = {}
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch('/api/dailyanswer/top20', {
    headers
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch top daily answers')
  }

  return (await response.json()) as DailyAnswer[]
}

export async function submitAnswer(answer: string): Promise<DailyAnswer> {
  const headers: HeadersInit = {
    'Content-Type': 'application/json'
  }
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch(`/api/dailyanswer`, {
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
