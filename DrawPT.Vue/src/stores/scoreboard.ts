import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { GameResults, RoundResults } from '@/models/gameModels'

export const useScoreboardStore = defineStore('scoreboard', () => {
  const roundNumber = ref(0)
  const gameResults = ref({ playerResults: [] } as GameResults)
  const roundResults = ref([] as RoundResults[])

  const lastRoundResults = computed(() => {
    return roundResults.value[roundResults.value.length - 1]
  })

  function clearScoreboard() {
    roundNumber.value = 0
    gameResults.value.playerResults = []
    roundResults.value = []
  }

  function setRound(round: number) {
    roundNumber.value = round
  }

  function updateGameResults(results: GameResults) {
    gameResults.value.playerResults = results.playerResults
  }

  function addRoundResult(round: RoundResults) {
    roundResults.value.push(round)
  }

  return {
    lastRoundResults,
    roundNumber,
    gameResults,
    clearScoreboard,
    addRoundResult,
    updateGameResults,
    setRound
  }
})
