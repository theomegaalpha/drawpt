<script setup lang="ts">
import { onMounted } from 'vue'
import Game from '@/components/room/game/Game.vue'

import { useGameStateStore } from '@/stores/gameState'

const gameState = useGameStateStore()

onMounted(async () => {
  gameState.startRound(1)
  gameState.currentTheme = 'Dickless Vagrant'
  gameState.currentImageUrl =
    'https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f112667271fdad06396cdb_QDhk9GJWfYfchRCbp8kTMay1FxyeMGxzHkB7IMd3Cfo.webp'
  gameState.isGuessLocked = false
  gameState.addPlayer({
    id: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
    username: 'micro david',
    avatar: '/images/profile-photos/anime-5.png',
    connectionId: 'cFHPnG4NzRkPI4NqvBck9we1g84AK02'
  })
  gameState.addPlayer({
    id: 'f06514a7-0e9a-4664-bf2c-3464855d12az',
    username: 'micro david2',
    avatar: '/images/profile-photos/anime-5.png',
    connectionId: 'cFHPnG4NzRkPI4NqvBck9we1g84AK02'
  })

  // Simulate a player answering after 5 seconds
  setTimeout(() => {
    gameState.handlePlayerAnsweredEvent({
      connectionId: 'cFHPnG4NzRkPI4NqvBck9we1g84AK02',
      id: 'f06514a7-0e9a-4664-bf2c-3164855d12aq',
      playerId: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
      username: 'micro david',
      avatar: '/images/profile-photos/anime-5.png',
      isGambling: false,
      score: 10,
      bonusPoints: 1,
      reason: '',
      guess: 'Some answer',
      submittedAt: new Date().toISOString()
    })
  }, 1000)
})
</script>

<template>
  <main class="h-full">
    <div class="h-full">
      <Game />
    </div>
  </main>
</template>
