<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import service from '@/services/signalRService' // Still needed for invoke
import { useNotificationStore } from '@/stores/notifications'
import { usePlayerStore } from '@/stores/player'
import { useRoomJoinStore } from '@/stores/roomJoinStore'
import { registerRoomHubEvents, unregisterRoomHubEvents } from '@/services/roomEventHandlers'
import api from '@/services/api'
import StandardInput from '../common/StandardInput.vue'

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
      service.invoke('requestToJoinGame', roomCode.value)
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
  api.getPlayer().then((res) => {
    playerStore.updatePlayer(res)
    username.value = res.username || ''
    api.checkRoom(roomCode.value).then((exists) => {
      if (!exists) {
        roomJoinStore.setJoinError('😭 This game has already ended.')
      }
    })
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
    playerStore.setUsername(username.value)
    service.invoke('joinGame', roomCode.value, username.value)
  } catch (error) {
    console.error('Error submitting username or invoking joinGame:', error)
    roomJoinStore.setJoinError('Failed to finalize username or join game. Please try again.')
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
    <div
      class="text-color-default w-full max-w-md space-y-1 rounded-lg border border-gray-300 bg-white p-10 shadow-lg dark:border-gray-300/20 dark:bg-slate-500/10"
    >
      <h2 class="text-center text-3xl font-extrabold">Join Room</h2>
      <h3 class="text-center text-lg font-medium">
        Room Code: <span class="font-bold">{{ roomCode }}</span>
      </h3>

      <div class="pb-2" v-if="joinError">
        <p
          class="rounded-md border border-black/20 p-2 text-sm font-medium text-red-700 dark:border-white/10 dark:text-red-300"
        >
          {{ joinError }}
        </p>
      </div>
      <form @submit.prevent="submitUsernameAndEnter" class="space-y-2">
        <div>
          <label for="username" class="mb-1 block text-sm font-medium">Username</label>
          <StandardInput
            id="username"
            v-model="username"
            class="cursor-text"
            :placeholder="'Enter your username'"
            :disabled="!canSetUsername || isLoading"
            required
          />
        </div>

        <button
          type="submit"
          :disabled="!canSetUsername || isLoading || !username.trim()"
          class="btn-default w-full"
        >
          <span v-if="isLoading && canSetUsername">Joining...</span>
          <span v-else>Join Room</span>
        </button>
      </form>
    </div>
  </div>
</template>

<style scoped></style>
