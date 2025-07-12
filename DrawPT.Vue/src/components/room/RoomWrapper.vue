<script setup lang="ts">
import SetUsername from '@/components/room/SetUsername.vue'
import Lobby from '@/components/room/lobby/Lobby.vue'
import Game from '@/components/room/game/Game.vue'
import GameNotifications from '@/components/room/GameNotifications.vue'
import GameResults from '@/components/room/game/gameresults/GameResults.vue'
import VolumeControls from '@/components/room/volume/VolumeControls.vue'
import { GameStatus } from '@/models/gameModels'

import { onMounted, onUnmounted } from 'vue'
import { usePlayerStore } from '@/stores/player'
import { useScoreboardStore } from '@/stores/scoreboard'
import { useNotificationStore } from '@/stores/notifications'
import { useRoomJoinStore } from '@/stores/roomJoin'
import { useVolumeStore } from '@/stores/volume'
import { useGameStateStore } from '@/stores/gameState'
import api from '@/api/api'
import service from '@/services/signalRService'
import {
  registerBaseGameHubEvents,
  unregisterBaseGameHubEvents
} from '@/services/gameEventHandlers'
import { useBackgroundMusic } from '@/composables/useBackgroundMusic'
import { registerAudioEvents, unregisterAudioEvents } from '@/services/audioEventHandlers'

useBackgroundMusic()

const playerStore = usePlayerStore()
const scoreboardStore = useScoreboardStore()
const notificationStore = useNotificationStore()
const roomJoinStore = useRoomJoinStore()
const { isModalOpen, toggleModal } = useVolumeStore()
const gameState = useGameStateStore()

onMounted(async () => {
  roomJoinStore.reset()
  scoreboardStore.clearScoreboard()
  gameState.successfullyJoined = false

  api.getPlayer().then((res) => {
    playerStore.updatePlayer(res)
  })

  try {
    if (!service.isConnected) {
      await service.startConnection('/gamehub')
      notificationStore.addGameNotification('Connected to game server.', false)
    }
    registerBaseGameHubEvents()
    registerAudioEvents()
  } catch (err) {
    console.error('SignalR connection failed in RoomWrapper:', err)
    notificationStore.addGameNotification('Failed to connect to the game server.', true)
  }
})

onUnmounted(() => {
  scoreboardStore.clearScoreboard()
  unregisterBaseGameHubEvents()
  unregisterAudioEvents()
  if (service.isConnected) {
    service.stopConnection()
    notificationStore.addGameNotification('Disconnected from game server.', false)
  }
})
</script>

<template>
  <GameNotifications />
  <SetUsername v-if="!gameState.successfullyJoined" />
  <div v-if="gameState.successfullyJoined && playerStore.player?.id" class="h-full">
    <Lobby v-if="gameState.currentStatus === GameStatus.WaitingForPlayers" />
    <Game v-else-if="gameState.currentStatus < GameStatus.Completed" />
    <GameResults v-else-if="gameState.currentStatus === GameStatus.Completed" />

    <!-- Button to toggle Volume Settings Modal -->
    <div class="absolute left-4 top-4 z-[100]">
      <VolumeControls :show="isModalOpen" @close="toggleModal" />
    </div>
  </div>
</template>
