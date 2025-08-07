// src/services/gameEventHandlers.ts
import service from '@/services/signalRService'
import { usePlayerStore } from '@/stores/player'
import { useNotificationStore } from '@/stores/notifications'
import { useRoomJoinStore } from '@/stores/roomJoin'
import { useGameStateStore } from '@/stores/gameState'
import { useAudioStore } from '@/stores/audio'

import type { Player } from '@/models/player'
import type {
  PlayerAnswer,
  GameState,
  RoundResults,
  GameResults,
  GameGamble,
  IGameConfiguration
} from '@/models/gameModels'

// Helper to easily access stores within handlers
const getStores = () => ({
  playerStore: usePlayerStore(),
  roomJoinStore: useRoomJoinStore(),
  notificationStore: useNotificationStore(),
  gameStateStore: useGameStateStore(),
  volumeStore: useAudioStore()
})

export function registerBaseGameHubEvents() {
  const stores = getStores()

  service.on('playerJoined', (player: Player) => {
    if (stores.playerStore.player && player.id !== stores.playerStore.player.id) {
      stores.notificationStore.addGameNotification(`${player.username} has joined the game!`)
    }
    stores.gameStateStore.addPlayer(player)
  })

  service.on('playerLeft', (player: Player) => {
    stores.notificationStore.addGameNotification(`${player.username} has left the game.`)
    stores.gameStateStore.removePlayer(player)
  })

  service.on('successfullyJoined', (connectionId: string, gameState: GameState) => {
    console.log('Successfully joined the game with connection ID:', connectionId)
    stores.notificationStore.addGameNotification('Welcome to the party!')
    stores.playerStore.updateConnectionId(connectionId)
    stores.gameStateStore.initializeGameState(gameState)
  })

  service.on('broadcastGameConfigurationChange', (config: IGameConfiguration) => {
    stores.gameStateStore.changeGameConfiguration(config)
  })

  service.on('gameStarted', async (gameState: GameState) => {
    stores.notificationStore.addGameNotification('Game has started!')
    stores.gameStateStore.initializeGameState(gameState)
    stores.gameStateStore.startGame()
    // Load and shuffle background music when game starts
    await stores.volumeStore.loadBackgroundMusic()
    stores.volumeStore.shuffleMusic()
    stores.volumeStore.togglePlayMusic(true)
  })

  service.on('roundStarted', (roundNumber: number) => {
    stores.notificationStore.addGameNotification(`Round ${roundNumber} has started!`)
    stores.gameStateStore.startRound(roundNumber)
  })

  service.on('roundResults', (roundResult: RoundResults) => {
    stores.gameStateStore.handleBroadcastRoundResultsEvent(roundResult)
  })

  service.on('gambleResults', (gambleResults: GameGamble) => {
    console.log('Gamble results received:', gambleResults)
    stores.gameStateStore.handleBroadcastGambleResultsEvent(gambleResults)
  })

  service.on('broadcastFinalResults', (results: GameResults) => {
    stores.notificationStore.addGameNotification('The results are in!!!')
    stores.gameStateStore.handleBroadcastFinalResultsEvent(results)
    stores.gameStateStore.handleEndGameEvent()
  })

  service.on('writeMessage', (message: string) => {
    stores.notificationStore.addGameNotification(message)
  })

  service.on('themeSelection', (themes: string[]) => {
    stores.gameStateStore.handleThemeSelectionEvent(themes)
  })

  service.on('themeSelected', (theme: string) => {
    stores.gameStateStore.handleThemeSelectedEvent(theme)
    stores.notificationStore.addGameNotification('Selected theme: ' + theme)
  })

  service.on('playerAnswered', (playerAnswer: PlayerAnswer) => {
    stores.gameStateStore.handlePlayerAnsweredEvent(playerAnswer)
  })

  service.on('playerGambled', (gamble: GameGamble) => {
    stores.gameStateStore.handlePlayerGambledEvent(gamble)
  })

  service.on('awardBonusPoints', (points: number) => {
    stores.gameStateStore.handleAwardBonusPointsEvent(points)
  })

  service.on('navigateBackToLobby', () => {
    stores.gameStateStore.handleNavigateBackToLobbyEvent()
  })
}

export function unregisterBaseGameHubEvents() {
  service.off('playerJoined')
  service.off('playerLeft')
  service.off('successfullyJoined')
  service.off('gameStarted')
  service.off('broadcastFinalResults')
  service.off('writeMessage')
  service.off('themeSelection')
  service.off('themeSelected')
  service.off('roundResults')
  service.off('gambleResults')
  service.off('awardBonusPoints')
}
