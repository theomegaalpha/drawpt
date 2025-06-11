<script setup lang="ts">
import PlayerList from './PlayerList.vue' // New import
import RoomInfo from './RoomInfo.vue' // New import
import { usePlayerStore } from '@/stores/player'
import { useRoomStore } from '@/stores/room'
import service from '@/services/signalRService'
import FlickeringGrid from '../FlickeringGrid.vue' // Adjusted path if Lobby.vue is moved

const playerStore = usePlayerStore() // Renamed for clarity
const roomStore = useRoomStore() // Renamed for clarity

const startGame = async () => {
  var gameStarted = await service.invoke('startGame')
  if (!gameStarted) {
    console.error('Game could not be started')
  }
}

// copyRoomCodeToClipboard logic will be moved to RoomInfo.vue, but invoked from here or via event
</script>

<template>
  <FlickeringGrid color="rgb(197, 162, 255)" darkModeColor="rgb(100, 100, 100)">
    <main
      class="grid min-h-screen grid-cols-1 items-center justify-center gap-3 p-10 md:grid-cols-2"
    >
      <RoomInfo
        :roomCode="roomStore.room?.code"
        :username="playerStore.player?.username"
        :isHost="
          roomStore.room?.players.length > 0 &&
          roomStore.room?.players[0].id === playerStore.player?.id
        "
        @startGame="startGame"
      />
      <!-- PlayerList will handle displaying the list of players -->
      <PlayerList :players="roomStore.room?.players" />
    </main>
  </FlickeringGrid>
</template>

<style scoped>
/* Styles specific to Lobby layout if any, otherwise can be removed or kept minimal */
</style>
