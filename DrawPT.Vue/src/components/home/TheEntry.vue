<script setup lang="ts">
import api from '@/services/api'
import { computed, onMounted, ref } from 'vue'
import { useMsal } from '@/auth/useMsal'
import { useRouter } from 'vue-router'
import { useRoomStore } from '@/stores/room'

const { accounts } = useMsal()
const { clearRoom, updateRoomCode } = useRoomStore()
const roomCode = ref('')
const guess = ref('')
const showTooltip = ref(false)

const submitGuess = () => {
  if (guess.value.trim()) {
    console.log('Submitted guess:', guess.value)
    // Here you would typically send the guess to your backend
    guess.value = ''
  }
}

const isAuthenticated = computed(() => accounts.value.length > 0)

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
  <main class="flex h-screen w-full flex-col items-center justify-center">
    <div class="mb-6 overflow-hidden rounded-lg bg-white shadow-md">
      <div class="bg-indigo-600 p-4 text-white">
        <h2 class="text-lg font-medium">Guess Today's Prompt!</h2>
        <p class="text-sm opacity-80">Guess what the AI has drawn!</p>
      </div>

      <!-- AI Drawing Display -->
      <div class="bg-gray-100 p-2">
        <div
          class="flex aspect-square items-center justify-center overflow-hidden rounded-md border border-gray-200 bg-white"
        >
          <img
            src="https://picsum.photos/id/237/600/600"
            alt="AI Drawing"
            class="max-h-full max-w-full object-contain"
          />
        </div>
      </div>

      <!-- Guess Input -->
      <div class="p-4">
        <form @submit.prevent="submitGuess" class="flex gap-2">
          <Input v-model="guess" placeholder="Enter your guess..." class="flex-1" />
          <button
            class="rounded border border-zinc-200 bg-zinc-50 px-4 py-2 shadow hover:bg-zinc-100 dark:border-zinc-700 dark:bg-zinc-800 dark:text-zinc-100 dark:hover:bg-zinc-700"
            type="submit"
          >
            Submit
          </button>
        </form>
      </div>
    </div>

    <div class="mb-6 rounded-lg bg-white p-4 shadow-md dark:bg-zinc-800">
      <h2 class="mb-2 text-xl font-bold text-indigo-600 dark:text-indigo-300">How to Play</h2>
      <p class="mb-4 text-zinc-700 dark:text-zinc-200">
        AI is the artist! Guess what it drew based on unique and tricky prompts. Test your skills
        and have fun!
      </p>

      <div class="relative">
        <button
          class="w-full rounded-md border-2 border-none bg-blue-500 px-4 py-2 font-semibold text-white hover:bg-blue-600 disabled:bg-blue-300 disabled:text-zinc-400 dark:bg-blue-600 dark:text-white dark:hover:bg-blue-700 dark:disabled:bg-zinc-600 dark:disabled:text-zinc-400"
          disabled
          @mouseenter="showTooltip = true"
          @mouseleave="showTooltip = false"
          @focus="showTooltip = true"
          @blur="showTooltip = false"
        >
          Join Lobby
        </button>
        <div
          v-if="showTooltip"
          class="absolute bottom-full left-1/2 mb-2 -translate-x-1/2 transform rounded bg-gray-800 px-3 py-1 text-sm text-white shadow-lg"
        >
          Coming soon!
        </div>
      </div>
    </div>
    <div
      v-if="isAuthenticated"
      class="flex-item mt-4 rounded-lg border border-zinc-200 bg-white p-6 shadow dark:border-zinc-700 dark:bg-zinc-800"
    >
      <button
        class="flex-item w-40 rounded-md border-2 border-none bg-blue-500 px-4 py-2 font-semibold text-white hover:bg-blue-600 disabled:bg-blue-300 disabled:text-zinc-400 dark:bg-blue-600 dark:text-white dark:hover:bg-blue-700 dark:disabled:bg-zinc-600 dark:disabled:text-zinc-400"
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
          class="flex-item mb-2 w-40 rounded-md border-2 border-zinc-300 bg-white px-4 py-2 text-black dark:border-zinc-600 dark:bg-zinc-700 dark:text-white"
          placeholder="Room Code"
          maxlength="4"
          v-model="roomCode"
          v-autocapitalize="true"
          @keyup.enter="roomCode.length === 4 ? joinRoom(roomCode) : null"
        />
        <button
          class="flex-item w-40 rounded-md border-2 border-none bg-blue-500 px-4 py-2 font-semibold text-white hover:bg-blue-700 disabled:bg-blue-300 disabled:text-zinc-400 dark:bg-blue-600 dark:text-white dark:hover:bg-blue-700 dark:disabled:bg-zinc-600 dark:disabled:text-zinc-400"
          :disabled="roomCode.length < 4"
          @click="joinRoom(roomCode)"
        >
          Join
        </button>
      </div>
    </div>
  </main>
</template>
