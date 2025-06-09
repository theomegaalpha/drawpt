<script setup lang="ts">
import { onMounted } from 'vue'
import RoundResults from '@/components/room/game/postround/RoundResults.vue'
import { useScoreboardStore } from '@/stores/scoreboard'

const { lastRoundResults, addRoundResult } = useScoreboardStore()
onMounted(() => {
  if (!lastRoundResults) {
    addRoundResult({
      id: '69748ede-3f59-4e5c-af68-98b66cb5b989',
      roundNumber: 1,
      theme: '"Post-Apocalyptic"',
      question: {
        id: '9028b2d3-9d30-48d4-807b-b3bd322b810c',
        roundNumber: 1,
        theme: '',
        originalPrompt:
          ' A lone survivor in tattered clothing stands atop a crumbling skyscraper, overlooking a vast city overrun by twisted, overgrown vines and rusted vehicles under a smoky orange sky.',
        imageUrl:
          'http://127.0.0.1:58918/devstoreaccount1/images/gemini-image-31a1b010-bfb8-496f-8c5f-1cab69c8a0ed.png',
        createdAt: '2025-06-09T15:07:57.2106036Z'
      },
      answers: [
        {
          id: '4f5a4624-5490-4b98-901e-03140f244b5b',
          playerId: 'f06514a7-0e9a-4664-bf2c-3464855d12ad',
          connectionId: 'Ul_kz2qGUma0gMytKOIzJwaUau3AK02',
          score: 9,
          bonusPoints: 4,
          reason:
            "The phrase captures the core theme of a post-apocalyptic setting, relevant to the original's ruined city and overgrowth, but lacks the detailed imagery and narrative context.",
          submittedAt: '2025-06-09T15:08:03.4328637Z',
          guess: 'post apocalyptic',
          isGambling: false
        }
      ]
    })
  }
})
</script>

<template>
  <RoundResults />
</template>
