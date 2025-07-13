// src/services/roomEventHandlers.ts
import { useRoomJoinStore } from '@/stores/roomJoin'
import { useGameStateStore } from '@/stores/gameState'
import type { Player } from '@/models/player'
import signalRService from '@/services/signalRService' // Import the service instance

const handleRoomIsFull = () => {
  const roomJoinStore = useRoomJoinStore()
  roomJoinStore.handleRoomIsFull()
}

const handleAlreadyInRoom = () => {
  const roomJoinStore = useRoomJoinStore()
  roomJoinStore.handleAlreadyInRoom()
}

const handleNavigateToRoom = () => {
  const roomJoinStore = useRoomJoinStore()
  roomJoinStore.handleNavigateToRoom()
}

const handleErrorJoiningRoom = (errorMessage?: string) => {
  const roomJoinStore = useRoomJoinStore()
  roomJoinStore.handleErrorJoiningRoom(errorMessage)
}

const handleInitRoomPlayers = (players: Player[]) => {
  const gameStateStore = useGameStateStore()
  for (let i = 0; i < players.length; i++) {
    gameStateStore.addPlayer(players[i])
  }
}

export function registerRoomHubEvents() {
  signalRService.on('roomIsFull', handleRoomIsFull)
  signalRService.on('alreadyInRoom', handleAlreadyInRoom)
  signalRService.on('navigateToRoom', handleNavigateToRoom)
  signalRService.on('errorJoiningRoom', handleErrorJoiningRoom)
  signalRService.on('initRoomPlayers', handleInitRoomPlayers)
}

export function unregisterRoomHubEvents() {
  signalRService.off('roomIsFull')
  signalRService.off('alreadyInRoom')
  signalRService.off('navigateToRoom')
  signalRService.off('errorJoiningRoom')
  signalRService.off('initRoomPlayers')
}
