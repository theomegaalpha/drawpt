// src/services/gameEventHandlers.ts
import service from '@/services/signalRService'
import { usePlayerStore } from '@/stores/player'
import { useScoreboardStore } from '@/stores/scoreboard'
import { useNotificationStore } from '@/stores/notifications'
import { useRoomJoinStore } from '@/stores/roomJoin'
import { useGameStateStore } from '@/stores/gameState'

import type { Player } from '@/models/player'
import type { GameState, RoundResults, GameResults } from '@/models/gameModels'

// Helper to easily access stores within handlers
const getStores = () => ({
  playerStore: usePlayerStore(),
  roomJoinStore: useRoomJoinStore(),
  scoreboardStore: useScoreboardStore(),
  notificationStore: useNotificationStore(),
  gameStateStore: useGameStateStore()
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

  service.on('gameStarted', (gameState: GameState) => {
    stores.notificationStore.addGameNotification('Game has started!')
    stores.gameStateStore.initializeGameState(gameState)
    stores.gameStateStore.startGame()
  })

  service.on('roundStarted', (roundNumber: number) => {
    stores.notificationStore.addGameNotification(`Round ${roundNumber} has started!`)
    stores.gameStateStore.startRound(roundNumber)
  })

  service.on('roundResults', (roundResult: RoundResults) => {
    console.log('Round results received:', roundResult)
    stores.notificationStore.addGameNotification(`Round ${roundResult.roundNumber} has ended!`)
    stores.scoreboardStore.addRoundResult(roundResult)
    stores.gameStateStore.handleBroadcastRoundResultsEvent(roundResult)
  })

  service.on('broadcastFinalResults', (results: GameResults) => {
    stores.notificationStore.addGameNotification('The results are in!!!')
    stores.scoreboardStore.updateGameResults(results)
    stores.gameStateStore.handleEndGameEvent()
  })

  service.on('writeMessage', (message: string) => {
    stores.notificationStore.addGameNotification(message)
  })

  // Non-interactive events that update gameStateStore
  service.on('themeSelection', (themes: string[]) => {
    stores.gameStateStore.handleThemeSelectionEvent(themes)
  })

  service.on('themeSelected', (theme: string) => {
    // Server confirms a theme was selected by someone
    stores.gameStateStore.handleThemeSelectedEvent(theme)
    stores.notificationStore.addGameNotification('Selected theme: ' + theme)
  })

  service.on('broadcastRoundResults', (gameRound: RoundResults) /* Specify type */ => {
    stores.scoreboardStore.addRoundResult(gameRound)
    stores.gameStateStore.handleBroadcastRoundResultsEvent(gameRound)
  })

  service.on('awardBonusPoints', (points: number) => {
    stores.gameStateStore.handleAwardBonusPointsEvent(points)
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
  service.off('broadcastRoundResults')
  service.off('awardBonusPoints')
}
