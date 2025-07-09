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
      class="grid h-96 w-full grid-cols-1 gap-4 md:grid-cols-3 md:items-stretch lg:mx-auto lg:max-w-4xl"
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
        class="group relative col-span-1 transform overflow-hidden rounded-xl shadow-md transition duration-300 ease-in-out hover:scale-105 hover:shadow-lg"
        @click="createRoom()"
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
        class="group relative col-span-1 transform cursor-default overflow-hidden rounded-xl shadow-md transition duration-300 ease-in-out hover:scale-105 hover:shadow-lg"
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
</template>
