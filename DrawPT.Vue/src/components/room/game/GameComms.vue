<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import service from '@/services/signalRService'
import { usePlayerStore } from '@/stores/player'
import { useRoomStore } from '@/stores/room'
import { useNotificationStore } from '@/stores/notifications'
import {
  registerBaseGameHubEvents,
  unregisterBaseGameHubEvents
} from '@/services/gameEventHandlers'
import { registerRoomHubEvents, unregisterRoomHubEvents } from '@/services/roomEventHandlers'

const playerStore = usePlayerStore()
const roomStore = useRoomStore()
const notificationStore = useNotificationStore()

onMounted(async () => {
  try {
    await service.startConnection('/gamehub')
    registerBaseGameHubEvents()
    registerRoomHubEvents()

    if (roomStore.room && roomStore.room.code && playerStore.player && playerStore.player.id) {
      service.invoke('joinGame', roomStore.room.code, playerStore.player.id)
    } else {
      console.warn('Room code or player ID missing, cannot join game. Ensure stores are populated.')
      notificationStore.addGameNotification(
        'Could not join game: missing room or player info.',
        true
      )
    }
  } catch (err) {
    console.error('SignalR connection failed in GameComms:', err)
    notificationStore.addGameNotification('Failed to connect to the game server.', true)
  }
})

onUnmounted(() => {
  unregisterBaseGameHubEvents()
  unregisterRoomHubEvents()
  service.stopConnection()
})
</script>

<template>
  <div></div>
</template>
