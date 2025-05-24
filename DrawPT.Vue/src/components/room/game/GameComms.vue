<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import service from '@/services/signalRService'
import { usePlayerStore } from '@/stores/player'
import { useRoomStore } from '@/stores/room'
import { useScoreboardStore } from '@/stores/scoreboard'
import { useNotificationStore } from '@/stores/notifications'
import type { Player } from '@/models/player'

const { updateGameResults } = useScoreboardStore()
const { player: you, updateConnectionId } = usePlayerStore()
const { room, startGame, addPlayer, removePlayer } = useRoomStore()
const { addGameNotification } = useNotificationStore()
const { VITE_HUB_URL } = import.meta.env

onMounted(async () => {
  service.startConnection(VITE_HUB_URL).then(async () => {
    service.invoke('joinGame', room.code, you.id)

    service.on('playerJoined', (player: Player) => {
      if (player.id !== you.id) addGameNotification(`${player.username} has joined the game!`)
      addPlayer(player)
    })

    service.on('playerLeft', (player: Player) => {
      addGameNotification(`${player.username} has left the game.`)
      removePlayer(player)
    })

    service.on('successfullyJoined', (connectionId) => {
      addGameNotification('Welcome to the party!')
      updateConnectionId(connectionId)
    })

    service.on('gameStarted', (config) => {
      console.log('Game has started!', config)
      startGame()
      addGameNotification('Game has started!')
    })

    service.on('broadcastFinalResults', (results) => {
      addGameNotification('The results are in!!!')
      updateGameResults(results)
    })

    service.on('writeMessage', (message) => {
      addGameNotification(message)
    })
  })
})

onUnmounted(() => {
  service.stopConnection()
})
</script>

<template>
  <div></div>
</template>
