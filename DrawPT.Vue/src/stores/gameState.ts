// src/stores/gameState.ts
import { defineStore } from 'pinia'
import type { PlayerQuestion, RoundResults } from '@/models/gameModels'
import { useScoreboardStore } from './scoreboard'

export const useGameStateStore = defineStore('gameState', {
  state: () => ({
    themeOptionsFromSignalR: [] as string[],
    selectableThemeOptionsFromSignalR: [] as string[], // For when the current player needs to select a theme
    currentImageUrl: '' as string,
    shouldShowResults: false,
    currentBonusPoints: 0,
    isGuessLocked: true,
    currentRoundNumber: 0
    // Timeout ID for bonus points display, if managed by store
    // bonusPointsTimeoutId: null as NodeJS.Timeout | null,
  }),
  getters: {
    // Example getter: are themes available for selection by the current player?
    areThemesSelectable: (state) => state.selectableThemeOptionsFromSignalR.length > 0,
    // Example getter: are themes being displayed (but not for selection by current player)?
    areThemesVisible: (state) => state.themeOptionsFromSignalR.length > 0
  },
  actions: {
    // Called by gameEventHandlers.ts for non-interactive events
    handleThemeSelectionEvent(themes: string[]) {
      this.currentImageUrl = ''
      this.shouldShowResults = false
      this.themeOptionsFromSignalR = themes
      this.selectableThemeOptionsFromSignalR = [] // Clear selectable themes if general theme info comes in
    },
    handleThemeSelectedEvent(theme: string) {
      // A theme has been selected (by anyone, or by this player via askTheme)
      this.themeOptionsFromSignalR = []
      this.selectableThemeOptionsFromSignalR = [] // Ensure selection options are cleared
      // Notification is handled by gameEventHandlers or notificationStore directly
    },
    handleBroadcastRoundResultsEvent(roundResults: RoundResults) {
      this.shouldShowResults = true
      // Potentially update other relevant state from gameRound if needed
      // e.g., this.currentRoundNumber = gameRound.roundNumber;
    },
    handleAwardBonusPointsEvent(points: number) {
      this.currentBonusPoints = points
      // The display timeout for bonus points will be handled in the Game.vue component
      // or via a separate action if more complex logic is needed.
    },

    // Actions called by Game.vue for its interactive SignalR handlers ('askTheme', 'askQuestion')
    prepareForThemeSelection(themes: string[]) {
      this.currentImageUrl = ''
      this.shouldShowResults = false
      this.themeOptionsFromSignalR = [] // Clear general theme display
      this.selectableThemeOptionsFromSignalR = themes
      this.isGuessLocked = true // Lock guess while selecting theme
    },
    prepareForQuestion(question: PlayerQuestion) {
      this.shouldShowResults = false
      this.isGuessLocked = false // Unlock guess input
      this.currentImageUrl = question.imageUrl || ''
      this.currentRoundNumber = question.roundNumber // Keep track of current round
      // Scoreboard store's setRound can be called from Game.vue or here
      const scoreboardStore = useScoreboardStore()
      scoreboardStore.setRound(question.roundNumber)
    },

    // Actions called from Game.vue UI interactions or internal logic
    clearSelectableThemes() {
      this.selectableThemeOptionsFromSignalR = []
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
      // This might trigger an optimistic update or simply wait for server confirmation via 'themeSelected' event
      this.selectableThemeOptionsFromSignalR = [] // Clear options after selection
      // The actual `service.invoke('SelectTheme', theme)` would happen in Game.vue's promise resolution
    },
    handleEndGameEvent() {
      this.shouldShowResults = true
      this.currentImageUrl = ''
      this.themeOptionsFromSignalR = []
      this.selectableThemeOptionsFromSignalR = []
      this.currentBonusPoints = 0
      this.isGuessLocked = true
      this.currentRoundNumber = 0
    }
  }
})
