<script setup lang="ts">
import api from '@/api/api'
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router' // Added useRoute
import { useRoomStore } from '@/stores/room'
import UnshinyButton from '../common/UnshinyButton.vue'
import StandardInput from '../common/StandardInput.vue'
import SelectionCard from './SelectionCard.vue'
import matchmakingBackground from './matchmaking.jpg'
import createBackground from './create.jpg'
import joinBackground from './join.jpg'
import { ChevronLeft } from 'lucide-vue-next'

const { updateRoomCode, clearRoom } = useRoomStore()
const roomCodeInput = ref<string>('')
const isCreating = ref<boolean>(false)
const isJoining = ref<boolean>(false)
const router = useRouter()

const joinRoom = (codeToJoin: string) => {
  isJoining.value = true
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
  isCreating.value = true
  api
    .createRoom()
    .then((newlyCreatedCode) => {
      if (newlyCreatedCode) {
        joinRoom(newlyCreatedCode)
      } else {
        console.error('Failed to create room: No room code received from API.')
        // Optionally, add a user notification here
      }
    })
    .finally(() => {
      isCreating.value = false
    })
}

onMounted(() => {
  clearRoom()
})
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center space-y-4 p-10">
    <RouterLink class="flex" :to="{ name: 'home' }">
      <UnshinyButton>
        <ChevronLeft class="mr-2 mt-[1px]" />
        Back Home
      </UnshinyButton>
    </RouterLink>
    <div
      class="grid w-full grid-cols-1 gap-4 md:grid-cols-3 md:items-stretch lg:mx-auto lg:max-w-4xl"
    >
      <SelectionCard
        header="Join Matchmaking"
        :backgroundImage="matchmakingBackground"
        :disabled="true"
      >
        <p class="mx-auto w-fit rounded-full bg-black/50 px-3 py-1 text-white">Coming Soon</p>
      </SelectionCard>
      <SelectionCard
        class="cursor-pointer"
        header="Create Lobby"
        :backgroundImage="createBackground"
        :isLoading="isCreating"
        @click="createRoom()"
      />
      <SelectionCard header="Join Lobby" :backgroundImage="joinBackground" :isLoading="isJoining">
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
      </SelectionCard>
    </div>
  </div>
</template>
