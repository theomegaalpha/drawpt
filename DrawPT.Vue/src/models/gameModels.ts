import type { PlayerResult } from './player'

export interface IGameConfiguration {
  MaxPlayers: number
  NumberOfQuestions: number
  QuestionTimeout: number
  ThemeTimeout: number
  TransitionDelay: number
  GamblingEnabled: boolean
}

export enum GameStatus {
  WaitingForPlayers = 0,
  JustStarted = 1,
  StartingRound = 2,
  AskingTheme = 3,
  AskingQuestion = 4,
  ShowingRoundResults = 5,
  Completed = 6,
  Abandoned = 7
}

export interface GameState {
  RoomCode: string
  CurrentRound: number
  TotalRounds: number
  GameConfiguration: IGameConfiguration
  HostPlayerId: string
  Players: string[]
  Themes: string[]
  CurrentTheme: string
  CurrentImageUrl: string
  CurrentStatus: GameStatus
}

export interface GameResults {
  playerResults: PlayerResult[]
  totalRounds: number
  wasCompleted: boolean
  endedAt: string
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
  username: string
  avatar: string | null
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
