<script setup lang="ts">
import Avatar from '@/components/common/Avatar.vue'
import { usePlayerStore } from '@/stores/player'
import { useRoomStore } from '@/stores/room'
import service from '@/services/signalRService'
import { ref } from 'vue' // Import ref

const { player } = usePlayerStore()
const { room } = useRoomStore()
const isCopied = ref(false) // Reactive variable for feedback

const startGame = async () => {
  var gameStarted = await service.invoke('startGame')
  if (!gameStarted) {
    console.error('Game could not be started')
  }
}

const copyRoomCodeToClipboard = () => {
  if (room && room.code) {
    navigator.clipboard
      .writeText(room.code)
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
  <main
    class="grid min-h-screen grid-cols-1 items-center justify-center gap-3 space-x-4 p-10 md:grid-cols-2"
  >
    <div class="col-span-1 p-10 text-center">
      <h1 class="mb-5 text-4xl font-bold">DrawPT</h1>
      <h2 class="mb-3 text-2xl">Welcome, {{ player.username }}</h2>
      <p class="mb-2 text-lg">Waiting for other players to join...</p>
      <button
        v-if="room.players.length > 0 && room.players[0].id === player.id"
        class="flex-item my-5 w-40 rounded-md border-2 border-none bg-blue-500 px-4 py-2 font-semibold text-white hover:bg-blue-600 disabled:bg-blue-300 disabled:text-zinc-400 dark:bg-blue-600 dark:text-zinc-800 dark:hover:bg-blue-700 dark:disabled:bg-blue-300"
        @click="startGame"
      >
        Start Game
      </button>
      <h2
        @click="copyRoomCodeToClipboard"
        class="mx-auto w-fit cursor-pointer rounded-xl bg-zinc-800/10 px-4 py-2 text-2xl font-bold text-blue-400 transition-all hover:bg-zinc-800/20 dark:bg-white/10 dark:text-blue-300 dark:hover:bg-white/20"
        :class="{ 'font-semibold text-green-500 dark:text-green-400': isCopied }"
      >
        {{ isCopied ? 'Copied!' : room.code }}
      </h2>
    </div>
    <div class="col-span-1 px-5 md:px-20 xl:px-32">
      <h2 class="mb-5 text-center text-2xl font-bold">Players</h2>
      <div
        class="animate-slide-in mt-2 flex flex-row items-center rounded-lg border border-zinc-200 bg-white p-2 shadow dark:border-zinc-700 dark:bg-zinc-800"
        v-for="p in room.players"
        :key="p.id"
      >
        <Avatar :username="p.username" :color="p.color" />
        <p class="ml-2 xl:ml-4">{{ p.username }}</p>
      </div>
      <div
        class="mt-2 flex flex-row items-center rounded-lg border border-zinc-200 bg-zinc-100 p-2 shadow dark:border-zinc-600 dark:bg-zinc-700"
        v-for="p in 8 - room.players.length"
        :key="p"
      >
        <div
          class="flex h-10 w-10 cursor-default items-center justify-center rounded-full bg-slate-200 font-semibold text-white dark:bg-slate-400"
        ></div>
        <p class="ml-2 xl:ml-4">Open player slot</p>
      </div>
    </div>
  </main>
</template>

<style></style>
