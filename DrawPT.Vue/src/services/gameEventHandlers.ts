// src/services/gameEventHandlers.ts
import service from '@/services/signalRService'
import { usePlayerStore } from '@/stores/player'
import { useRoomStore } from '@/stores/room'
import { useScoreboardStore } from '@/stores/scoreboard'
import { useNotificationStore } from '@/stores/notifications'
import { useRoomJoinStore } from '@/stores/roomJoinStore'
import { useGameStateStore } from '@/stores/gameState'

import type { Player } from '@/models/player'
import type { GameState, RoundResults } from '@/models/gameModels' // Assuming GameRound type exists or will be defined

// Helper to easily access stores within handlers
const getStores = () => ({
  playerStore: usePlayerStore(),
  roomStore: useRoomStore(),
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
    stores.roomStore.addPlayer(player)
  })

  service.on('playerLeft', (player: Player) => {
    stores.notificationStore.addGameNotification(`${player.username} has left the game.`)
    stores.roomStore.removePlayer(player)
  })

  service.on('successfullyJoined', (connectionId: string) => {
    console.log('Successfully joined the game with connection ID:', connectionId)
    stores.notificationStore.addGameNotification('Welcome to the party!')
    stores.playerStore.updateConnectionId(connectionId)
    stores.roomStore.setSuccessfullyJoined(true)
  })

  service.on('gameStarted', (gameState: GameState) => {
    stores.roomStore.startGame()
    stores.notificationStore.addGameNotification('Game has started!')
    // Potentially update gameStateStore with initial game state if provided
    // stores.gameStateStore.initializeGameState(gameState);
  })

  service.on('roundEnded', (roundResult: RoundResults) => {
    stores.notificationStore.addGameNotification(`Round ${roundResult.roundNumber} has ended!`)
    stores.scoreboardStore.addRoundResult(roundResult)
    stores.gameStateStore.handleBroadcastRoundResultsEvent(roundResult)
  })

  service.on('broadcastFinalResults', (results: any) /* Specify type for results */ => {
    stores.notificationStore.addGameNotification('The results are in!!!')
    stores.scoreboardStore.updateGameResults(results)
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
  service.off('broadcastRoundResults')
  service.off('awardBonusPoints')
}
