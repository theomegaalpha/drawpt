<script setup lang="ts">
import { onBeforeMount } from 'vue'
import GameResults from '@/components/room/game/gameresults/GameResults.vue'
import { useScoreboardStore } from '@/stores/scoreboard'
const { updateGameResults } = useScoreboardStore()
var result = {
  playerResults: [
    {
      id: 'player1-uuid',
      connectionId: 'conn1',
      color: 'blue',
      username: 'PlayerOne',
      score: 150
    },
    {
      id: 'player2-uuid',
      connectionId: 'conn1',
      color: 'blue',
      username: 'PlayerTwo',
      score: 140
    },
    {
      id: 'player3-uuid',
      connectionId: 'conn3',
      color: 'red',
      username: 'PlayerThree',
      score: 200
    }
  ],
  totalRounds: 10,
  wasCompleted: true,
  endedAt: '2025-06-12T10:00:00Z'
}
onBeforeMount(() => {
  console.log('Adding round result')
  updateGameResults(result)
})
</script>

<template>
  <GameResults />
</template>
