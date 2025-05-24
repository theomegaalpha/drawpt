<script setup lang="ts">
import api from '@/services/api'
import { computed, onMounted, ref } from 'vue'
import { useMsal } from '@/auth/useMsal'
import { useRouter } from 'vue-router'
import { useRoomStore } from '@/stores/room'

const { accounts } = useMsal()
const { clearRoom, updateRoomCode } = useRoomStore()
const roomCode = ref('')

const isAuthenticated = computed(() => accounts.value.length > 0)

const name = computed(() => {
  if (accounts.value.length > 0) {
    const name = accounts.value[0].name
    if (name) {
      return name.split(' ')[0]
    }
  }
  return ''
})

const router = useRouter()
const joinRoom = (code: string) => {
  code = code.toUpperCase()
  updateRoomCode(code)
  router.push(`/room`)
}

const createRoom = () => {
  api.createRoom().then((code) => {
    joinRoom(code)
  })
}

onMounted(() => {
  clearRoom()
})
</script>

<template>
  <div
    class="hero fixed top-20 w-full bg-blue-500 p-10 text-center text-white shadow-md dark:bg-violet-900"
  >
    <h1 v-if="name" class="mb-4 text-4xl">Welcome back, {{ name }}!</h1>
    <h1 v-else class="mb-4 text-4xl">Welcome to the party!</h1>
    <p class="text-xl">A game of pictionary but the artist is AI and the prompt is insane!</p>
  </div>
  <main class="flex h-screen w-full flex-col items-center justify-center">
    <div
      v-if="isAuthenticated"
      class="flex-item mt-4 rounded-lg border border-zinc-200 bg-white p-6 shadow dark:border-zinc-700 dark:bg-zinc-800"
    >
      <button
        class="flex-item w-40 rounded-md border-2 border-none bg-blue-500 px-4 py-2 font-semibold text-white hover:bg-blue-600 disabled:bg-blue-300 disabled:text-zinc-400 dark:bg-blue-600 dark:text-zinc-800 dark:hover:bg-blue-700 dark:disabled:bg-blue-300"
        @click="createRoom()"
      >
        Create Room
      </button>
    </div>
    <div
      class="flex-item mt-4 rounded-lg border border-zinc-200 bg-white p-6 shadow dark:border-zinc-700 dark:bg-zinc-800"
    >
      <div class="flex flex-col">
        <input
          class="flex-item mb-2 w-40 rounded-md border-2 border-zinc-300 px-4 py-2 dark:text-black"
          placeholder="Room Code"
          maxlength="4"
          v-model="roomCode"
          v-autocapitalize="true"
          @keyup.enter="roomCode.length === 4 ? joinRoom(roomCode) : null"
        />
        <button
          class="flex-item w-40 rounded-md border-2 border-none bg-blue-500 px-4 py-2 font-semibold text-white hover:bg-blue-600 disabled:bg-blue-300 disabled:text-zinc-400 dark:bg-blue-600 dark:text-zinc-800 dark:hover:bg-blue-700 dark:disabled:bg-blue-300"
          :disabled="roomCode.length < 4"
          @click="joinRoom(roomCode)"
        >
          Join
        </button>
      </div>
    </div>
  </main>
</template>
