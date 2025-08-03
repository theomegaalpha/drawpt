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

// default game configuration
const defaultGameConfig: IGameConfiguration = {
  MaxPlayers: 8,
  TotalRounds: 6,
  QuestionTimeout: 40,
  ThemeTimeout: 30,
  TransitionDelay: 50,
  PlayerPromptMode: false
}

export const useGameStateStore = defineStore('gameState', {
  state: () => ({
    // GameState properties
    roomCode: '' as string,
    currentRound: 0,
    totalRounds: 0,
    gameConfiguration: defaultGameConfig,
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
    playerPromptMode: (state) => state.gameConfiguration.PlayerPromptMode,
    askingImagePrompt: (state) => state.currentStatus === GameStatus.AskingImagePrompt,
    areThemesSelectable: (state) => state.hasPlayerAction && state.themes.length > 0,
    areThemesVisible: (state) => !state.hasPlayerAction && state.themes.length > 0,
    showGameCanvas: (state) => state.currentImageUrl !== '',
    lastRoundResults: (state) => {
      return state.roundResults[state.roundResults.length - 1]
    },
    gameEnded: (state) =>
      state.currentStatus === GameStatus.Completed || state.currentStatus === GameStatus.Abandoned
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

    setPlayerPromptMode(mode: boolean) {
      this.gameConfiguration.PlayerPromptMode = mode
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
    handleAwardBonusPointsEvent(points: number) {
      this.currentBonusPoints = points
      setTimeout(() => {
        this.currentBonusPoints = 0
      }, 5000)
    },

    startGame() {
      this.currentStatus = GameStatus.JustStarted
    },
    startRound(roundNumber: number) {
      this.currentStatus = GameStatus.StartingRound
      this.currentRound = roundNumber
      this.playerAnswers.length = 0
    },
    handleBroadcastRoundResultsEvent(roundResult: RoundResults) {
      this.currentStatus = GameStatus.ShowingRoundResults
      this.showImageLoader = false
      this.currentImageUrl = ''
      this.roundResults.push(roundResult)
      this.shouldShowResults = true
    },
    prepareForPlayerImagePrompt() {
      this.currentStatus = GameStatus.AskingImagePrompt
      this.hasPlayerAction = true
      this.currentImageUrl = ''
    },
    prepareForThemeSelection(themes: string[]) {
      this.currentStatus = GameStatus.AskingTheme
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
    },
    handleThemeSelectionEvent(themes: string[]) {
      this.currentStatus = GameStatus.AskingTheme
      this.hasPlayerAction = false
      this.currentImageUrl = ''
      this.showImageLoader = false
      this.shouldShowResults = false
      this.themes = themes
    },
    prepareForQuestion(question: PlayerQuestion) {
      this.currentStatus = GameStatus.AskingQuestion
      this.shouldShowResults = false
      this.showImageLoader = false
      this.isGuessLocked = false // Unlock guess input
      this.currentImageUrl = question.imageUrl || ''
      this.currentRound = question.roundNumber
    },
    handleBroadcastFinalResultsEvent(results: GameResults) {
      this.currentStatus = GameStatus.Completed
      this.gameResults =
        results ||
        ({
          playerResults: [],
          totalRounds: 8,
          endedAt: '',
          wasCompleted: true
        } as GameResults)
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
      this.currentStatus = GameStatus.Completed
      this.shouldShowResults = true
      this.currentImageUrl = ''
      this.themes = []
      this.currentBonusPoints = 0
      this.isGuessLocked = true
      this.currentRound = 0
      this.showImageLoader = false
    },
    handleNavigateBackToLobbyEvent() {
      this.currentStatus = GameStatus.WaitingForPlayers
    }
  }
})
