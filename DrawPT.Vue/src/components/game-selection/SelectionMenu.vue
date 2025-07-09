<script setup lang="ts">
import api from '@/api/api'
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router' // Added useRoute
import { useRoomStore } from '@/stores/room'
import StandardInput from '../common/StandardInput.vue'

import matchmakingBackground from './matchmaking.jpg'

const showTooltip = ref(false)

const { updateRoomCode, clearRoom } = useRoomStore()
const roomCodeInput = ref<string>('')
const router = useRouter()

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
  api.createRoom().then((newlyCreatedCode) => {
    if (newlyCreatedCode) {
      joinRoom(newlyCreatedCode)
    } else {
      console.error('Failed to create room: No room code received from API.')
      // Optionally, add a user notification here
    }
  })
}

onMounted(() => {
  clearRoom()
})
</script>

<template>
  <div class="flex min-h-screen items-center justify-center p-10">
    <div
      class="grid h-96 w-full grid-cols-1 gap-3 md:grid-cols-3 md:items-stretch lg:mx-auto lg:max-w-4xl"
    >
      <!-- matchmaking -->
      <div
        class="relative col-span-1 cursor-not-allowed overflow-hidden rounded-xl shadow-md transition duration-300 ease-in-out hover:shadow-lg"
      >
        <div
          class="absolute inset-0 bg-black/50 bg-cover bg-center bg-blend-darken blur-sm grayscale filter"
          :style="{ backgroundImage: `url(${matchmakingBackground})` }"
        ></div>
        <div class="relative z-10 flex h-full flex-col justify-center p-8 text-center">
          <h2 class="mb-4 rounded-full bg-black/50 py-1 text-xl font-bold text-white">
            Join Matchmaking
          </h2>
          <p class="mx-auto w-fit rounded-full bg-black/50 px-3 py-1 text-white">Coming Soon</p>
        </div>
      </div>
      <!-- create lobby -->
      <div
        class="group relative col-span-1 overflow-hidden rounded-xl shadow-md transition duration-300 ease-in-out hover:shadow-lg"
        role="button"
      >
        <!-- Background image: black & white by default, colored on hover -->
        <div
          class="absolute inset-0 bg-cover bg-center grayscale filter transition duration-300 ease-in-out group-hover:grayscale-0"
          :style="{ backgroundImage: `url(${matchmakingBackground})` }"
        ></div>
        <!-- Content layer -->
        <div class="relative z-10 flex h-full flex-col justify-center p-8 text-center">
          <h2 class="mb-4 rounded-full bg-black/50 py-1 text-xl font-bold text-white">
            Create Lobby
          </h2>
        </div>
      </div>
      <!-- join lobby -->
      <div
        class="group relative col-span-1 cursor-default overflow-hidden rounded-xl shadow-md transition duration-300 ease-in-out hover:shadow-lg"
        role="button"
      >
        <!-- Background image: black & white by default, colored on hover -->
        <div
          class="absolute inset-0 bg-cover bg-center grayscale filter transition duration-300 ease-in-out group-hover:grayscale-0"
          :style="{ backgroundImage: `url(${matchmakingBackground})` }"
        ></div>
        <!-- Content layer -->
        <div class="relative z-10 flex h-full flex-col justify-center p-8 text-center">
          <h2 class="mb-4 rounded-full bg-black/50 py-1 text-xl font-bold text-white">
            Join Lobby
          </h2>
          <StandardInput
            placeholder="Room Code"
            maxlength="4"
            :autocapitalize="true"
            v-model="roomCodeInput"
            @keyup.enter="roomCodeInput.length === 4 ? joinRoom(roomCodeInput) : null"
          />
          <button
            class="btn-default mt-1 rounded-full"
            :disabled="roomCodeInput.length < 4"
            @click="joinRoom(roomCodeInput)"
          >
            Join
          </button>
        </div>
      </div>
    </div>
  </div>
  <div
    class="grid w-full grid-cols-1 gap-3 md:grid-cols-2 md:items-stretch lg:mx-auto lg:max-w-4xl"
  >
    <!-- Inner grid for columns -->
    <div class="col-span-1 flex flex-col rounded-xl p-8 text-center shadow-md dark:bg-gray-500/5">
      <h2 class="mb-4 text-xl font-bold">Public Matchmaking</h2>
      <div class="flex justify-center p-8 text-center">
        <div class="relative w-full">
          <button
            class="btn-default w-full"
            disabled
            @mouseenter="showTooltip = true"
            @mouseleave="showTooltip = false"
            @focus="showTooltip = true"
            @blur="showTooltip = false"
          >
            Join Matchmaking Queue
          </button>
          <div
            v-if="showTooltip"
            class="absolute bottom-full left-1/2 -translate-x-1/2 transform whitespace-nowrap rounded-md border border-black/10 bg-gray-50 px-3 py-2 text-sm text-black shadow-lg dark:border-white/10 dark:bg-black dark:text-gray-200"
          >
            Coming soon!
          </div>
        </div>
      </div>
    </div>
    <div class="col-span-1 rounded-xl p-8 text-center shadow-md dark:bg-gray-500/5">
      <h2 class="mb-4 text-xl font-bold">Private Lobby</h2>
      <div class="relative">
        <button class="btn-default w-full" @click="createRoom()">Create Room</button>
      </div>

      <div class="relative my-4 flex w-full">
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
</template>
