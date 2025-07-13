<script setup lang="ts">
import { usePlayerStore } from '@/stores/player'
import { useGameStateStore } from '@/stores/gameState'
import service from '@/services/signalRService'
import { ref } from 'vue'
import ShinyButton from '@/components/common/ShinyButton.vue'

const { player } = usePlayerStore()
const { players, roomCode } = useGameStateStore()
const isCopied = ref(false)
const isLoading = ref(false)

const startGame = async () => {
  if (isLoading.value) return

  isLoading.value = true
  var gameStarted = await service.invoke('startGame')
  if (!gameStarted) {
    console.error('Game could not be started')
  }
  isLoading.value = false
}

const copyRoomCodeToClipboard = () => {
  if (roomCode) {
    navigator.clipboard
      .writeText(roomCode)
      .then(() => {
        isCopied.value = true
        setTimeout(() => {
          isCopied.value = false
        }, 3000)
      })
      .catch((err) => {
        console.error('Failed to copy room code: ', err)
      })
  }
}
</script>

<template>
  <div class="col-span-1 p-10 text-center">
    <h1 class="mb-5 text-4xl font-bold">DrawPT</h1>
    <h2 class="mb-2 text-2xl">Welcome, {{ player.username }}</h2>
    <p class="mb-2 text-lg">Waiting for other players to join...</p>
    <ShinyButton
      v-if="players.length > 0 && players[0].id === player.id"
      class="my-5 px-10"
      :class="{ 'cursor-progress': isLoading }"
      :isPrimary="true"
      @click="startGame"
    >
      <Loader2Icon v-if="isLoading" class="mr-4 h-5 w-5 animate-spin" />
      <span class="text-indigo-700 dark:text-indigo-300" v-else>Start Game</span>
    </ShinyButton>
    <h2
      @click="copyRoomCodeToClipboard"
      class="mx-auto w-fit cursor-pointer rounded-xl bg-zinc-800/10 px-4 py-2 text-2xl font-bold text-blue-400 transition-all hover:bg-zinc-800/20 dark:bg-white/10 dark:text-blue-300 dark:hover:bg-white/20"
      :class="{ 'font-semibold text-green-500 dark:text-green-400': isCopied }"
    >
      {{ isCopied ? 'Copied!' : roomCode }}
    </h2>
  </div>
</template>
