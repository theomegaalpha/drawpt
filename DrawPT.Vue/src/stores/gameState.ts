// src/stores/gameState.ts
import { defineStore } from 'pinia'
import type { PlayerQuestion, RoundResults } from '@/models/gameModels'
import { useScoreboardStore } from './scoreboard'

export const useGameStateStore = defineStore('gameState', {
  state: () => ({
    themeOptions: [] as string[],
    selectableThemeOptions: [] as string[],
    currentImageUrl: '' as string,
    showImageLoader: false,
    shouldShowResults: false,
    currentBonusPoints: 0,
    isGuessLocked: true,
    currentRoundNumber: 0
    // Timeout ID for bonus points display, if managed by store
    // bonusPointsTimeoutId: null as NodeJS.Timeout | null,
  }),
  getters: {
    areThemesSelectable: (state) => state.selectableThemeOptions.length > 0,
    areThemesVisible: (state) => state.themeOptions.length > 0,
    showGameCanvas: (state) => state.currentImageUrl !== ''
  },
  actions: {
    handleThemeSelectionEvent(themes: string[]) {
      this.currentImageUrl = ''
      this.showImageLoader = false
      this.shouldShowResults = false
      this.themeOptions = themes
      this.selectableThemeOptions = []
    },
    handleThemeSelectedEvent(theme: string) {
      this.themeOptions = []
      this.selectableThemeOptions = []
      this.showImageLoader = true
      console.log('handle theme selected event is now', this.showImageLoader)
    },
    handleBroadcastRoundResultsEvent(roundResults: RoundResults) {
      this.shouldShowResults = true
      this.showImageLoader = false
      this.currentImageUrl = ''
    },
    handleAwardBonusPointsEvent(points: number) {
      this.currentBonusPoints = points
      setTimeout(() => {
        this.currentBonusPoints = 0
      }, 5000)
    },

    prepareForThemeSelection(themes: string[]) {
      this.currentImageUrl = ''
      this.shouldShowResults = false
      this.showImageLoader = false
      this.themeOptions = []
      this.selectableThemeOptions = themes
      this.isGuessLocked = true
    },
    prepareForQuestion(question: PlayerQuestion) {
      this.shouldShowResults = false
      this.showImageLoader = false
      this.isGuessLocked = false // Unlock guess input
      this.currentImageUrl = question.imageUrl || ''
      this.currentRoundNumber = question.roundNumber // Keep track of current round
      const scoreboardStore = useScoreboardStore()
      scoreboardStore.setRound(question.roundNumber)
    },

    // Actions called from Game.vue UI interactions or internal logic
    clearSelectableThemes() {
      this.selectableThemeOptions = []
    },
    setGuessLock(locked: boolean) {
      this.isGuessLocked = locked
    },
    clearBonusPointsDisplay() {
      this.currentBonusPoints = 0
    },
    // Action to be called when a theme is chosen by the current player via UI
    // This is distinct from handleThemeSelectedEvent which is a broadcast
    playerSelectedTheme(theme: string) {
      this.selectableThemeOptions = []
      this.themeOptions = []
    },
    handleEndGameEvent() {
      this.shouldShowResults = true
      this.currentImageUrl = ''
      this.themeOptions = []
      this.selectableThemeOptions = []
      this.currentBonusPoints = 0
      this.isGuessLocked = true
      this.currentRoundNumber = 0
      this.showImageLoader = false
    }
  }
})
