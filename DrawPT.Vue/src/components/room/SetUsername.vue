<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import service from '@/services/signalRService' // Still needed for invoke
import { useNotificationStore } from '@/stores/notifications'
import { usePlayerStore } from '@/stores/player'
import { useRoomJoinStore } from '@/stores/roomJoin'
import { registerRoomHubEvents, unregisterRoomHubEvents } from '@/services/roomEventHandlers'
import api from '@/api/api'
import { onBeforeUnmount } from 'vue'

const router = useRouter()
const route = useRoute()
const notificationStore = useNotificationStore()
const playerStore = usePlayerStore()
const roomJoinStore = useRoomJoinStore()

const username = ref(playerStore.player?.username || '')
const roomCode = ref((route.params.roomCode as string) || '')

// Computed properties from the store
const joinError = computed(() => roomJoinStore.joinError)
const canJoinRoom = computed(() => roomJoinStore.canJoinRoom)

let unsubscribeFromConnectionStatus: (() => void) | null = null
let unsubscribeNavigateToRoom: (() => void) | null = null

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

watch(canJoinRoom, (newVal) => {
  if (newVal) {
    playerStore.setUsername(username.value)
    service.invoke('joinGame', roomCode.value, username.value)
  }
})

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
        return
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

onBeforeUnmount(() => {
  if (unsubscribeNavigateToRoom) {
    unsubscribeNavigateToRoom()
    unsubscribeNavigateToRoom = null
  }
})

onUnmounted(() => {
  unregisterRoomHubEvents()
  if (unsubscribeFromConnectionStatus) {
    unsubscribeFromConnectionStatus()
    unsubscribeFromConnectionStatus = null
  }
})
</script>

<template>
  <div class="flex min-h-screen items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
    <div
      class="text-color-default w-full max-w-md space-y-1 rounded-lg border border-gray-300 bg-white p-10 shadow-lg dark:border-gray-300/20 dark:bg-slate-500/10"
    >
      <h2 class="text-center text-3xl font-extrabold">Joining Room...</h2>
      <h3 class="text-center text-lg font-medium">
        Room Code: <span class="font-bold">{{ roomCode }}</span>
      </h3>
      <div v-if="joinError" class="pb-2">
        <p
          class="rounded-md border border-black/20 p-2 text-sm font-medium text-red-700 dark:border-white/10 dark:text-red-300"
        >
          {{ joinError }}
        </p>
      </div>
      <div v-else class="flex flex-col items-center justify-center py-8">
        <span class="loader mb-4"></span>
        <span class="text-gray-600 dark:text-gray-300">Waiting for server...</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.loader {
  border: 4px solid #f3f3f3;
  border-top: 4px solid #6366f1;
  border-radius: 50%;
  width: 36px;
  height: 36px;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
