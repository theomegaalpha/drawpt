<script setup lang="ts">
import api from '@/services/api'
import { computed, onMounted, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router' // Added useRoute
import { useRoomStore } from '@/stores/room'
import { useAuthStore } from '@/stores/auth'
import { RouterLink } from 'vue-router'
import GuessInput from '../common/GuessInput.vue'
import StandardInput from '../common/StandardInput.vue'
import Leaderboard from './leaderboard/Leaderboard.vue'

const { clearRoom, updateRoomCode } = useRoomStore()
const roomCodeInput = ref<string>('')
const guess = ref('')
const showTooltip = ref(false)

const submitGuess = () => {
  if (guess.value.trim()) {
    console.log('Submitted guess:', guess.value)
    guess.value = ''
  }
}

const authStore = useAuthStore()
const isAuthenticated = computed(() => {
  return !!authStore.user
})

const router = useRouter()
const route = useRoute() // Added to access current route

const joinRoom = (codeToJoin: string) => {
  if (!codeToJoin || codeToJoin.trim().length === 0) {
    console.warn('Attempted to join with an empty room code.')
    // Optionally, add a user notification here
    return
  }
  const upperCaseCode = codeToJoin.toUpperCase()
  updateRoomCode(upperCaseCode)
  // Navigate to the 'room' route, passing roomCode as a parameter.
  // Assumes 'room' route is defined as /room/:roomCode
  router.push({ name: 'room', params: { roomCode: upperCaseCode } })
}

const createRoom = () => {
  if (!isAuthenticated.value) {
    router.push({ name: 'login' })
    return
  }

  api.createRoom().then((newlyCreatedCode) => {
    if (newlyCreatedCode) {
      joinRoom(newlyCreatedCode)
    } else {
      console.error('Failed to create room: No room code received from API.')
      // Optionally, add a user notification here
    }
  })
}

const scrollToPrompt = () => {
  const element = document.getElementById('prompt-of-the-day')
  if (element) {
    element.scrollIntoView({ behavior: 'smooth' })
  }
}

onMounted(() => {
  clearRoom()
  const queryRoomCode = route.query.roomCode as string
  if (queryRoomCode) {
    // If a roomCode is present in the URL query, attempt to join it automatically
    joinRoom(queryRoomCode)
  }
})
</script>

<template>
  <main class="container mx-auto px-6 py-8">
    <!-- Hero Section -->
    <div class="mb-10 animate-blur-in px-6 py-16 text-center">
      <h1 class="text-color-accent mb-4 text-4xl font-bold md:text-5xl">AI Draws, You Decipher</h1>
      <p class="text-color-default mx-auto max-w-3xl text-lg">
        AI-powered Pictionary where you decode abstract creations based on complex phrases.
      </p>
      <p class="text-color-default mx-auto mb-8 max-w-2xl text-lg font-semibold">
        Can you beat the algorithm?
      </p>
      <div>
        <button class="btn-default text-lg font-semibold shadow-md" @click="scrollToPrompt">
          Daily Challenge
        </button>
        <RouterLink
          class="btn-default ml-2 text-lg font-semibold shadow-md"
          :to="{ name: 'game-selection' }"
        >
          Play Game
        </RouterLink>
      </div>
    </div>
    <div id="prompt-of-the-day" class="mb-10 grid grid-cols-1 gap-8 lg:grid-cols-2">
      <div class="bg-surface-default rounded-xl p-6 shadow-md">
        <div class="flex items-baseline gap-x-2">
          <h2 class="text-xl font-bold">Guess Today's Prompt</h2>
          <span class="text-lg">Hint: Dark</span>
        </div>
        <div class="prose prose-indigo dark:prose-invert text-color-default mt-2">
          <div
            class="overflow-hidden rounded-lg border border-gray-200 bg-gray-50 dark:border-zinc-700 dark:bg-zinc-900"
          >
            <img
              src="https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f112667271fdad06396cdb_QDhk9GJWfYfchRCbp8kTMay1FxyeMGxzHkB7IMd3Cfo.webp"
              alt="AI Drawing"
              class="h-auto w-full object-contain"
            />
          </div>
          <GuessInput class="mt-4" v-model="guess" :submitAction="submitGuess" />
        </div>
      </div>

      <!-- Right column wrapper for Game Options and Leaderboard -->
      <div class="flex flex-col gap-8">
        <div
          class="bg-surface-default flex flex-grow flex-col overflow-hidden rounded-xl shadow-md"
        >
          <div class="p-4">
            <h2 class="text-xl font-medium">Leaderboard</h2>
          </div>
          <Leaderboard class="flex-grow" />
        </div>
        <div class="bg-surface-default rounded-xl p-6 shadow-md">
          <h2 class="mb-4 text-xl font-bold">Game Options</h2>
          <div class="space-y-3">
            <div class="relative">
              <button
                class="btn-default w-full"
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
                class="absolute bottom-full left-1/2 mb-2 -translate-x-1/2 transform whitespace-nowrap rounded-md border border-black/10 bg-gray-50 px-3 py-2 text-sm text-black shadow-lg dark:border-white/10 dark:bg-black dark:text-gray-200"
              >
                Coming soon!
              </div>
            </div>
            <div class="relative">
              <button class="btn-default w-full" @click="createRoom()">Create Room</button>
            </div>

            <div class="relative flex w-full">
              <StandardInput
                placeholder="Room Code"
                maxlength="4"
                :autocapitalize="true"
                v-model="roomCodeInput"
                @keyup.enter="roomCodeInput.length === 4 ? joinRoom(roomCodeInput) : null"
              />
              <button
                class="btn-default ml-2 flex rounded-full px-6"
                :disabled="roomCodeInput.length < 4"
                @click="joinRoom(roomCodeInput)"
              >
                Join
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- Features Section -->
    <div class="bg-surface-default mb-10 rounded-xl p-8 shadow-md">
      <h2 class="mb-6 text-center text-2xl font-bold">Game Features</h2>
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
          <h3 class="text-color-default mb-2 text-lg font-bold">AI-Generated Art</h3>
          <p class="text-color-muted">Unique drawings created by advanced AI algorithms</p>
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
          <h3 class="text-color-default mb-2 text-lg font-bold">Multiplayer Mode</h3>
          <p class="text-color-muted">Compete with friends to see who can guess fastest</p>
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
          <h3 class="text-color-default mb-2 text-lg font-bold">Creative Prompts</h3>
          <p class="text-color-muted">Challenging and unique concepts to test your skills</p>
        </div>
      </div>
    </div>
  </main>
</template>
