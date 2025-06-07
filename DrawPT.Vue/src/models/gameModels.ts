import type { PlayerResult } from './player'

export interface IGameConfiguration {
  MaxPlayers: number
  NumberOfQuestions: number
  QuestionTimeout: number
  ThemeTimeout: number
  TransitionDelay: number
  GamblingEnabled: boolean
}

export interface GameState {
  RoomCode: string
  CurrentRound: number
  GameConfiguration: IGameConfiguration
  TotalRounds: number
  HostPlayerId: string
  Players: string[]
}

export interface GameResults {
  playerResults: PlayerResult[]
}

export interface GameQuestion {
  id: string
  roundNumber: number
  theme: string
  originalPrompt: string
  imageUrl: string
  createdAt: string
}

export interface RoundResults {
  id: string
  roundNumber: number
  theme: string
  question: GameQuestion
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
