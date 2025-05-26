<script setup lang="ts">
import Avatar from '../common/Avatar.vue'
import { ref, onMounted } from 'vue'
import Api from '@/services/api'
import { usePlayerStore } from '@/stores/player'
import StandardInput from '../common/StandardInput.vue'
const { player, updatePlayer, randomizeColor } = usePlayerStore()
const username = ref(player.username)
const updateUsername = () => {
  const newUsername = username.value
  const updatedPlayer = { ...player, username: newUsername }
  updatePlayer(updatedPlayer)
  Api.updatePlayer(updatedPlayer)
  emit('update:modelValue', true)
}

const emit = defineEmits(['update:modelValue'])

onMounted(() => {
  randomizeColor()
})
</script>

<template>
  <main class="mt-10 flex w-full items-center justify-center">
    <div
      class="flex-item mt-4 rounded-lg border border-zinc-200 bg-white p-6 shadow dark:border-zinc-700 dark:bg-zinc-800"
    >
      <div
        class="flex flex-row items-center rounded-lg border border-gray-200 bg-white p-2 shadow dark:border-zinc-600 dark:bg-zinc-700"
      >
        <Avatar :username="player.username" :color="player.color" @click="randomizeColor" />
        <StandardInput type="text" v-model="username" />
      </div>

      <button
        class="flex-item mt-2 w-full rounded-md border-2 border-none bg-blue-500 px-4 py-2 font-semibold text-white hover:bg-blue-600 disabled:bg-blue-300 disabled:text-zinc-400 dark:bg-blue-600 dark:text-zinc-800 dark:hover:bg-blue-700 dark:disabled:bg-blue-300"
        @click="updateUsername"
      >
        Update Username
      </button>
    </div>
  </main>
</template>
