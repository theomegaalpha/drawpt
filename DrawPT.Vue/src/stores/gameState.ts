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
      this.currentStatus = gameState.currentStatus
      // UI defaults
      this.themes = []
      this.currentTheme = ''
      this.currentImageUrl = ''
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
    removePlayer(player: Player) {
      const index = this.players.findIndex((p) => p.id === player.id)
      if (index !== -1) {
        this.players.splice(index, 1)
      }
    },

    startGame() {
      this.currentStatus = GameStatus.JustStarted
    },

    handleThemeSelectedEvent(theme: string) {
      this.themes = []
      this.showImageLoader = true
      this.currentTheme = theme
    },
    handleBroadcastRoundResultsEvent(roundResult: RoundResults) {
      this.shouldShowResults = true
      this.showImageLoader = false
      this.currentImageUrl = ''
      this.roundResults.push(roundResult)
      this.currentStatus = GameStatus.ShowingRoundResults
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
      this.isGuessLocked = true
      this.currentStatus = GameStatus.AskingTheme
    },
    handleThemeSelectionEvent(themes: string[]) {
      this.currentImageUrl = ''
      this.showImageLoader = false
      this.shouldShowResults = false
      this.themes = themes
      this.currentStatus = GameStatus.AskingTheme
    },
    prepareForQuestion(question: PlayerQuestion) {
      this.shouldShowResults = false
      this.showImageLoader = false
      this.isGuessLocked = false // Unlock guess input
      this.currentImageUrl = question.imageUrl || ''
      this.currentRound = question.roundNumber // Keep track of current round
      const scoreboardStore = useScoreboardStore()
      scoreboardStore.setRound(question.roundNumber)
      this.currentStatus = GameStatus.AskingQuestion
    },

    // Actions called from Game.vue UI interactions or internal logic
    clearThemes() {
      this.themes = []
    },
    setGuessLock(locked: boolean) {
      this.isGuessLocked = locked
    },
    clearBonusPointsDisplay() {
      this.currentBonusPoints = 0
    },
    playerSelectedTheme(theme: string) {
      this.currentTheme = theme
      this.themes = []
    },
    handleEndGameEvent() {
      this.shouldShowResults = true
      this.currentImageUrl = ''
      this.themes = []
      this.currentBonusPoints = 0
      this.isGuessLocked = true
      this.currentRound = 0
      this.showImageLoader = false
      this.currentStatus = GameStatus.Completed
    }
  }
})
