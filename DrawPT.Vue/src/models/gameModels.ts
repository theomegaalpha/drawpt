import type { PlayerResult } from './player'

export interface GameResults {
  playerResults: PlayerResult[]
}

export interface GameRound {
  roundNumber: number
  question: GameQuestion
  answers: GameAnswer[]
}

export interface GameAnswerBase {
  guess: string
  isGambling: boolean
}

export interface GameAnswer extends GameAnswerBase {
  playerConnectionId: string
  score: number
  bonusPoints: number
  reason: string
}

export interface GameQuestion {
  Id: string
  originalPrompt: string
  imageUrl: string
}
