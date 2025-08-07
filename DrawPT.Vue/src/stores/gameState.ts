// src/stores/gameState.ts
import { defineStore } from 'pinia'
import type {
  GameState,
  PlayerAnswer,
  PlayerQuestion,
  IGameConfiguration,
  RoundResults,
  GameGamble,
  GameResults,
  GameQuestion
} from '@/models/gameModels'
import { GameStatus } from '@/models/gameModels'
import type { Player } from '@/models/player'

// default game configuration
const defaultGameConfig: IGameConfiguration = {
  maxPlayers: 8,
  totalRounds: 6,
  questionTimeout: 40,
  promptTimeout: 120,
  themeTimeout: 30,
  transitionDelay: 50,
  playerPromptMode: false
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
    currentQuestion: {} as GameQuestion,
    currentStatus: GameStatus.WaitingForPlayers,
    // UI-specific
    successfullyJoined: false,
    playerAnswers: [] as PlayerAnswer[],
    roundResults: [] as RoundResults[],
    gameResults: {} as GameResults,
    gambleResults: {} as GameGamble,
    hasPlayerAction: false,
    showImageLoader: false,
    currentBonusPoints: 0,
    isGuessLocked: true
  }),
  getters: {
    gambler: (state) => {
      const gambler = state.players.find((p) => p.id === state.gambleResults?.gamblerId)
      if (gambler) return gambler
      return {
        id: '',
        connectionId: '',
        username: 'Player Left',
        avatar: ''
      } as Player
    },
    gamblePlayer: (state) => {
      const player = state.players.find((p) => p.id === state.gambleResults?.playerId)
      if (player) return player
      return {
        id: '',
        connectionId: '',
        username: 'Player Left',
        avatar: ''
      } as Player
    },
    currentTheme: (state) => state.currentQuestion.theme,
    currentImageUrl: (state) => state.currentQuestion.imageUrl,
    currentPrompt: (state) => state.currentQuestion.originalPrompt,
    askingGamble: (state) => state.currentStatus === GameStatus.AskingGamble,
    shouldShowResults: (state) => state.currentStatus === GameStatus.ShowingRoundResults,
    shouldShowGambleResults: (state) => state.currentStatus === GameStatus.ShowingGambleResults,
    askingImagePrompt: (state) => state.currentStatus === GameStatus.AskingImagePrompt,
    areThemesSelectable: (state) => state.hasPlayerAction && state.themes.length > 0,
    areThemesVisible: (state) => !state.hasPlayerAction && state.themes.length > 0,
    showGameCanvas: (state) =>
      state.currentStatus === GameStatus.AskingQuestion && state.currentQuestion.imageUrl !== '',
    gameEnded: (state) =>
      state.currentStatus === GameStatus.Completed || state.currentStatus === GameStatus.Abandoned,
    playerPromptMode: (state) => state.gameConfiguration.playerPromptMode,
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
      this.clearQuestion()
      this.showImageLoader = false
      this.currentBonusPoints = 0
      this.isGuessLocked = true
      this.successfullyJoined = true
    },

    changeGameConfiguration(config: IGameConfiguration) {
      Object.assign(this.gameConfiguration, config)
    },

    setPlayerPromptMode(mode: boolean) {
      this.gameConfiguration.playerPromptMode = mode
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
      console.log('theme selected', theme)
    },
    handlePlayerAnsweredEvent(playerAnswer: PlayerAnswer) {
      if (!this.playerAnswers.includes(playerAnswer)) {
        this.playerAnswers.push(playerAnswer)
      }
    },
    handlePlayerGambledEvent(gamble: GameGamble) {
      const gambler = this.players.find((p) => p.id === gamble.gamblerId)
      console.info('received gamble event', gambler?.username)
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
    prepareForPlayerImagePrompt() {
      this.currentStatus = GameStatus.AskingImagePrompt
      this.hasPlayerAction = true
    },
    prepareForPlayerGamble(question: GameQuestion) {
      this.currentStatus = GameStatus.AskingGamble
      Object.assign(this.currentQuestion, question)
      this.hasPlayerAction = true
    },
    prepareForThemeSelection(themes: string[]) {
      this.currentStatus = GameStatus.AskingTheme
      this.hasPlayerAction = true
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
      this.showImageLoader = false
      this.themes = themes
    },
    prepareForQuestion(question: PlayerQuestion) {
      this.currentStatus = GameStatus.AskingQuestion
      this.showImageLoader = false
      this.isGuessLocked = false
      Object.assign(this.currentQuestion, question)
      this.currentRound = question.roundNumber
    },
    handleBroadcastRoundResultsEvent(roundResult: RoundResults) {
      this.currentStatus = GameStatus.ShowingRoundResults
      this.showImageLoader = false
      this.roundResults.push(roundResult)
    },
    handleBroadcastGambleResultsEvent(newResults: GameGamble) {
      this.currentStatus = GameStatus.ShowingGambleResults
      this.showImageLoader = false
      Object.assign(this.gambleResults, newResults)
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
    clearQuestion() {
      this.currentQuestion = {
        id: '',
        playerGenerated: false,
        playerId: '',
        roundNumber: 0,
        theme: '',
        originalPrompt: '',
        imageUrl: '',
        createdAt: ''
      }
    },
    setGuessLock(locked: boolean) {
      this.isGuessLocked = locked
    },
    clearBonusPointsDisplay() {
      this.currentBonusPoints = 0
    },
    playerSubmittedPrompt() {
      this.currentStatus = GameStatus.StartingRound
    },
    playerSelectedTheme(theme: string) {
      // Apply selected theme to currentQuestion and clear theme options
      this.currentQuestion.theme = theme
      this.themes = []
    },
    handleEndGameEvent() {
      this.currentStatus = GameStatus.Completed
      this.clearQuestion()
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
