<script setup lang="ts">
import type { GameAnswer } from '@/models/gameModels'
import { ref, computed } from 'vue'
const props = defineProps<{
  username?: string
  isYou?: boolean
  answer: GameAnswer
}>()

const showReason = ref(props.isYou)

const initials = computed(() => {
  const names = props.username?.split(' ')
  if (!names) return ''
  let initials = names[0].substring(0, 1).toUpperCase()

  if (names.length > 1) {
    initials += names[names.length - 1].substring(0, 1).toUpperCase()
  }

  return initials
})
const avatarColor = computed(() => {
  return '#' + Math.floor(Math.random() * 16777215).toString(16)
})
</script>

<template>
  <div
    class="w-[70vw] flex justify-center items-center mt-4 bg-white border-gray-200 rounded-lg shadow dark:bg-gray-800 dark:border-gray-700"
    :class="[isYou ? 'border-4 border-green-400 dark:border-green-400' : 'border']"
  >
    <div class="w-full flex p-5">
      <div class="flex-col group relative w-max">
        <div
          class="cursor-default w-10 h-10 p-2 rounded-full flex items-center justify-center text-white text-sm font-bold"
          :style="{ backgroundColor: avatarColor }"
          aria-label="avatar"
        >
          {{ initials }}
        </div>
        <span
          class="cursor-default pointer-events-none px-2 bg-gray-200 dark:bg-gray-600 border border-gray-600 dark:border-gray-200 absolute -top-7 translate-x-[-25%] w-max opacity-0 transition-opacity group-hover:opacity-100"
        >
          {{ username }}
        </span>
      </div>
      <div class="flex-col cursor-pointer" @click="showReason = !showReason">
        <div
          v-if="answer.guess !== ''"
          class="flex px-6 justify-center ml-4 border border-white p-1 rounded-sm"
        >
          {{ answer.guess }}
        </div>
        <div
          v-if="showReason"
          class="flex px-6 justify-center ml-4 border border-white p-1 rounded-sm mt-1"
        >
          {{ answer.reason }}
        </div>
      </div>
    </div>
    <div class="mr-5 flex items-end">
      <span>{{ answer.score }}</span>
      <span v-if="answer.bonusPoints > 0" class="ml-1 text-green-400">
        +{{ answer.bonusPoints }}
      </span>
      <span class="ml-1 text-xs">pts</span>
    </div>
  </div>
</template>
