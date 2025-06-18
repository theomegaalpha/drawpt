import { ref } from 'vue'
import { defineStore } from 'pinia'
import type { Player } from '@/models/player'
import { avatarColors } from '@/components/constants/colors'
import api from '@/api/api'

export const usePlayerStore = defineStore('player', () => {
  const player = ref({
    id: '',
    connectionId: '',
    color: '',
    username: '',
    avatar: ''
  } as Player)
  const avatarOptions = ref<string[]>([
    '/images/profile-photos/animal-1.png',
    '/images/profile-photos/animal-2.png',
    '/images/profile-photos/animal-3.png',
    '/images/profile-photos/animal-4.png',
    '/images/profile-photos/animal-5.png',
    '/images/profile-photos/animal-6.png',
    '/images/profile-photos/anime-1.png',
    '/images/profile-photos/anime-2.png',
    '/images/profile-photos/anime-3.png',
    '/images/profile-photos/anime-4.png',
    '/images/profile-photos/anime-5.png',
    '/images/profile-photos/anime-6.png',
    '/images/profile-photos/anime-7.png',
    '/images/profile-photos/anime-8.png',
    '/images/profile-photos/anime-9.png',
    '/images/profile-photos/anime-10.png'
  ])
  const blankAvatar = ref<string>('/images/profile-photos/blank.png')

  function updatePlayer(playerData: Player) {
    Object.assign(player.value, playerData)
  }

  function randomizeColor() {
    player.value.color = avatarColors[Math.floor(Math.random() * avatarColors.length)]
  }

  function updateConnectionId(connectionId: string) {
    player.value.connectionId = connectionId
  }

  // Action to specifically update the username
  function setUsername(newUsername: string) {
    if (player.value) {
      player.value.username = newUsername
    }
  }

  async function init() {
    if (!player.value.id) {
      try {
        const playerData = await api.getPlayer()
        updatePlayer(playerData)
      } catch (error) {
        console.error('Failed to initialize player:', error)
      }
    }
  }

  if (!player.value.id) {
    init()
  }

  return {
    player,
    blankAvatar,
    avatarOptions,
    updateConnectionId,
    randomizeColor,
    updatePlayer,
    setUsername,
    init
  }
})
