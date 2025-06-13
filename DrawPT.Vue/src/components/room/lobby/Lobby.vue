<script setup lang="ts">
import PlayerList from './PlayerList.vue' // New import
import RoomInfo from './RoomInfo.vue' // New import
import { usePlayerStore } from '@/stores/player'
import { useRoomStore } from '@/stores/room'
import { useVolumeStore } from '@/stores/volumeStore'
import service from '@/services/signalRService'
import FlickeringGrid from './FlickeringGrid.vue'
import { onMounted } from 'vue'

const playerStore = usePlayerStore() // Renamed for clarity
const roomStore = useRoomStore() // Renamed for clarity
const volumeStore = useVolumeStore()
const { setMusicUrl, togglePlayMusic } = volumeStore

const startGame = async () => {
  var gameStarted = await service.invoke('startGame')
  if (!gameStarted) {
    console.error('Game could not be started')
  }
}

onMounted(() => {
  setMusicUrl('/music/lofi1.mp3')
  togglePlayMusic()
})
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

      <PlayerList />
    </main>
  </FlickeringGrid>
</template>
