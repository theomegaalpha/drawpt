<script setup lang="ts">
import SetUsername from '@/components/room/SetUsername.vue'
import Lobby from '@/components/room/Lobby.vue'
import Game from '@/components/room/game/Game.vue'
import GameComms from '@/components/room/game/GameComms.vue'
import GameNotifications from '@/components/room/game/GameNotifications.vue'
import GameResults from '@/components/room/game/postgame/GameResults.vue'

import { onMounted, onUnmounted, ref } from 'vue'
import { useRoomStore } from '@/stores/room'
import { usePlayerStore } from '@/stores/player'
import { useScoreboardStore } from '@/stores/scoreboard'
import api from '@/services/api'

const { room } = useRoomStore()
const { player, updatePlayer } = usePlayerStore()
const { gameResults, clearScoreboard } = useScoreboardStore()
const setUsername = ref(false)

onMounted(() => {
  api.getPlayer().then((res) => {
    updatePlayer(res)
  })
})

onUnmounted(() => {
  clearScoreboard()
})
</script>

<template>
  <SetUsername v-model="setUsername" v-if="!setUsername && player.id" />
  <div v-if="setUsername && player.id">
    <GameComms />
    <GameNotifications />

    <GameResults v-if="gameResults.playerResults.length > 0" />
    <div v-else>
      <Lobby v-if="!room.isGameStarted" />
      <Game />
    </div>
  </div>
</template>
