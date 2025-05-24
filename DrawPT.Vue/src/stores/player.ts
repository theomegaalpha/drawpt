import { ref } from 'vue'
import { defineStore } from 'pinia'
import type { Player } from '@/models/player'
import { avatarColors } from '@/components/constants/colors'

export const usePlayerStore = defineStore('player', () => {
  const player = ref({
    id: '',
    connectionId: '',
    color: '',
    username: ''
  } as Player)

  function updatePlayer(playerData: Player) {
    Object.assign(player.value, playerData)
  }

  function randomizeColor() {
    player.value.color = avatarColors[Math.floor(Math.random() * avatarColors.length)]
  }

  function updateConnectionId(connectionId: string) {
    player.value.connectionId = connectionId
  }

  return { updateConnectionId, updatePlayer, randomizeColor, player }
})
