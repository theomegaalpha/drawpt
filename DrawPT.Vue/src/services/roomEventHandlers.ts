// src/services/roomEventHandlers.ts
import { useRoomJoinStore } from '@/stores/roomJoinStore'
import type { HubConnection } from '@microsoft/signalr'
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

export function registerRoomHubEvents() {
  signalRService.on('roomIsFull', handleRoomIsFull)
  signalRService.on('alreadyInRoom', handleAlreadyInRoom)
  signalRService.on('navigateToRoom', handleNavigateToRoom)
  signalRService.on('errorJoiningRoom', handleErrorJoiningRoom)
}

export function unregisterRoomHubEvents() {
  signalRService.off('roomIsFull')
  signalRService.off('alreadyInRoom')
  signalRService.off('navigateToRoom')
  signalRService.off('errorJoiningRoom')
}
