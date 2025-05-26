<script setup lang="ts">
import RoomWrapper from '@/components/room/RoomWrapper.vue'
import { useRoomStore } from '@/stores/room'
import { ref, onMounted } from 'vue'
import Api from '@/services/api'
import StandardInput from '@/components/common/StandardInput.vue'

const roomExists = ref(false)
const roomCode = ref('')
const errorMessage = ref('')
const { room, updateRoomCode } = useRoomStore()

const updateCode = (code: string) => {
  if (!code) {
    errorMessage.value = '😭 Please enter a room code.'
    return
  }
  code = code.toUpperCase()
  updateRoomCode(code)
  checkRoom(code)
}

const checkRoom = (code: string) => {
  errorMessage.value = ''
  if (!code) return
  Api.checkRoom(room.code).then((exists) => {
    roomExists.value = exists
    if (!exists) {
      errorMessage.value = '😭 This game has already ended.'
    }
  })
}

onMounted(() => {
  checkRoom(room.code)
})
</script>

<template>
  <main>
    <div
      v-if="!roomExists"
      class="flex h-screen flex-col items-center justify-center bg-gradient-to-br from-indigo-50 to-purple-100 dark:from-zinc-900 dark:to-purple-900"
    >
      <div
        v-if="errorMessage"
        class="mb-4 flex items-center justify-center rounded-sm border-red-200 bg-red-50 px-4 py-1 dark:text-slate-700"
      >
        {{ errorMessage }}
      </div>
      <div
        class="flex-item mt-4 rounded-lg border border-zinc-200 bg-white p-6 shadow-md dark:border-zinc-700 dark:bg-zinc-800"
      >
        <div class="flex items-center space-x-2">
          <StandardInput
            placeholder="Room Code"
            maxlength="4"
            v-model="roomCode"
            v-autocapitalize="true"
            @keyup.enter="roomCode.length === 4 ? updateCode(roomCode) : null"
          />
          <button
            class="w-auto rounded-md bg-indigo-600 px-4 py-2 font-semibold text-white hover:bg-indigo-700 disabled:cursor-not-allowed disabled:bg-indigo-300 dark:bg-indigo-500 dark:hover:bg-indigo-600 dark:disabled:bg-indigo-800 dark:disabled:text-indigo-500"
            :disabled="roomCode.length < 4"
            @click="updateCode(roomCode)"
          >
            Join
          </button>
        </div>
      </div>
    </div>
    <RoomWrapper v-else />
  </main>
</template>
