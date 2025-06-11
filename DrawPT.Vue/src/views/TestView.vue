<script setup lang="ts">
import { onMounted } from 'vue'
import RoundResults from '@/components/room/game/roundresults/RoundResults.vue'
import { useScoreboardStore } from '@/stores/scoreboard'

const { lastRoundResults, addRoundResult } = useScoreboardStore()
var result = {
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
      'https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f112667271fdad06396cdb_QDhk9GJWfYfchRCbp8kTMay1FxyeMGxzHkB7IMd3Cfo.webp',
    createdAt: '2025-06-09T15:07:57.2106036Z'
  },
  answers: [
    {
      id: '4f5a4624-5490-4b98-901e-03140f244b5b',
      connectionId: 'Ul_kz2qGUma0gMytKOIzJwaUau3AK02',
      username: 'Player 1',
      score: 9,
      bonusPoints: 4,
      reason:
        "The phrase captures the core theme of a post-apocalyptic setting, relevant to the original's ruined city and overgrowth, but lacks the detailed imagery and narrative context.",
      submittedAt: '2025-06-09T15:08:03.4328637Z',
      guess: 'post apocalyptic',
      isGambling: false
    },
    {
      id: '4f5a4624-5490-4b98-901e-03140f244b5c',
      connectionId: 'Ul_kz2qGUma0gMytKOIzJwaUau3AK02',
      username: 'Player 2',
      score: 8,
      bonusPoints: 3,
      reason:
        'The phrase captures the essence of a post-apocalyptic world, but lacks the vivid imagery and narrative depth of the original prompt.',
      submittedAt: '2025-06-09T15:08:03.4328637Z',
      guess: 'post apocalyptic world',
      isGambling: false
    },
    {
      id: '4f5a4624-5490-4b98-901e-03140f244b5d',
      connectionId: 'Ul_kz2qGUma0gMytKOIzJwaUau3AK02',
      username: 'Player 3',
      score: 9,
      bonusPoints: 4,
      reason:
        "The phrase captures the core theme of a post-apocalyptic setting, relevant to the original's ruined city and overgrowth, but lacks the detailed imagery and narrative context.",
      submittedAt: '2025-06-09T15:08:03.4328637Z',
      guess: 'post apocalyptic',
      isGambling: false
    },
    {
      id: '4f5a4624-5490-4b98-901e-03140f244b5d',
      connectionId: 'Ul_kz2qGUma0gMytKOIzJwaUau3AK02',
      username: 'Player 4',
      score: 9,
      bonusPoints: 4,
      reason:
        "The phrase captures the core theme of a post-apocalyptic setting, relevant to the original's ruined city and overgrowth, but lacks the detailed imagery and narrative context.",
      submittedAt: '2025-06-09T15:08:03.4328637Z',
      guess: 'post apocalyptic',
      isGambling: false
    }
  ]
}
onMounted(() => {
  console.log('Adding round result')
  addRoundResult(result)
})
</script>

<template>
  <RoundResults v-if="lastRoundResults?.question !== null" />
</template>
