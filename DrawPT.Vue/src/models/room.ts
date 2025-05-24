import type { Player } from './player'

export interface Room {
  id: string
  code: string
  name: string
  playerLimit: number
  players: Player[]
  isGameStarted: boolean
  isPrivate: boolean
  currentRound: number
  totalRounds: number
}
