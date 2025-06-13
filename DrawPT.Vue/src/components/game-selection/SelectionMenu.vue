<script setup lang="ts">
import api from '@/services/api'
import { ref } from 'vue'
import { useRouter } from 'vue-router' // Added useRoute
import { useRoomStore } from '@/stores/room'
import FlickeringGrid from '@/components/common/FlickeringGrid.vue'
import StandardInput from '../common/StandardInput.vue'

const showTooltip = ref(false)

const { updateRoomCode } = useRoomStore()
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
</script>

<template>
  <div class="flex min-h-screen items-center justify-center p-10">
    <!-- Outer centering container -->
    <div
      class="grid w-full grid-cols-1 gap-3 md:grid-cols-2 md:items-stretch lg:mx-auto lg:max-w-4xl"
    >
      <!-- Inner grid for columns -->
      <div
        class="col-span-1 flex flex-col justify-center rounded-xl p-8 text-center shadow-md dark:bg-gray-500/5"
      >
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
          class="absolute bottom-full left-1/2 mb-2 -translate-x-1/2 transform whitespace-nowrap rounded-md border border-black/10 bg-gray-50 px-3 py-2 text-sm text-black shadow-lg dark:border-white/10 dark:bg-black dark:text-gray-200"
        >
          Coming soon!
        </div>
      </div>
      <div class="col-span-1 rounded-xl p-8 text-center shadow-md dark:bg-gray-500/5">
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
  </div>
</template>
