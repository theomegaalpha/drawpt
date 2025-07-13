<script setup lang="ts">
import { ref, onBeforeUnmount, onMounted } from 'vue'
import Game from '@/components/room/game/Game.vue'
import signalRService from '@/services/signalRService'
import { registerAudioEvents, unregisterAudioEvents } from '@/services/audioEventHandlers'

import { useGameStateStore } from '@/stores/gameState'

const gameState = useGameStateStore()

onMounted(async () => {
  gameState.startRound(1)
  gameState.currentTheme = 'Dickless Vagrant'
  gameState.currentImageUrl =
    'https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f112667271fdad06396cdb_QDhk9GJWfYfchRCbp8kTMay1FxyeMGxzHkB7IMd3Cfo.webp'
  gameState.isGuessLocked = false
})

onBeforeUnmount(() => {
  unregisterAudioEvents()
})
</script>

<template>
  <div class="h-full">
    <Game />
  </div>
</template>
