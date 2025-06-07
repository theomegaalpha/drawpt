import { defineStore } from 'pinia'
import { useNotificationStore } from './notifications'

interface RoomJoinState {
  isLoading: boolean
  canSetUsername: boolean
  joinError: string | null
}

export const useRoomJoinStore = defineStore('roomJoin', {
  state: (): RoomJoinState => ({
    isLoading: false,
    canSetUsername: false,
    joinError: null
  }),
  actions: {
    setLoading(loading: boolean) {
      this.isLoading = loading
    },
    setCanSetUsername(canSet: boolean) {
      this.canSetUsername = canSet
      if (canSet) {
        this.joinError = null // Clear error when username can be set
      }
    },
    setJoinError(error: string | null) {
      this.joinError = error
      this.isLoading = false
      this.canSetUsername = false
      if (error) {
        const notificationStore = useNotificationStore()
        notificationStore.addGameNotification(error, true)
      }
    },
    reset() {
      this.isLoading = false
      this.canSetUsername = false
      this.joinError = null
    },
    handleRoomIsFull() {
      this.setJoinError('Sorry, this room is full.')
    },
    handleAlreadyInRoom() {
      this.setJoinError('You appear to be already in this room or another session is active.')
    },
    handleNavigateToRoom() {
      console.log('Navigating to room...')
      const notificationStore = useNotificationStore()
      notificationStore.addGameNotification(
        'Connected to room. Please set or confirm your username.',
        false
      )
      this.setCanSetUsername(true)
      this.setLoading(false)
    },
    handleErrorJoiningRoom(errorMessage?: string) {
      this.setJoinError(errorMessage || 'An error occurred while trying to join the room.')
    }
  }
})
