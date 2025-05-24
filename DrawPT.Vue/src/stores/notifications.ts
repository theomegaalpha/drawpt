import { ref } from 'vue'
import { defineStore } from 'pinia'
import { type GameNotification } from '@/models/gameNotification'

export const useNotificationStore = defineStore('notification', () => {
  const gameNotifications = ref([] as GameNotification[])

  function addGameNotification(message: string, isAlert: boolean = false) {
    gameNotifications.value.push({ message, isAlert })
    setTimeout(() => {
      gameNotifications.value.shift()
    }, 7000)
  }

  return { gameNotifications, addGameNotification }
})
