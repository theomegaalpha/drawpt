import { defineStore } from 'pinia'
import api from '@/api/api'
import { isAuthenticated } from '@/lib/auth'
import type { DailyAnswer, DailyQuestion } from '@/models/dailyModels'

export const useDailiesStore = defineStore('dailies', {
  state: () => ({
    dailyQuestion: {} as DailyQuestion,
    dailyAnswer: {} as DailyAnswer,
    isLoadingDaily: true,
    isAssessing: false,
    showAssessment: false,
    imageLoaded: false,
    dailyError: false
  }),
  getters: {
    getDailyQuestion: (state) => state.dailyQuestion,
    hasDailyAnswer: (state) => state.dailyAnswer.guess !== undefined,
    isDailyQuestionLoaded: (state) => Object.keys(state.dailyQuestion).length > 0,
    isDailyAnswerLoaded: (state) => Object.keys(state.dailyAnswer).length > 0
  },
  actions: {
    async initStore() {
      if (this.isDailyQuestionLoaded && this.isDailyAnswerLoaded) {
        return
      }

      this.dailyError = false
      this.isLoadingDaily = true
      this.imageLoaded = false
      try {
        if (!this.isDailyQuestionLoaded) {
          const question = await api.getDailyQuestion()
          this.dailyQuestion = question
          if (question?.imageUrl) this.imageLoaded = true
          else this.dailyError = true
        }

        if ((await isAuthenticated()) && !this.isDailyAnswerLoaded) {
          const answer = await api.getDailyAnswer()
          if (!answer) return
          this.dailyAnswer = answer
          this.setShowAssessment(answer?.guess ? true : false)
        }
      } catch (err: any) {
        console.warn((err as Error).message || 'Failed to initialize daily data')
        this.dailyError = true
      } finally {
        this.isLoadingDaily = false
      }
    },
    setIsLoading(loading: boolean) {
      this.isLoadingDaily = loading
    },
    setImageLoaded(loaded: boolean) {
      this.imageLoaded = loaded
    },
    setIsAssessing(assessing: boolean) {
      this.isAssessing = assessing
    },
    setShowAssessment(show: boolean) {
      this.showAssessment = show
    },
    setDailyAnswer(answer: DailyAnswer) {
      this.dailyAnswer = answer
      this.setShowAssessment(answer.guess ? true : false)
    }
  }
})
