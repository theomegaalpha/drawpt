<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import service from '@/services/signalRService' // Still needed for invoke
import { useNotificationStore } from '@/stores/notifications'
import { usePlayerStore } from '@/stores/player'
import { useRoomJoinStore } from '@/stores/roomJoinStore'
import { registerRoomHubEvents, unregisterRoomHubEvents } from '@/services/roomEventHandlers'
import api from '@/services/api'

const router = useRouter()
const route = useRoute()
const notificationStore = useNotificationStore()
const playerStore = usePlayerStore()
const roomJoinStore = useRoomJoinStore()

const username = ref(playerStore.player?.username || '')
const roomCode = ref((route.params.roomCode as string) || '')

// Computed properties from the store
const canSetUsername = computed(() => roomJoinStore.canSetUsername)
const isLoading = computed(() => roomJoinStore.isLoading)
const joinError = computed(() => roomJoinStore.joinError)

let unsubscribeFromConnectionStatus: (() => void) | null = null

const requestToJoin = async () => {
  roomJoinStore.setLoading(true)
  try {
    if (!service.isConnected) {
      roomJoinStore.setJoinError('Connection to server not ready. Please wait or try refreshing.')
      return
    }

    if (roomCode.value) {
      service.invoke('requestToJoinRoom', roomCode.value)
    } else {
      roomJoinStore.setJoinError('Room code is missing. Cannot join.')
      router.push({ name: 'home' })
    }
  } catch (error) {
    console.error('Error requesting to join room:', error)
    roomJoinStore.setJoinError('Failed to request room entry. Please try again.')
  }
}

onMounted(() => {
  roomJoinStore.reset()
  if (!roomCode.value) {
    notificationStore.addGameNotification('No room code specified.', true)
    router.push({ name: 'home' })
    return
  }
  api.checkRoom(roomCode.value).then((exists) => {
    if (!exists) {
      roomJoinStore.setJoinError('😭 This game has already ended.')
    }
  })

  console.warn('SetUsername: Waiting for SignalR connection from RoomWrapper...')
  unsubscribeFromConnectionStatus = service.onConnectionStatusChanged((isConnected) => {
    if (isConnected) {
      console.log('Requesting to join.')
      registerRoomHubEvents()
      requestToJoin()
      if (unsubscribeFromConnectionStatus) {
        unsubscribeFromConnectionStatus()
        unsubscribeFromConnectionStatus = null
      }
    } else {
      // This case might occur if the connection drops while waiting
      // roomJoinStore.setJoinError('Still waiting for server connection...');
    }
  })

  // Fallback timeout in case the connection event is missed or never occurs
  const timeoutId = setTimeout(() => {
    if (!service.isConnected && unsubscribeFromConnectionStatus) {
      roomJoinStore.setJoinError('Could not connect to server in a timely manner. Please refresh.')
      unsubscribeFromConnectionStatus()
      unsubscribeFromConnectionStatus = null
    }
  }, 7000) // 7 seconds timeout
  // Clear timeout if component unmounts or connection established
  onUnmounted(() => clearTimeout(timeoutId))
})

onUnmounted(() => {
  unregisterRoomHubEvents()
  if (unsubscribeFromConnectionStatus) {
    unsubscribeFromConnectionStatus()
    unsubscribeFromConnectionStatus = null
  }
})

const submitUsernameAndEnter = async () => {
  if (!canSetUsername.value || !username.value.trim()) {
    notificationStore.addGameNotification('Please enter a valid username.', true)
    return
  }
  roomJoinStore.setLoading(true)

  try {
    notificationStore.addGameNotification('huuuhhh???')
    playerStore.setUsername(username.value)
    console.log('Submitting username:', playerStore.player.username)
    api
      .updatePlayer(playerStore.player)
      .then(() => {
        service.invoke('joinGame', roomCode.value)
      })
      .catch((error) => {
        console.error('Error updating username:', error)
        roomJoinStore.setJoinError('Failed to finalize username. Please try again.')
        return
      })
  } catch (error) {
    console.error('Error submitting username or invoking joinGame:', error)
    roomJoinStore.setJoinError('Failed to finalize username or join game. Please try again.')
  }
}
</script>

<template>
  <div class="set-username-container mx-auto mt-8 max-w-md p-4">
    <h2 class="mb-6 text-center text-2xl font-bold">Join Room: {{ roomCode }}</h2>

    <div v-if="isLoading && !canSetUsername" class="mb-4 text-center">
      <p class="text-lg text-gray-600">Attempting to connect to room...</p>
      <div class="mt-2 animate-pulse text-indigo-600">Connecting...</div>
    </div>

    <div v-if="joinError" class="mb-4 rounded-md bg-red-100 p-3 text-center text-red-700">
      <p>{{ joinError }}</p>
    </div>

    <form @submit.prevent="submitUsernameAndEnter" class="space-y-6">
      <div>
        <label for="username" class="mb-1 block text-sm font-medium text-gray-700">Username</label>
        <input
          id="username"
          type="text"
          v-model="username"
          placeholder="Enter your username"
          :disabled="!canSetUsername || isLoading"
          class="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 disabled:bg-gray-100 disabled:text-gray-500 sm:text-sm"
          required
        />
      </div>

      <button
        type="submit"
        :disabled="!canSetUsername || isLoading || !username.trim()"
        class="flex w-full justify-center rounded-md border border-transparent bg-indigo-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:bg-gray-400"
      >
        <span v-if="isLoading && canSetUsername">Joining...</span>
        <span v-else>Join Room</span>
      </button>
    </form>
  </div>
</template>

<style scoped></style>
