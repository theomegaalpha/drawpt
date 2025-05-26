<script setup lang="ts">
import api from '@/services/api'
import { computed, onMounted, ref } from 'vue'
import { useMsal } from '@/auth/useMsal'
import { useRouter } from 'vue-router'
import { useRoomStore } from '@/stores/room'
import StandardInput from '../common/StandardInput.vue' // Corrected import path

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
  <main class="container mx-auto px-6 py-8">
    <!-- Game Interface - Web Layout -->
    <div class="mb-10 grid grid-cols-1 gap-8 lg:grid-cols-5">
      <!-- Left Column - Drawing Display -->
      <div class="overflow-hidden rounded-xl bg-white shadow-md lg:col-span-3 dark:bg-zinc-800">
        <div class="bg-indigo-600 p-4 text-white dark:bg-indigo-500">
          <h2 class="text-xl font-medium">Current Drawing</h2>
          <p class="text-sm opacity-80">Guess what the AI has drawn!</p>
        </div>

        <div class="p-6">
          <div
            class="overflow-hidden rounded-lg border border-gray-200 bg-gray-50 dark:border-zinc-700 dark:bg-zinc-900"
          >
            <img
              src="https://picsum.photos/id/237/800/800"
              alt="AI Drawing"
              class="h-auto w-full object-contain"
            />
          </div>
        </div>
      </div>

      <!-- Right Column - Game Controls -->
      <div class="flex flex-col gap-6 lg:col-span-2">
        <!-- Guess Input -->
        <div class="rounded-xl bg-white p-6 shadow-md dark:bg-zinc-800">
          <h2 class="mb-4 text-xl font-bold text-indigo-600 dark:text-indigo-400">
            Make Your Guess
          </h2>
          <form @submit.prevent="submitGuess" class="space-y-4">
            <div>
              <StandardInput id="guess" v-model="guess" placeholder="Enter your guess..." />
            </div>
            <button
              type="submit"
              class="w-full rounded-md bg-indigo-600 px-4 py-2 font-semibold text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 dark:bg-indigo-500 dark:hover:bg-indigo-600 dark:focus:ring-offset-zinc-800"
            >
              Submit Guess
            </button>
          </form>
        </div>

        <!-- Game Actions -->
        <div class="rounded-xl bg-white p-6 shadow-md dark:bg-zinc-800">
          <h2 class="mb-4 text-xl font-bold text-indigo-600 dark:text-indigo-400">Game Options</h2>
          <div class="space-y-3">
            <div class="relative">
              <button
                class="w-full cursor-not-allowed rounded-md bg-gray-200 px-4 py-2 text-gray-400 dark:bg-zinc-700 dark:text-zinc-500"
                disabled
                @mouseenter="showTooltip = true"
                @mouseleave="showTooltip = false"
                @focus="showTooltip = true"
                @blur="showTooltip = false"
              >
                Join Multiplayer Lobby
              </button>
              <div
                v-if="showTooltip"
                class="absolute bottom-full left-1/2 mb-2 -translate-x-1/2 transform whitespace-nowrap rounded-md bg-gray-800 px-3 py-2 text-sm text-white shadow-lg dark:bg-black dark:text-gray-200"
              >
                Coming soon!
              </div>
            </div>
            <div v-if="isAuthenticated" class="relative">
              <button
                class="w-full rounded-md bg-indigo-600 px-4 py-2 font-semibold text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 dark:bg-indigo-500 dark:hover:bg-indigo-600 dark:focus:ring-offset-zinc-800"
                @click="createRoom()"
              >
                Create Room
              </button>
            </div>

            <div class="flex items-center space-x-2">
              <StandardInput
                class="flex-grow"
                placeholder="Room Code"
                maxlength="4"
                v-model="roomCode"
                v-autocapitalize="true"
                @keyup.enter="roomCode.length === 4 ? joinRoom(roomCode) : null"
              />
              <button
                class="w-auto rounded-md bg-indigo-600 px-4 py-2 font-semibold text-white hover:bg-indigo-700 disabled:cursor-not-allowed disabled:bg-indigo-300 dark:bg-indigo-500 dark:hover:bg-indigo-600 dark:disabled:bg-indigo-800 dark:disabled:text-indigo-500"
                :disabled="roomCode.length < 4"
                @click="joinRoom(roomCode)"
              >
                Join
              </button>
            </div>

            <button
              class="w-full rounded-md bg-gray-200 px-4 py-2 font-semibold text-gray-700 hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-400 focus:ring-offset-2 dark:bg-zinc-700 dark:text-gray-200 dark:hover:bg-zinc-600 dark:focus:ring-gray-500 dark:focus:ring-offset-zinc-800"
            >
              View Leaderboard
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Game Explanation & Demo -->
    <div class="mb-10 grid grid-cols-1 gap-8 lg:grid-cols-2">
      <!-- Game Explanation -->
      <div class="rounded-xl bg-white p-6 shadow-md dark:bg-zinc-800">
        <h2 class="mb-4 text-2xl font-bold text-indigo-600 dark:text-indigo-400">How to Play</h2>
        <div class="prose prose-indigo dark:prose-invert">
          <p class="text-lg">
            AI is the artist! Guess what it drew based on unique and tricky prompts. Test your
            skills and challenge yourself!
          </p>
          <ul class="mt-4">
            <li>AI generates a drawing based on a secret prompt</li>
            <li>You have to guess that prompt based on the generated drawing</li>
            <li>Score points for how close you are to the original prompt</li>
            <li>Earn bonus points for being fast</li>
          </ul>
        </div>
      </div>

      <!-- Game Demo -->
      <div class="overflow-hidden rounded-xl bg-white shadow-md dark:bg-zinc-800">
        <div class="bg-indigo-600 p-4 text-white dark:bg-indigo-500">
          <h2 class="text-xl font-medium">See How It Works</h2>
        </div>
        <div class="p-6">
          <div class="overflow-hidden rounded-lg bg-gray-50 dark:bg-zinc-900">
            <img src="https://picsum.photos/id/237/800/450" alt="Game Demo" class="h-auto w-full" />
          </div>
          <p class="mt-4 text-gray-600 dark:text-gray-400">
            Watch as AI generates drawings in real-time and players submit their guesses! The faster
            you guess correctly, the more points you earn.
          </p>
        </div>
      </div>
    </div>

    <!-- Features Section -->
    <div class="mb-10 rounded-xl bg-white p-8 shadow-md dark:bg-zinc-800">
      <h2 class="mb-6 text-center text-2xl font-bold text-indigo-600 dark:text-indigo-400">
        Game Features
      </h2>
      <div class="grid grid-cols-1 gap-6 md:grid-cols-3">
        <div class="p-4 text-center">
          <div
            class="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-indigo-100 dark:bg-indigo-900"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="text-indigo-600 dark:text-indigo-400"
            >
              <path d="M12 17.8 5.8 21 7 14.1 2 9.3l7-1L12 2l3 6.3 7 1-5 4.8 1.2 6.9-6.2-3.2Z" />
            </svg>
          </div>
          <h3 class="mb-2 text-lg font-bold text-gray-800 dark:text-gray-200">AI-Generated Art</h3>
          <p class="text-gray-600 dark:text-gray-400">
            Unique drawings created by advanced AI algorithms
          </p>
        </div>
        <div class="p-4 text-center">
          <div
            class="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-indigo-100 dark:bg-indigo-900"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="text-indigo-600 dark:text-indigo-400"
            >
              <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
              <circle cx="9" cy="7" r="4" />
              <path d="M22 21v-2a4 4 0 0 0-3-3.87" />
              <path d="M16 3.13a4 4 0 0 1 0 7.75" />
            </svg>
          </div>
          <h3 class="mb-2 text-lg font-bold text-gray-800 dark:text-gray-200">Multiplayer Mode</h3>
          <p class="text-gray-600 dark:text-gray-400">
            Compete with friends to see who can guess fastest
          </p>
        </div>
        <div class="p-4 text-center">
          <div
            class="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-indigo-100 dark:bg-indigo-900"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="text-indigo-600 dark:text-indigo-400"
            >
              <path d="M12 20h9" />
              <path d="M16.5 3.5a2.12 2.12 0 0 1 3 3L7 19l-4 1 1-4Z" />
            </svg>
          </div>
          <h3 class="mb-2 text-lg font-bold text-gray-800 dark:text-gray-200">Creative Prompts</h3>
          <p class="text-gray-600 dark:text-gray-400">
            Challenging and unique concepts to test your skills
          </p>
        </div>
      </div>
    </div>
  </main>
</template>
