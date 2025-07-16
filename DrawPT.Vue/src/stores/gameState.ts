// src/stores/gameState.ts
import { defineStore } from 'pinia'
import type {
  GameState,
  PlayerAnswer,
  PlayerQuestion,
  IGameConfiguration,
  RoundResults,
  GameResults
} from '@/models/gameModels'
import { GameStatus } from '@/models/gameModels'
import type { Player } from '@/models/player'

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
    playerAnswers: [] as PlayerAnswer[],
    roundResults: [] as RoundResults[],
    gameResults: {} as GameResults,
    hasPlayerAction: false,
    showImageLoader: false,
    shouldShowResults: false,
    currentBonusPoints: 0,
    isGuessLocked: true
  }),
  getters: {
    areThemesSelectable: (state) => state.hasPlayerAction && state.themes.length > 0,
    areThemesVisible: (state) => !state.hasPlayerAction && state.themes.length > 0,
    showGameCanvas: (state) => state.currentImageUrl !== '',
    lastRoundResults: (state) => {
      return state.roundResults[state.roundResults.length - 1]
    }
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
      this.playerAnswers.length = 0
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
      this.initializeGameState({} as GameState)
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
    startRound(roundNumber: number) {
      this.currentRound = roundNumber
      this.currentStatus = GameStatus.StartingRound
      this.playerAnswers.length = 0
    },

    handleThemeSelectedEvent(theme: string) {
      this.themes = []
      this.showImageLoader = true
      this.currentTheme = theme
    },
    handlePlayerAnsweredEvent(playerAnswer: PlayerAnswer) {
      if (!this.playerAnswers.includes(playerAnswer)) {
        this.playerAnswers.push(playerAnswer)
      }
    },
    handleBroadcastRoundResultsEvent(roundResult: RoundResults) {
      this.showImageLoader = false
      this.currentImageUrl = ''
      this.roundResults.push(roundResult)
      this.currentStatus = GameStatus.ShowingRoundResults
      this.shouldShowResults = true
    },

    handleBroadcastGameResultsEvent(results: GameResults) {
      this.gameResults =
        results ||
        ({
          playerResults: [],
          totalRounds: 8,
          endedAt: '',
          wasCompleted: true
        } as GameResults)
    },
    handleAwardBonusPointsEvent(points: number) {
      this.currentBonusPoints = points
      setTimeout(() => {
        this.currentBonusPoints = 0
      }, 5000)
    },

    prepareForThemeSelection(themes: string[]) {
      this.hasPlayerAction = true
      this.currentImageUrl = ''
      this.shouldShowResults = false
      this.showImageLoader = false
      for (const theme of themes) {
        if (this.themes.indexOf(theme) === -1) {
          this.themes.push(theme)
        }
      }
      this.isGuessLocked = true
      this.currentStatus = GameStatus.AskingTheme
    },
    handleThemeSelectionEvent(themes: string[]) {
      this.hasPlayerAction = false
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
      this.currentRound = question.roundNumber
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
