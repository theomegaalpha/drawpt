import type { PlayerResult } from './player'

export interface GameResults {
  playerResults: PlayerResult[]
}

export interface GameRound {
  roundNumber: number
  question: PlayerQuestion
  answers: PlayerAnswer[]
}

export interface PlayerAnswerBase {
  guess: string
  isGambling: boolean
}

export interface PlayerAnswer {
  id: string
  connectionId: string
  guess: string
  score: number
  bonusPoints: number
  reason: string
  isGambling: boolean
  submittedAt: string
}

export interface PlayerQuestion {
  Id: string
  theme: string
  roundNumber: number
  originalPrompt: string
  imageUrl: string
}
