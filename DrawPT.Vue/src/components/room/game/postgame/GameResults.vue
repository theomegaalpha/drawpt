<script setup lang="ts">
import { useScoreboardStore } from '@/stores/scoreboard'
import { usePlayerStore } from '@/stores/player'
import PlayerResultCard from './PlayerResultCard.vue'
const scoreboardStore = useScoreboardStore()
const { gameResults } = scoreboardStore
const { player: you } = usePlayerStore()
</script>

<template>
  <div class="fixed top-4 right-4 bottom-4 left-4 border-grey-50">
    <div
      v-for="(result, index) in gameResults.playerResults.sort(
        (a, b) => b.finalScore - a.finalScore
      )"
      :key="index"
    >
      <PlayerResultCard
        :is-you="result.connectionId === you.connectionId"
        :player-result="result"
      />
    </div>
  </div>
</template>
