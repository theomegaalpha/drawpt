import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import type { Room } from '@/models/room'
import type { Player } from '@/models/player'
import type { GameState } from '@/models/gameModels'

export const useRoomStore = defineStore('room', () => {
  const room = ref({
    id: '',
    code: '',
    name: '',
    playerLimit: 8,
    players: [],
    isGameStarted: false,
    isPrivate: false,
    currentRound: 0,
    totalRounds: 7
  } as Room)

  const players = computed((): Player[] => room.value.players || [])
  const successfullyJoined = ref(false)

  function setSuccessfullyJoined(success: boolean) {
    successfullyJoined.value = success
  }

  function updateRoomCode(code: string) {
    room.value.code = code
  }

  function addPlayer(player: Player) {
    room.value.players.push(player)
  }

  function removePlayer(player: Player) {
    const index = room.value.players.findIndex((p) => p.id === player.id)
    if (index !== -1) {
      room.value.players.splice(index, 1)
    }
  }

  function startGame() {
    room.value.isGameStarted = true
  }

  function clearRoom() {
    room.value.id = ''
    room.value.code = ''
    room.value.name = ''
    room.value.playerLimit = 8
    room.value.players = []
    room.value.isGameStarted = false
    room.value.isPrivate = false
    room.value.currentRound = 0
    room.value.totalRounds = 7
    successfullyJoined.value = false
  }

  return {
    setSuccessfullyJoined,
    updateRoomCode,
    clearRoom,
    startGame,
    addPlayer,
    removePlayer,
    room,
    players,
    successfullyJoined
  }
})
