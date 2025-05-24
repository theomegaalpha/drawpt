<script setup lang="ts">
import RoomWrapper from '@/components/room/RoomWrapper.vue'
import { useRoomStore } from '@/stores/room'
import { ref, onMounted } from 'vue'
import Api from '@/services/api'

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
    <div v-if="!roomExists" class="flex h-screen flex-col items-center justify-center">
      <div
        v-if="errorMessage"
        class="mb-4 flex items-center justify-center rounded-sm border-red-200 bg-red-50 px-4 py-1 dark:text-slate-700"
      >
        {{ errorMessage }}
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
            @keyup.enter="roomCode.length === 4 ? updateCode(roomCode) : null"
          />
          <button
            class="flex-item w-40 rounded-md border-2 border-none bg-blue-500 px-4 py-2 font-semibold text-white hover:bg-blue-600 disabled:bg-blue-300 disabled:text-zinc-400 dark:bg-blue-600 dark:text-zinc-800 dark:hover:bg-blue-700 dark:disabled:bg-blue-300"
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
