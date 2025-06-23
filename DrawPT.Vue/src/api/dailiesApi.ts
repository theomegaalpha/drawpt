import type { DailyAnswer, DailyQuestionEntity, DailyQuestion } from '@/models/dailyModels'
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

export async function getDailyQuestions(): Promise<DailyQuestion[]> {
  const headers: HeadersInit = {}
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch('/api/dailyquestion/list', {
    headers
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch player data')
  }

  return (await response.json()) as DailyQuestion[]
}

export async function getFutureDailyQuestions(): Promise<DailyQuestionEntity[]> {
  const headers: HeadersInit = {}
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch('/api/dailyquestion/future', {
    headers
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch player data')
  }

  return (await response.json()) as DailyQuestionEntity[]
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

export async function createQuestion(date: Date): Promise<DailyQuestion> {
  const headers: HeadersInit = {
    'Content-Type': 'application/json'
  }
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch(`/api/dailyquestion`, {
    method: 'POST',
    headers,
    body: JSON.stringify(date)
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch player data')
  }

  return (await response.json()) as DailyQuestion
}

export async function getDailyAnswerHistory(): Promise<DailyAnswer[]> {
  const headers: HeadersInit = {}
  if (await isAuthenticated()) {
    headers['Authorization'] = `Bearer ${await getAccessToken()}`
  }
  const response = await fetch('/api/dailyanswer/history', {
    headers
  })

  if (!response.ok) {
    console.error(response.status, response.statusText)
    throw new Error('Failed to fetch daily answer history')
  }

  return (await response.json()) as DailyAnswer[]
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
