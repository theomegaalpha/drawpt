import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { GameResults, RoundResults } from '@/models/gameModels'
import type { PlayerResult } from '@/models/player'

export const useScoreboardStore = defineStore('scoreboard', () => {
  const roundNumber = ref(0)
  const gameResults = ref({
    playerResults: [] as PlayerResult[],
    totalRounds: 8,
    endedAt: '',
    wasCompleted: true
  } as GameResults)
  const roundResults = ref([] as RoundResults[])

  const lastRoundResults = computed(() => {
    return roundResults.value[roundResults.value.length - 1]
  })

  function clearScoreboard() {
    roundNumber.value = 0
    gameResults.value.playerResults = []
    gameResults.value.wasCompleted = false
    roundResults.value = []
  }

  function setRound(round: number) {
    roundNumber.value = round
  }

  function addRoundResult(round: RoundResults) {
    roundResults.value.push(round)
  }

  function updateGameResults(results: GameResults) {
    gameResults.value = results || {
      playerResults: [],
      totalRounds: 8,
      endedAt: '',
      wasCompleted: true
    }
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
