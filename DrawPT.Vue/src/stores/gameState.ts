// src/stores/gameState.ts
import { defineStore } from 'pinia'
import type {
  GameState,
  PlayerQuestion,
  IGameConfiguration,
  RoundResults
} from '@/models/gameModels'
import { GameStatus } from '@/models/gameModels'
import { useScoreboardStore } from './scoreboard'
import type { Player, PlayerResult } from '@/models/player'

export const useGameStateStore = defineStore('gameState', {
  state: () => ({
    // GameState properties
    roomCode: '' as string,
    currentRound: 0,
    totalRounds: 0,
    gameConfiguration: {} as IGameConfiguration,
    hostPlayerId: '' as string,
    players: [] as Player[],
    themes: [] as string[],
    selectableThemeOptions: [] as string[],
    currentTheme: '' as string,
    currentImageUrl: '' as string,
    currentStatus: GameStatus.WaitingForPlayers,
    // UI-specific
    successfullyJoined: false,
    roundResults: [] as RoundResults[],
    gameResults: [] as PlayerResult[],
    hasPlayerAction: false,
    showImageLoader: false,
    shouldShowResults: false,
    currentBonusPoints: 0,
    isGuessLocked: true
  }),
  getters: {
    areThemesSelectable: (state) => state.hasPlayerAction && state.themes.length > 0,
    areThemesVisible: (state) => state.themes.length > 0,
    showGameCanvas: (state) => state.currentImageUrl !== ''
  },
  actions: {
    initializeGameState(gameState: GameState) {
      // Map backend state
      this.roomCode = gameState.roomCode || ''
      this.currentRound = gameState.currentRound || 0
      this.totalRounds = gameState.totalRounds || 0
      this.gameConfiguration = gameState.gameConfiguration
      this.hostPlayerId = gameState.hostPlayerId
      this.players = gameState.players || []
      this.themes = gameState.themes || []
      this.currentTheme = gameState.currentTheme || ''
      this.currentImageUrl = gameState.currentImageUrl || ''
      this.currentStatus = gameState.currentStatus
      // UI defaults
      this.showImageLoader = false
      this.shouldShowResults = false
      this.currentBonusPoints = 0
      this.isGuessLocked = true
      this.successfullyJoined = true
    },

    updateRoomCode(code: string) {
      this.roomCode = code
    },
    clearRoom() {
      this.roomCode = ''
      this.currentRound = 0
    },
    addPlayer(player: Player) {
      const existingPlayer = this.players.find((p) => p.id === player.id)
      if (!existingPlayer) {
        this.players.push(player)
      }
    },

    handleThemeSelectionEvent(themes: string[]) {
      this.currentImageUrl = ''
      this.showImageLoader = false
      this.shouldShowResults = false
      this.themes = themes
      this.selectableThemeOptions = []
    },
    handleThemeSelectedEvent() {
      this.themes = []
      this.selectableThemeOptions = []
      this.showImageLoader = true
    },
    handleBroadcastRoundResultsEvent(roundResult: RoundResults) {
      this.shouldShowResults = true
      this.showImageLoader = false
      this.currentImageUrl = ''
      this.roundResults.push(roundResult)
    },
    handleAwardBonusPointsEvent(points: number) {
      this.currentBonusPoints = points
      setTimeout(() => {
        this.currentBonusPoints = 0
      }, 5000)
    },

    prepareForThemeSelection(themes: string[], hasAction: boolean = false) {
      this.hasPlayerAction = hasAction
      this.currentImageUrl = ''
      this.shouldShowResults = false
      this.showImageLoader = false
      this.themes = []
      this.selectableThemeOptions = themes
      this.isGuessLocked = true
    },
    prepareForQuestion(question: PlayerQuestion) {
      this.shouldShowResults = false
      this.showImageLoader = false
      this.isGuessLocked = false // Unlock guess input
      this.currentImageUrl = question.imageUrl || ''
      this.currentRound = question.roundNumber // Keep track of current round
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
    playerSelectedTheme() {
      this.selectableThemeOptions = []
      this.themes = []
    },
    handleEndGameEvent() {
      this.shouldShowResults = true
      this.currentImageUrl = ''
      this.themes = []
      this.selectableThemeOptions = []
      this.currentBonusPoints = 0
      this.isGuessLocked = true
      this.currentRound = 0
      this.showImageLoader = false
    }
  }
})
