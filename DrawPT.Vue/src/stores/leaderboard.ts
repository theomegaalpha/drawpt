import { ref } from 'vue'
import { defineStore } from 'pinia'
import type { PlayerResult } from '@/models/player'
import type { DailyAnswer } from '@/models/dailyModels'

export const useLeaderboardStore = defineStore('leaderboard', () => {
  const roundNumber = ref(0)
  const isLoading = ref(false)
  const dailies = ref([] as DailyAnswer[])
  const playerResults = ref([] as PlayerResult[])

  function clearLeaderboard() {
    roundNumber.value = 0
    isLoading.value = false
    dailies.value = []
    playerResults.value = []
  }

  function setIsLoading(loading: boolean) {
    isLoading.value = loading
  }

  function fetchDailies() {
    setIsLoading(true)
    return fetch('/api/dailyanswer/top20')
      .then((response) => response.json())
      .then((data) => {
        dailies.value = data
      })
      .catch((error) => {
        console.error('Error fetching dailies:', error)
      })
      .finally(() => {
        setIsLoading(false)
      })
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
    fetchDailies,
    fetchPlayerResults,
    isLoading,
    dailies,
    playerResults
  }
})
