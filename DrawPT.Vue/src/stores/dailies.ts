import { defineStore } from 'pinia'
import type { DailyAnswer, DailyQuestion } from '@/models/dailyModels'

export const useVolumeStore = defineStore('dailies', {
  state: () => ({
    dailyQuestion: {} as DailyQuestion,
    dailyAnswer: {} as DailyAnswer
  }),
  getters: {
    getDailyQuestion: (state) => state.dailyQuestion,
    getDailyAnswer: (state) => state.dailyAnswer
  },
  actions: {
    loadTodaysQuestion() {
      this.dailyQuestion
    }
  }
})
