<script setup lang="ts">
import { onMounted } from 'vue'
import Game from '@/components/room/game/Game.vue'
import GameResults from '@/components/room/game/gameresults/GameResults.vue'
import type { GameState } from '@/models/gameModels'
import { GameStatus } from '@/models/gameModels'

import { useGameStateStore } from '@/stores/gameState'

const gameState = useGameStateStore()

onMounted(async () => {
  gameState.initializeGameState({} as GameState)
  gameState.startRound(1)
  gameState.addPlayer({
    id: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
    username: 'micro david',
    avatar: '/images/profile-photos/anime-5.png',
    connectionId: 'cFHPnG4NzRkPI4NqvBck9we1g84AK02'
  })
  gameState.addPlayer({
    id: 'f06514a7-0e9a-4664-bf2c-3464855d12az',
    username: 'sean jean',
    avatar: '/images/profile-photos/anime-2.png',
    connectionId: 'cFHPnG4NzRkPI4NqvBck9we1g84AK02'
  })
  gameState.prepareForQuestion({
    id: 'test-question-1',
    playerGenerated: true,
    playerId: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
    roundNumber: 1,
    theme: 'Sample Theme',
    originalPrompt: 'A bright sunflower field under a blue sky',
    imageUrl:
      'https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f112667271fdad06396cdb_QDhk9GJWfYfchRCbp8kTMay1FxyeMGxzHkB7IMd3Cfo.webp',
    createdAt: new Date().toISOString()
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

  setTimeout(() => {
    gameState.handleBroadcastRoundResultsEvent({
      id: 'round-1',
      roundNumber: 1,
      theme: gameState.currentTheme,
      question: {
        id: 'question-1',
        playerGenerated: false,
        playerId: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
        roundNumber: 1,
        theme: gameState.currentTheme,
        originalPrompt:
          'A lone samurai stands on a misty mountain ridge at dawn, clad in intricately detailed armor with etched dragon motifs and crimson silk underlayers; cherry blossom petals swirl around him as golden sunlight breaks through ancient pine trees, casting long shadows on the rugged terrain, the samurai’s katana gleaming with a faint blue aura, embodying both honor and silent strength.',
        imageUrl:
          'https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f112667271fdad06396cdb_QDhk9GJWfYfchRCbp8kTMay1FxyeMGxzHkB7IMd3Cfo.webp',
        createdAt: new Date().toISOString()
      },
      answers: [
        {
          id: 'answer-1',
          playerId: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
          connectionId: 'cFHPnG4NzRkPI4NqvBck9we1g84AK02',
          username: 'micro david',
          avatar: '/images/profile-photos/anime-5.png',
          guess: 'Some answer',
          score: 10,
          bonusPoints: 1,
          reason: 'You provided a detailed and creative answer that matched the theme well.',
          isGambling: false,
          submittedAt: new Date().toISOString()
        }
      ]
    })

    gameState.handleBroadcastGambleResultsEvent({
      gamblerId: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
      playerId: 'f06514a7-0e9a-4664-bf2c-3464855d12az',
      isHigh: true,
      createdAt: new Date().toISOString(),
      score: 12,
      bonusPoints: 3
    })
  }, 5000)

  // simulate game end
  setTimeout(() => {
    gameState.handleBroadcastFinalResultsEvent({
      playerResults: [
        {
          id: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
          connectionId: 'cFHPnG4NzRkPI4NqvBck9we1g84AK02',
          username: 'micro david',
          avatar: '/images/profile-photos/anime-5.png',
          score: 11
        },
        {
          id: 'f06514a7-0e9a-4664-bf2c-3464855d12az',
          connectionId: 'cFHPnG4NzRkPI4NqvBck9we1g84AK02',
          username: 'sean jean',
          avatar: '/images/profile-photos/anime-2.png',
          score: 8
        }
      ],
      totalRounds: 2,
      wasCompleted: true,
      endedAt: new Date().toISOString()
    })
    gameState.handleEndGameEvent()
  }, 6000)

  // Simulate round results
  setTimeout(() => {
    gameState.handleBroadcastRoundResultsEvent({
      id: 'round-1',
      roundNumber: 1,
      theme: gameState.currentTheme,
      question: {
        id: 'question-1',
        playerGenerated: false,
        playerId: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
        roundNumber: 1,
        theme: gameState.currentTheme,
        originalPrompt:
          'A lone samurai stands on a misty mountain ridge at dawn, clad in intricately detailed armor with etched dragon motifs and crimson silk underlayers; cherry blossom petals swirl around him as golden sunlight breaks through ancient pine trees, casting long shadows on the rugged terrain, the samurai’s katana gleaming with a faint blue aura, embodying both honor and silent strength.',
        imageUrl: gameState.currentImageUrl,
        createdAt: new Date().toISOString()
      },
      answers: [
        {
          id: 'answer-1',
          playerId: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
          connectionId: 'cFHPnG4NzRkPI4NqvBck9we1g84AK02',
          username: 'micro david',
          avatar: '/images/profile-photos/anime-5.png',
          guess: 'Some answer',
          score: 10,
          bonusPoints: 1,
          reason: 'You provided a detailed and creative answer that matched the theme well.',
          isGambling: false,
          submittedAt: new Date().toISOString()
        }
      ]
    })
  }, 5000000)
})
</script>

<template>
  <main class="h-full">
    <div class="h-full">
      <Game />
      <GameResults v-if="gameState.currentStatus === GameStatus.Completed" />
    </div>
  </main>
</template>
