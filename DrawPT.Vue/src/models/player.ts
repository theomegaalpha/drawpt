export interface Player {
  id: string
  connectionId: string
  color: string
  username: string
}

export interface PlayerResult extends Player {
  finalScore: number
}
