<script setup lang="ts">
import SetUsername from '@/components/room/SetUsername.vue'
import Lobby from '@/components/room/lobby/Lobby.vue'
import Game from '@/components/room/game/Game.vue'
import GameNotifications from '@/components/room/GameNotifications.vue'
import GameResults from '@/components/room/game/gameresults/GameResults.vue'

import { onMounted, onUnmounted } from 'vue'
import { useRoomStore } from '@/stores/room'
import { usePlayerStore } from '@/stores/player'
import { useScoreboardStore } from '@/stores/scoreboard'
import { useNotificationStore } from '@/stores/notifications'
import { useRoomJoinStore } from '@/stores/roomJoinStore'
import api from '@/services/api'
import service from '@/services/signalRService'
import {
  registerBaseGameHubEvents,
  unregisterBaseGameHubEvents
} from '@/services/gameEventHandlers'

const roomStore = useRoomStore()
const playerStore = usePlayerStore()
const scoreboardStore = useScoreboardStore()
const notificationStore = useNotificationStore()
const roomJoinStore = useRoomJoinStore()

onMounted(async () => {
  roomJoinStore.reset()
  scoreboardStore.clearScoreboard()
  roomStore.setSuccessfullyJoined(false)

  api.getPlayer().then((res) => {
    playerStore.updatePlayer(res)
  })

  try {
    if (!service.isConnected) {
      await service.startConnection('/gamehub')
      notificationStore.addGameNotification('Connected to game server.', false)
    }
    registerBaseGameHubEvents()
  } catch (err) {
    console.error('SignalR connection failed in RoomWrapper:', err)
    notificationStore.addGameNotification('Failed to connect to the game server.', true)
  }
})

onUnmounted(() => {
  scoreboardStore.clearScoreboard()
  unregisterBaseGameHubEvents()
  if (service.isConnected) {
    service.stopConnection()
    notificationStore.addGameNotification('Disconnected from game server.', false)
  }
})
</script>

<template>
  <GameNotifications />
  <SetUsername v-if="!roomStore.successfullyJoined" />
  <div v-if="roomStore.successfullyJoined && playerStore.player?.id">
    <GameResults v-if="scoreboardStore.gameResults.playerResults.length > 0" />
    <div v-else>
      <Lobby v-if="!roomStore.room.isGameStarted" />
      <Game v-else />
    </div>
  </div>
</template>
