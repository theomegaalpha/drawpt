export interface DailyAnswer {
  playerId: string
  username: string
  date: string
  guess: string
  reason: string
  closenessArray: number[]
  score: number
}

export interface DailyQuestion {
  date: string
  style?: string
  theme: string
  imageUrl: string
}
