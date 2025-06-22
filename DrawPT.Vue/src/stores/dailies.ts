import { defineStore } from 'pinia'
import api from '@/api/api'
import { isAuthenticated } from '@/lib/auth'
import type { DailyAnswer, DailyQuestion } from '@/models/dailyModels'

export const useDailiesStore = defineStore('dailies', {
  state: () => ({
    dailyQuestions: [] as DailyQuestion[],
    dailyQuestion: {} as DailyQuestion,
    dailyAnswer: {} as DailyAnswer,
    dailyAnswerHistory: [] as DailyAnswer[],
    isLoadingDaily: true,
    isAssessing: false,
    showAssessment: false,
    dailyError: false
  }),
  getters: {
    getDailyQuestion: (state) => state.dailyQuestion,
    hasDailyAnswer: (state) => state.dailyAnswer.guess !== undefined,
    isDailyQuestionLoaded: (state) => Object.keys(state.dailyQuestion).length > 0,
    isDailyAnswerLoaded: (state) => Object.keys(state.dailyAnswer).length > 0,
    imageLoaded: (state) => {
      return state.dailyQuestion.imageUrl && state.dailyQuestion.imageUrl.length > 0
    }
  },
  actions: {
    async initStore() {
      if (this.dailyQuestions.length === 0) {
        const questions = await api.getDailyQuestions()
        this.dailyQuestions = questions
      }

      if (this.isDailyQuestionLoaded && this.isDailyAnswerLoaded) {
        return
      }

      this.dailyError = false
      this.isLoadingDaily = true
      try {
        if (!this.isDailyQuestionLoaded) {
          const question = await api.getDailyQuestion()
          this.dailyQuestion = question
          if (!question?.imageUrl) this.dailyError = true
        }

        if (await isAuthenticated()) {
          if (!this.isDailyAnswerLoaded) {
            const answer = await api.getDailyAnswer()
            if (!answer) return
            this.dailyAnswer = answer
            this.setShowAssessment(answer?.guess ? true : false)
          }
        } else if (!this.isDailyAnswerLoaded) {
          const storedData = localStorage.getItem('dailyAnswer')
          if (storedData) {
            try {
              const parsedData = JSON.parse(storedData)
              const today = new Date().toISOString().split('T')[0] // YYYY-MM-DD

              if (parsedData.storedDate === today && parsedData.answer) {
                // Optional: Add a check here to ensure the stored answer is for the current dailyQuestion
                // e.g., if (parsedData.answer.questionId === this.dailyQuestion.id) {
                this.dailyAnswer = parsedData.answer
                this.setShowAssessment(parsedData.answer?.guess ? true : false)
                // } else {
                //   localStorage.removeItem('dailyAnswer'); // Stale answer for a different question
                // }
              } else {
                // Stored data is not from today or malformed, remove it
                localStorage.removeItem('dailyAnswer')
              }
            } catch (e) {
              console.error('Failed to parse dailyAnswer from localStorage', e)
              localStorage.removeItem('dailyAnswer') // Clear corrupted data
            }
          }
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
    setIsAssessing(assessing: boolean) {
      this.isAssessing = assessing
    },
    setShowAssessment(show: boolean) {
      this.showAssessment = show
    },
    async setDailyAnswer(answer: DailyAnswer) {
      this.dailyAnswer = answer
      this.setShowAssessment(answer.guess ? true : false)
      if (!(await isAuthenticated())) {
        try {
          const today = new Date().toISOString().split('T')[0] // YYYY-MM-DD
          const dataToStore = {
            answer: answer,
            storedDate: today
            // Optional: Consider storing questionId for more robust validation on load
            // questionId: this.dailyQuestion.id
          }
          localStorage.setItem('dailyAnswer', JSON.stringify(dataToStore))
        } catch (e) {
          console.error('Failed to save dailyAnswer to localStorage', e)
        }
      }
    }
  }
})
