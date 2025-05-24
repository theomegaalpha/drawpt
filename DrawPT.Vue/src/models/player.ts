export interface Player {
  id: string
  connectionId: string
  color: string
  username: string
}

export interface PlayerWithExpiration extends Player {
  expiration: Date
}

export interface PlayerResult extends Player {
  finalScore: number
}
