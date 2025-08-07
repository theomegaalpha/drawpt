import type { Player, PlayerResult } from './player'

export interface IGameConfiguration {
  maxPlayers: number
  totalRounds: number
  themeTimeout: number
  questionTimeout: number
  promptTimeout: number
  transitionDelay: number
  playerPromptMode: boolean
}

export enum GameStatus {
  WaitingForPlayers = 0,
  JustStarted = 1,
  StartingRound = 2,
  AskingTheme = 3,
  AskingQuestion = 4,
  AskingImagePrompt = 5,
  AskingGamble = 6,
  ShowingRoundResults = 7,
  ShowingGambleResults = 8,
  Completed = 9,
  Abandoned = 10
}

export interface GameState {
  roomCode: string
  currentRound: number
  totalRounds: number
  gameConfiguration: IGameConfiguration
  hostPlayerId: string
  players: Player[]
  currentStatus: GameStatus
}

export interface GameResults {
  playerResults: PlayerResult[]
  totalRounds: number
  wasCompleted: boolean
  endedAt: string
}

export interface GameGamble {
  gamblerId: string
  playerId: string
  isHigh: boolean
  createdAt: string
  score: number
  bonusPoints: number
}

export interface GameQuestion {
  id: string
  playerGenerated: boolean
  playerId: string
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
  playerId: string
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
  id: string
  theme: string
  playerId: string
  playerGenerated: boolean
  roundNumber: number
  originalPrompt: string
  imageUrl: string
  createdAt: string
}
