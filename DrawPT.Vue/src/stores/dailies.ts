import { defineStore } from 'pinia'
import api from '@/api/api'
import { isAuthenticated } from '@/lib/auth'
import type { DailyAnswer, DailyQuestion } from '@/models/dailyModels'

export const useDailiesStore = defineStore('dailies', {
  state: () => ({
    dailyQuestion: {} as DailyQuestion,
    dailyAnswer: {} as DailyAnswer,
    isLoading: false,
    imageLoaded: false
  }),
  getters: {
    getDailyQuestion: (state) => state.dailyQuestion,
    getDailyAnswer: (state) => state.dailyAnswer,
    isDailyQuestionLoaded: (state) => Object.keys(state.dailyQuestion).length > 0,
    isDailyAnswerLoaded: (state) => Object.keys(state.dailyAnswer).length > 0
  },
  actions: {
    async initStore() {
      if (this.isDailyQuestionLoaded && this.isDailyAnswerLoaded) {
        return
      }

      this.isLoading = true
      this.imageLoaded = false
      try {
        if (!this.isDailyQuestionLoaded) {
          const question = await api.getDailyQuestion()
          this.dailyQuestion = question
          if (question.imageUrl) this.imageLoaded = true
        }

        if ((await isAuthenticated()) && !this.isDailyAnswerLoaded) {
          const answer = await api.getDailyAnswer()
          this.dailyAnswer = answer
        }
      } catch (err) {
        console.error((err as Error).message || 'Failed to initialize daily data')
      } finally {
        this.isLoading = false
      }
    },
    setIsLoading(loading: boolean) {
      this.isLoading = loading
    },
    setImageLoaded(loaded: boolean) {
      this.imageLoaded = loaded
    },
    setDailyAnswer(answer: DailyAnswer) {
      this.dailyAnswer = answer
    }
  }
})
