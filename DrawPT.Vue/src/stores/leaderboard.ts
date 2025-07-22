import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { PlayerResult } from '@/models/player'
import type { DailyAnswer } from '@/models/dailyModels'

export const useLeaderboardStore = defineStore('leaderboard', () => {
  const roundNumber = ref(0)
  const isLoading = ref(false)
  const playerResults = ref([] as PlayerResult[])
  const _dailies = ref([] as DailyAnswer[])
  const dailies = computed(() => [..._dailies.value].sort((a, b) => b.score - a.score))

  function clearLeaderboard() {
    roundNumber.value = 0
    isLoading.value = false
    _dailies.value = []
    playerResults.value = []
  }

  function setIsLoading(loading: boolean) {
    isLoading.value = loading
  }

  function fetchDailiesTop20() {
    setIsLoading(true)
    return fetch('/api/dailyanswer/top20')
      .then((response) => response.json())
      .then((data) => {
        _dailies.value = data
      })
      .catch((error) => {
        console.error('Error fetching dailies:', error)
      })
      .finally(() => {
        setIsLoading(false)
      })
  }

  function addDailyAnswer(answer: DailyAnswer) {
    _dailies.value.unshift(answer)
    if (_dailies.value.length > 20) _dailies.value.splice(20)
  }

  function fetchPlayerResults() {
    // setIsLoading(true)
    // return fetch('/api/leaderboard/game-results')
    //   .then((response) => response.json())
    //   .then((data) => {
    //     playerResults.value = data
    //   })
    //   .catch((error) => {
    //     console.error('Error fetching game results:', error)
    //   })
    //   .finally(() => {
    //     setIsLoading(false)
    //   })
  }

  return {
    setIsLoading,
    clearLeaderboard,
    fetchDailiesTop20,
    fetchPlayerResults,
    addDailyAnswer,
    isLoading,
    dailies,
    playerResults
  }
})
