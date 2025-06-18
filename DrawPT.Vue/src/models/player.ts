export interface Player {
  id: string
  connectionId: string
  color: string
  username: string
  avatar: string | null
}

export interface PlayerResult extends Player {
  score: number
}
